using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedroomController : MonoBehaviour
{
    // TODO: code in hot spots? ex: greater score for being near certain positions for certain items

    // How much to divide each unity meter tile into
    // Ex: 2.0f = 1 unity square meter will be 4 tiles.
    public int xDim; // The # of unity units from the controller to the x edge of the room
    public int zDim; // The # of unity units from the controller to the z edge of the room
    public int gridFactor = 2;
    public int height;

    void Start()
    {
        GameObject objController = new GameObject();
        objController.transform.position = this.transform.position;
        objController.AddComponent<ObjSpawner>();
        ObjSpawner objSpawner = objController.GetComponent<ObjSpawner>();
        objSpawner.init(xDim, zDim, gridFactor);

        // Populate bedroom with basic furniture
        // Pick a bed
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            GameObject bed = Resources.Load("Toon Furniture/Prefabs/Single_Bed_1") as GameObject;
            ScoreParams bedScoreParams = new ScoreParams(wallScore: 1, groupScore: -1, spread: 0.0f, messiness: 0.0f);
            objSpawner.SpawnObj(bed, new Vector3(6.0f, 2.0f, 3.0f), new Vector3(0.0f, 90.0f, 0.0f), bedScoreParams, dropFactor: 0.01f);
        }
        else
        {
            GameObject bed = Resources.Load("Toon Furniture/Prefabs/Queen_Bed_2") as GameObject;
            ScoreParams bedScoreParams = new ScoreParams(wallScore: 1, groupScore: -1, spread: 0.0f, messiness: 0.0f);
            objSpawner.SpawnObj(bed, new Vector3(4.0f, 2.0f, 2.0f), new Vector3(0.0f, 90.0f, 0.0f), bedScoreParams, dropFactor: 0.01f);
        }

        // Pick a desk
        GameObject desk = Resources.Load("Toon Furniture/Prefabs/Table_1") as GameObject;
        ScoreParams deskScoreParams = new ScoreParams(wallScore: 1, groupScore: -1, spread: 0.0f, messiness: 0.0f);
        GameObject deskObj = objSpawner.SpawnObj(desk, new Vector3(2.0f, 1.0f, 4.0f), new Vector3(-90.0f, 90.0f, 0.0f), deskScoreParams, dropFactor: 0.01f);
        deskObj.AddComponent<DeskController>();

        // Pick a plant
        GameObject plant = Resources.Load("Toon Furniture/Prefabs/House_Plant_1") as GameObject;
        ScoreParams plantScoreParams = new ScoreParams(wallScore: 1, groupScore: -2, spread: 0.0f, messiness: 0.0f);
        objSpawner.SpawnObj(plant, new Vector3(1.0f, 1.0f, 1.0f), Vector3.zero, plantScoreParams, dropFactor: 0.01f);
    }
}
