using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjSpawner : MonoBehaviour
{
    // TODO: code in hot spots? ex: greater score for being near certain positions for certain items

    public GameObject entrance; // TODO: factor in entrance
    int xDim; // The # of tiles from the controller to the x edge of the room
    int zDim; // The # of tiles from the controller to the z edge of the room
    float gridFactor; // Factor by which we multiple tile length
    bool[,] isTaken;

    public void init(int xDim, int zDim, float gridFactor)
    {
        this.gridFactor = gridFactor;
        this.xDim = xDim;
        this.zDim = zDim;
        this.isTaken = new bool[xDim, zDim];
    }

    // Updates all points in the grid to be taken, assuming a cube of size (xLen, zLen) was 
    // spawned at xStart, zStart. Also assumes the cube has already been spawned and won't
    // collide with any walls.
    public void UpdateGrid(int xStart, int zStart, float xLen, float zLen)
    {
        for (int x = xStart; x < xStart + xLen; x++)
            for (int z = zStart; z < zStart + zLen; z++)
                this.isTaken[x, z] = true;
    }


    // Checks if all tiles for an object of size xLen by zlen are free, and calculates a
    // a score for being against a wall or other objects
    //TODO: Account for stacking?
    private (bool, float) GetPlacementScore(
        int xStart, int zStart, float xLen, float zLen, float wallScore, float groupScore, float messiness)
    {
        // Quick check to see if we will collide into walls
        if (xStart + xLen - 1 >= this.xDim || zStart + zLen - 1 >= this.zDim)
            return (false, 0.0f);

        // Check tiles, increment score if tile adjacent to wall or taken spot
        float score = 0.0f;
        for (int x = xStart; x < xStart + xLen; x++)
            for (int z = zStart; z < zStart + zLen; z++)
                if (this.isTaken[x, z])
                    return (false, -1);
                else
                {
                    if (x == 0 || x == this.xDim - 1)
                        score += wallScore;
                    else if (this.isTaken[x - 1, z] || this.isTaken[x + 1, z])
                        score += groupScore;
                    if (z == 0 || z == this.zDim - 1)
                        score += wallScore;
                    else if (this.isTaken[x, z - 1] || this.isTaken[x, z + 1])
                        score += groupScore;
                }

        return (true, score + Random.Range(-messiness, messiness));
    }

    // Picks the best spot out of the list of spots and updates the grid accordingly.
    private (int, int) PickSpot(List<(int, int, float)> spots, Vector3 size)
    {
        spots.Sort((x, y) => x.Item3.CompareTo(y.Item3));
        float highestScore = spots[spots.Count - 1].Item3;

        List<(int, int, float)> bestSpots = spots.FindAll(spot => spot.Item3 == highestScore);
        int index = Random.Range(0, bestSpots.Count);

        int xPos = bestSpots[index].Item1;
        int zPos = bestSpots[index].Item2;

        UpdateGrid(xPos, zPos, size.x, size.z);
        return (xPos, zPos);
    }


    // Spawn a cube with the specified size in an available spot in the grid,
    // if possible. Returns the spawned GameObject Cube if it was spawned successfully,
    // or null if unsuccessful.
    GameObject SpawnCube(Vector3 size, int wallScore, int groupScore, float messiness)
    {
        // Find open spot to spawn cube
        List<(int, int, float)> spots = new List<(int, int, float)>();
        for (int x = 0; x < this.xDim; x++)
            for (int z = 0; z < this.zDim; z++)
            {
                (bool, float) score = GetPlacementScore(x, z, size.x, size.z, wallScore, groupScore, messiness);
                if (score.Item1)
                    spots.Add((x, z, score.Item2));
            }

        // If there are no available spots, return null.
        if (spots.Count == 0)
            return null;

        // Pick a spot, taking into account wall score if necessary
        (int, int) spot = PickSpot(spots, size);

        // Create placeholder cube
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.GetComponent<Renderer>().enabled = false;


        // Spawn object and scale it appropriately
        // Objects need to slightly rise up if not on floor, since floor is at y=0
        obj.transform.localScale = size / this.gridFactor;

        float xPos = this.transform.position.x + ((spot.Item1 / this.gridFactor) + (size.x / (2.0f * this.gridFactor)));
        float zPos = this.transform.position.z + ((spot.Item2 / this.gridFactor) + (size.z / (2.0f * this.gridFactor)));

        obj.transform.position = new Vector3(xPos, this.transform.position.y, zPos);

        return obj;
    }

    // Fits an object to a placeholder cube with a specific rotation, destroying 
    // the cube in the process.
    GameObject FitToCube(GameObject cube, GameObject obj, Vector3 size, Vector3 rot)
    {
        // Initialize object with original scale and rotation at center of cube
        GameObject instantiatedObj = Instantiate(obj, cube.transform.position, obj.transform.rotation * Quaternion.Euler(rot));

        // Get object and cube bounds
        Vector3 cubeSize = cube.GetComponent<Renderer>().bounds.size;
        Vector3 objSize = instantiatedObj.GetComponent<BoxCollider>().bounds.size;

        if (rot.y % 180.0f != 0)
            instantiatedObj.transform.localScale = new Vector3(cubeSize.z / objSize.z, cubeSize.y / objSize.y, cubeSize.x / objSize.x);
        else
            instantiatedObj.transform.localScale = new Vector3(cubeSize.x / objSize.x, cubeSize.y / objSize.y, cubeSize.z / objSize.z);

        Destroy(cube);
        return instantiatedObj;
    }

    public GameObject SpawnObj(GameObject resource, Vector3 size, Vector3 rot, int wallScore, int groupScore, float messiness)
    {
        GameObject cube = SpawnCube(size, wallScore, groupScore, messiness);
        if (cube == null)
        {
            Debug.Log("No spots left!");
            return null;
        }
            return FitToCube(cube, resource, size, rot);
    }
}
