using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingRoomController : RoomController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        // Call the parent start
        base.Start();

        // not inclusive for ints
        int couch_num = Random.Range(1, 4);
        GameObject couch;
        ScoreParams couchScoreParams = new ScoreParams(wallScore: 1, groupScore: -1, spread: 0.0f, messiness: 0.0f);
        switch (couch_num)
        {
            case 1:
                couch = Resources.Load("Toon Furniture/Prefabs/Couch_1") as GameObject;
                objSpawner.SpawnObj(couch, new Vector3(2f, 2f, 5f), new Vector3(-90.0f, 90.0f, 0.0f), couchScoreParams, dropFactor: 0.01f);
                break;
            case 2:
                // L shaped couch
                couch = Resources.Load("Toon Furniture/Prefabs/Couch_2") as GameObject;
                objSpawner.SpawnObj(couch, new Vector3(2f, 2f, 5f), new Vector3(-90.0f, 90.0f, 0.0f), couchScoreParams, dropFactor: 0.01f);
                break;
            case 3:
                couch = Resources.Load("Toon Furniture/Prefabs/Couch_3") as GameObject;
                objSpawner.SpawnObj(couch, new Vector3(2f, 2f, 5f), new Vector3(-90.0f, 90.0f, 0.0f), couchScoreParams, dropFactor: 0.01f);
                break;
        }
        //TODO: Rotate the couch so it faces the center of the room
        //TODO: Place a coffee table in front of the couch

        // Pick a plant and put it somewhere
        GameObject plant = Resources.Load("Toon Furniture/Prefabs/House_Plant_1") as GameObject;
        ScoreParams plantScoreParams = new ScoreParams(wallScore: 1, groupScore: -2, spread: 0.0f, messiness: 0.0f);
        objSpawner.SpawnObj(plant, new Vector3(1.0f, 1.0f, 1.0f), Vector3.zero, plantScoreParams, dropFactor: 0.01f);

    }
}
