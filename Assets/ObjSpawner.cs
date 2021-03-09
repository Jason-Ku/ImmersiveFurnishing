using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjSpawner : MonoBehaviour
{
    // TODO: code in hot spots? ex: greater score for being near certain positions for certain items

    public GameObject entrance; // TODO: factor in entrance
    int xDim; // The # of tiles from the controller to the x edge of the room (controller is in -x, -z quadrant corner)
    int zDim; // The # of tiles from the controller to the z edge of the room
    float gridFactor; // Factor by which we multiple tile length
    bool[,] isTaken;


    /**
     * Initializes the ObjSpawner grid system.
     * 
     * xDim: The # of tiles from the controller to the x edge of the room
     * zDim: The # of tiles from the controller to the z edge of the room
     * gridFactor: Factor by which we multiple tile length
     */
    public void init(int xDim, int zDim, float gridFactor)
    {
        this.gridFactor = gridFactor;
        this.xDim = xDim;
        this.zDim = zDim;
        this.isTaken = new bool[xDim, zDim];
    }


    /**
     * Updates points in the grid to be taken, assuming a rectangle of size (xLen, zLen) was 
     * spawned at xStart, zStart. Also assumes the rectangle has already been spawned and won't
     * collide with any walls because we already ensure we don't spawn anything inside walls.
     * 
     * xStart: x-coord of spawn point in grid
     * zStart: z-coord of spawn point in grid
     * xLen: x-length of rectangle in grid units
     * zLen: z-length of rectangle in grid units
     */
    public void UpdateGrid(int xStart, int zStart, float xLen, float zLen)
    {
        for (int x = xStart; x < xStart + xLen; x++)
            for (int z = zStart; z < zStart + zLen; z++)
                this.isTaken[x, z] = true;
    }


    /**
     * Checks if all tiles for a rectangle of size xLen by zlen are free, and calculates a
     * a score via various parameters.
     * TODO: Account for stacking?
     * 
     * xStart: x-coord of spawn point in grid
     * zStart: z-coord of spawn point in grid
     * size: size of object in grid units
     * scoreParams: scoring parameters
     */
    private (bool, float) GetPlacementScore(int xStart, int zStart, Vector3 size, ScoreParams scoreParams)
    {
        // Quick check to see if we will collide into walls
        if (xStart + size.x - 1 >= this.xDim || zStart + size.z - 1 >= this.zDim)
            return (false, -1);

        // Check tiles, increment score if tile adjacent to wall or taken spot
        float score = 0.0f;
        for (int x = xStart; x < xStart + size.x; x++)
            for (int z = zStart; z < zStart + size.z; z++)
                if (this.isTaken[x, z])
                    return (false, -1);
                else
                {
                    if (x == 0 || x == this.xDim - 1)
                        score += scoreParams.wallScore;
                    else if (this.isTaken[x - 1, z] || this.isTaken[x + 1, z])
                        score += scoreParams.groupScore;
                    else
                        score += scoreParams.spread;
                    if (z == 0 || z == this.zDim - 1)
                        score += scoreParams.wallScore;
                    else if (this.isTaken[x, z - 1] || this.isTaken[x, z + 1])
                        score += scoreParams.groupScore;
                    else
                        score += scoreParams.spread;
                }

        // Fuzz score with messiness value
        return (true, score + Random.Range(-scoreParams.messiness, scoreParams.messiness));
    }


    /**
     * Picks the best spot out of the list of spots for an object of the given size and updates the grid accordingly.
     * 
     * spots: A list of tuples describing potential spawn spots and their scores
     * size: Size of the object to spawn in grid units
     * 
     * Returns: A tuple describing the best spot for an object of a given size.
     */
    private (int, int) PickSpot(List<(int, int, float)> spots, Vector3 size)
    {
        // Sort available spots by score
        spots.Sort((x, y) => x.Item3.CompareTo(y.Item3));
        float highestScore = spots[spots.Count - 1].Item3;
        List<(int, int, float)> bestSpots = spots.FindAll(spot => spot.Item3 == highestScore);

        // Pick a random spot out of the ones with the highest score
        int index = Random.Range(0, bestSpots.Count);

        // Update grid and return spot
        int xPos = bestSpots[index].Item1;
        int zPos = bestSpots[index].Item2;
        UpdateGrid(xPos, zPos, size.x, size.z);

        return (xPos, zPos);
    }


    private List<(int, int, float)> getAvailableSpots(Vector3 size, ScoreParams scoreParams)
    {
        // Find open spot to spawn cube
        List<(int, int, float)> spots = new List<(int, int, float)>();
        for (int x = 0; x < this.xDim; x++)
            for (int z = 0; z < this.zDim; z++)
            {
                (bool, float) score = GetPlacementScore(x, z, size, scoreParams);
                if (score.Item1)
                    spots.Add((x, z, score.Item2));
            }

        // If there are no available spots, return null.
        if (spots.Count == 0)
        {
            Debug.Log("No spots left!");
            return null;
        }
        return spots;
    }

    /**
     * Return a boolean to determine whether to rotate an object 180 degrees based on
     * where it is located in the room. (wallAlign string is typically the longer edge)
     * The axis parameter determines what axis we will look at to determine whether we
     * need to flip the object.
     * 
     * axis: string that is "x" or "z" (perhaps there is a better system than this. THis will do for now)
     *       Can also specify something other than x or z (like an empty string) for no flipping preference
     * x: x coordinate of object in world units
     * z: z coordinate of object in world units
     */
    public bool BackToWall(string axis, float x, float z)
    {
        if (axis == "x")
            return x > this.transform.position.z + (this.xDim * 0.5f / this.gridFactor);
        else if (axis == "z")
            return z > this.transform.position.x + (this.zDim * 0.5f / this.gridFactor);
        else
        {
            Debug.Log("Invalid axis or none specified");
            return false;
        }
    }


    /**
     * Same as below, except without an axis specified.
     */
    public GameObject SpawnObj(GameObject resource, Vector3 size, Vector3 rot, ScoreParams scoreParams, float dropFactor)
    {
        return SpawnObj(resource, size, rot, scoreParams, "", dropFactor);
    }


    /**
     * Spawn a cube with the specified size in an available spot in the grid,
     * if possible. Returns the spawned GameObject Cube if it was spawned successfully,
     * or null if unsuccessful.
     * 
     * size: Size of the object to spawn in grid units
     * scoreParams: Scoring parameters for object placement
     * axis: Axis to check for flipping object to face center
     * dropFactor: How high to drop the object from in physics sim
     */
    public GameObject SpawnObj(GameObject resource, Vector3 size, Vector3 rot, ScoreParams scoreParams, string axis, float dropFactor)
    {
        // Find open spot to spawn cube
        List<(int, int, float)> spots = getAvailableSpots(size, scoreParams);
        if (spots == null)
        {
            return null;
        }

        // Pick a spot, taking into account wall score if necessary
        (int, int) spot = PickSpot(spots, size);
        float xPos = this.transform.position.x + ((spot.Item1 / this.gridFactor) + (size.x / (2.0f * this.gridFactor)));
        float zPos = this.transform.position.z + ((spot.Item2 / this.gridFactor) + (size.z / (2.0f * this.gridFactor)));

        // Spawn a placeholder cube to help with resizing
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = size / this.gridFactor;
        cube.GetComponent<Renderer>().enabled = false;
        Vector3 cubeSize = cube.GetComponent<Renderer>().bounds.size;
        Destroy(cube);

        // Spawn new object at right place and with correct rotation
        Vector3 spawnCoords = new Vector3(xPos, this.transform.position.y + (resource.GetComponent<Rigidbody>() == null ? 0.0f : dropFactor), zPos);
        GameObject instantiatedObj = Instantiate(resource, spawnCoords, Quaternion.Euler(rot));
        Vector3 objSize = instantiatedObj.GetComponent<BoxCollider>().bounds.size;

        // Fix weird scaling issue if the object was rotated around y axis
        if (rot.y % 180.0f != 0)
            instantiatedObj.transform.localScale = new Vector3(cubeSize.z / objSize.z, cubeSize.y / objSize.y, cubeSize.x / objSize.x);
        else
            instantiatedObj.transform.localScale = new Vector3(cubeSize.x / objSize.x, cubeSize.y / objSize.y, cubeSize.z / objSize.z);

        // Flip object so its back is to the wall, if necessary
        if (BackToWall(axis, xPos, zPos))
            instantiatedObj.transform.RotateAround(instantiatedObj.transform.position, Vector3.up, 180.0f);

        return instantiatedObj;
    }


    /* UNTESTED
     * Converts a worldSpace point to grid space, ignoring Y values.
     * 
     * worldSpace: Vector3 of the point in world space
     */
    private Vector2 worldSpaceToGridSpace(Vector3 worldSpace)
    {
        Vector3 relativePosition = worldSpace - transform.position;
        int xPos = Mathf.FloorToInt(relativePosition.x * this.gridFactor);
        int yPos = Mathf.FloorToInt(relativePosition.y * this.gridFactor);
        return new Vector2(xPos, yPos);
    }


    /** UNTESTED
     * Spawn a cube with the specified size near a specified spot in the grid,
     * if possible. Returns the spawned GameObject Cube if it was spawned successfully,
     * or null if unsuccessful.
     * 
     * This is useful for things like coffee tables which almost always are placed relative to something else
     * 
     * size: Size of the object to spawn in grid units
     * scoreParams: Scoring parameters for object placement
     * dropFactor: How high to drop the object from in physics sim
     * location: Where we want to spawn the object near
     */
    public GameObject SpawnObjNearLocation(GameObject resource, Vector3 size, Vector3 rot, ScoreParams scoreParams, float dropFactor, Vector3 location, float maxDistance = 2.0f)
    {
        // Find open spot to spawn cube
        List<(int, int, float)> spots = getAvailableSpots(size, scoreParams);
        if (spots == null)
        {
            return null;
        }

        // valid spots are close enough to the target location
        List<(int, int, float)> validSpots = new List<(int, int, float)>();
        Vector2 target = worldSpaceToGridSpace(location);
        foreach ((int, int, float) spotI in spots)
        {
            Vector2 spotVect = new Vector2(spotI.Item1, spotI.Item2);
            if (Vector2.Distance(spotVect, target) <= maxDistance)
            {
                validSpots.Add(spotI);
            }
        }
        (int, int) spot = PickSpot(validSpots, size);
        float xPos = this.transform.position.x + ((spot.Item1 / this.gridFactor) + (size.x / (2.0f * this.gridFactor)));
        float zPos = this.transform.position.z + ((spot.Item2 / this.gridFactor) + (size.z / (2.0f * this.gridFactor)));

        // Spawn a placeholder cube to help with resizing
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = size / this.gridFactor;
        cube.GetComponent<Renderer>().enabled = false;
        Vector3 cubeSize = cube.GetComponent<Renderer>().bounds.size;
        Destroy(cube);

        // Spawn new object at right place and with correct rotation
        Vector3 spawnCoords = new Vector3(xPos, this.transform.position.y + (resource.GetComponent<Rigidbody>() == null ? 0.0f : dropFactor), zPos);
        GameObject instantiatedObj = Instantiate(resource, spawnCoords, Quaternion.Euler(rot));

        Vector3 objSize = instantiatedObj.GetComponent<BoxCollider>().bounds.size;

        // Fix weird scaling issue if the object was rotated around y axis
        if (rot.y % 180.0f != 0)
            instantiatedObj.transform.localScale = new Vector3(cubeSize.z / objSize.z, cubeSize.y / objSize.y, cubeSize.x / objSize.x);
        else
            instantiatedObj.transform.localScale = new Vector3(cubeSize.x / objSize.x, cubeSize.y / objSize.y, cubeSize.z / objSize.z);

        return instantiatedObj;
    }
}
