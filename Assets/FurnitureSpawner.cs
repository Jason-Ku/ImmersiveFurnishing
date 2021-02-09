using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpawner : MonoBehaviour
{
    // TODO: code in hot spots? ex: greater score for being near certain positions for certain items

    // How much to divide each unity meter tile into
    // Ex: 2.0f = 1 unity square meter will be 4 tiles.
    public const int gridFactor = 4;

    public int xDim; // The # of unity units from the controller to the x edge of the room
    public int zDim; // The # of unity units from the controller to the z edge of the room
    public int height;
    public GameObject entrance;
    public int messy; // int for now, float later

    int gridX;
    int gridZ;
    bool[,] isTaken;


    // Updates all points in the grid to be taken, assuming a cube of size (xLen, zLen) was 
    // spawned at xStart, zStart. Also assumes the cube has already been spawned and won't
    // collide with any walls.
    void UpdateGrid(int xStart, int zStart, float xLen, float zLen)
    {
        for (int x = xStart; x < xStart + xLen; x++)
            for (int z = zStart; z < zStart + zLen; z++)
                isTaken[x, z] = true;
    }


    // Checks if all tiles for an object of size xLen by zlen are free, and calculates a
    // a score for being against a wall or other objects
    //TODO: Account for stacking?
    (bool, int) GetPlacementScore(int xStart, int zStart, float xLen, float zLen)
    {
        // Quick check to see if we will collide into walls
        if (xStart + xLen - 1 >= gridX || zStart + zLen - 1 >= gridZ)
            return (false, 0);

        // Check tiles, increment score if tile adjacent to wall or taken spot
        int score = 0;
        for (int x = xStart; x < xStart + xLen; x++)
            for (int z = zStart; z < zStart + zLen; z++)
                if (isTaken[x, z])
                    return (false, 0);
                else
                {
                    if (x == 0 || x == gridX - 1)
                        score++;
                    else if (isTaken[x - 1, z] || isTaken[x + 1, z])
                        score++;
                    if (z == 0 || z == gridZ - 1)
                        score++;
                    else if (isTaken[x, z - 1] || isTaken[x, z + 1])
                        score++;
                }

        return (true, score);
    }


    // Spawn a cube with the specified size in an available spot in the grid,
    // if possible. Returns the spawned GameObject Cube if it was spawned successfully,
    // or null if unsuccessful.
    GameObject SpawnCube(Vector3 size)
    {
        // Find open spot to spawn cube
        List<(int, int, int)> spots = new List<(int, int, int)>();

        for (int x = 0; x < gridX; x++)
            for (int z = 0; z < gridZ; z++)
            {
                (bool, int) score = GetPlacementScore(x, z, size.x, size.z);
                if (score.Item1)
                    spots.Add((x, z, score.Item2));
            }

        // If there are no available spots, return null. HANDLE THIS IN CALLER FUNCTION!
        // TODO: OR switch this to error handling.
        if (spots.Count == 0)
        {
            Debug.Log("No spots left!");
            return null;
        }
        
        // Pick a spot, taking into account wall score if necessary
        int index;
        float xPos;
        float zPos;
        if (messy == 1)
        {
            index = Random.Range(0, spots.Count);
            xPos = (float)spots[index].Item1 / (float)gridFactor;
            zPos = (float)spots[index].Item2 / (float)gridFactor;
            UpdateGrid(spots[index].Item1, spots[index].Item2, size.x, size.z);
        }
        else
        {
            spots.Sort((x, y) => x.Item3.CompareTo(y.Item3));
            int highestScore = spots[spots.Count - 1].Item3;

            List<(int, int, int)> bestSpots = spots.FindAll(spot => spot.Item3 == highestScore);
            index = Random.Range(0, bestSpots.Count);

            xPos = (float)bestSpots[index].Item1 / (float)gridFactor;
            zPos = (float)bestSpots[index].Item2 / (float)gridFactor;
            UpdateGrid(bestSpots[index].Item1, bestSpots[index].Item2, size.x, size.z);
        }

        Debug.Log(string.Format("{0}, {1}", xPos, zPos));

        // Create an obj and scale it down a bit
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.GetComponent<Renderer>().enabled = false;

        // Spawn object and scale it appropriately
        Vector3 currentPos = this.transform.position;
        obj.transform.position = new Vector3(currentPos.x + xPos + (size.x / 2.0f), 0.0f, currentPos.z + zPos + (size.z / 2.0f));
        obj.transform.localScale = size / gridFactor;

        // Update grid array with taken spots

        return obj;
    }

    void FitToCube(GameObject cube, GameObject obj, Vector3 rot)
    {
        // Initialize object with original scale and rotation at center of cube
        GameObject instantiatedObj = Instantiate(obj, cube.transform.position, obj.transform.rotation * Quaternion.Euler(rot));

        // Get object and cube bounds
        Vector3 cubeSize = cube.GetComponent<Renderer>().bounds.size;
        Vector3 objSize = instantiatedObj.GetComponent<BoxCollider>().bounds.size;

        instantiatedObj.transform.localScale = new Vector3(cubeSize.x / objSize.x, cubeSize.y / objSize.y, cubeSize.z / objSize.z);
    }


    void Start()
    {
        gridX = xDim * gridFactor;
        gridZ = zDim * gridFactor;
        isTaken = new bool[gridX, gridZ];

        //GameObject cube = SpawnCube(new Vector3(2.0f, 1.0f, 1.0f));
        //GameObject cube2 = SpawnCube(new Vector3(2.0f, 1.0f, 1.0f));

        //FitToCube(cube, Resources.Load("Toon Furniture/Prefabs/Single_Bed_1") as GameObject, new Vector3(0.0f, 0.0f, 0.0f));
        //FitToCube(cube2, Resources.Load("Toon Furniture/Prefabs/Table_1") as GameObject, new Vector3(-90.0f, 0.0f, 0.0f));

        for (int i = 0; i < 10; i++)
        {
            GameObject resource;
            int item = Random.Range(0, 3);

            if (item == 0)
                resource = Resources.Load("kitchen props/Prefabs/plate-stack-small") as GameObject;
            else if (item == 1)
                resource = Resources.Load("kitchen props/Prefabs/plate-stack-large") as GameObject;
            else
                resource = Resources.Load("kitchen props/Prefabs/bowl-stack") as GameObject;

            GameObject cube = SpawnCube(new Vector3(1.0f, 1.0f, 1.0f));
            FitToCube(cube, resource, new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
        }
    }
}
