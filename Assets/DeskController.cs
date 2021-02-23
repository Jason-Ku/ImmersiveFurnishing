using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskController : MonoBehaviour
{
    // This DeskController script is attached to the top surface corner of a desk.
    // Spawns items on the desk!

    void Start()
    {
        GameObject objController = new GameObject();
        
        // Put controller object on corner of top surface so objects spawn in correct locations
        Vector3 objSize = this.GetComponent<Renderer>().bounds.size;
        objController.transform.position = this.transform.position + new Vector3(-objSize.x / 2.0f, objSize.y, -objSize.z / 2.0f);

        // Instantiate and initialize object spawner
        objController.AddComponent<ObjSpawner>();
        ObjSpawner objSpawner = objController.GetComponent<ObjSpawner>();

        // TODO: Need a better way to find a good grid size...
        objSpawner.init(Mathf.RoundToInt(objSize.x) * 32, Mathf.RoundToInt(objSize.y) * 32, 40);

        // Spawn some random desk items...
        for (int i = 0; i < 6; i++)
        {
            GameObject resource;
            int item = Random.Range(0, 3);  

            if (item == 0)
                resource = Resources.Load("Office Supplies Low Poly/Assets/Prefabs/Pencil black") as GameObject;
            else if (item == 1)
                resource = Resources.Load("Office Supplies Low Poly/Assets/Prefabs/Pen blue") as GameObject;
            else
                resource = Resources.Load("Office Supplies Low Poly/Assets/Prefabs/Pencil yellow") as GameObject;

            // Random forces
            float forceAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
            Vector3 forceDir = new Vector3(Mathf.Sin(forceAngle), 0, Mathf.Cos(forceAngle));

            // messy
            ScoreParams penScoreParams = new ScoreParams(wallScore: 0, groupScore: 0, spread: 0.0f, messiness: 8.0f);
            GameObject obj = objSpawner.SpawnObj(resource, new Vector3(8.0f, 1.0f, 1.0f), new Vector3(0.0f, -90.0f, 0.0f), penScoreParams, dropFactor: 0.1f);
            obj.GetComponent<Rigidbody>().AddForce(forceDir * Random.Range(-2.0f, 2.0f), ForceMode.Impulse);
            obj.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(-5.0f, 5.0f), 0));

            // neat 
            //ScoreParams penScoreParams = new ScoreParams(wallScore: 0, groupScore: 1, spread: 0.0f, messiness: 0.0f);
            //GameObject obj = objSpawner.SpawnObj(resource, new Vector3(8.0f, 1.0f, 1.0f), new Vector3(0.0f, -90.0f, 0.0f), penScoreParams, dropFactor: 0.3f);
            //obj.GetComponent<Rigidbody>().AddForce(forceDir * Random.Range(-0.5f, 0.5f), ForceMode.Impulse);
            //obj.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(-0.5f, 0.5f), 0));
        }

        // Spawn items that should only be present once on the desk
        GameObject eraser = Resources.Load("Office Supplies Low Poly/Assets/Prefabs/Eraser") as GameObject;
        ScoreParams eraserScoreParams = new ScoreParams(wallScore: -1, groupScore: -1, spread: 0.0f, messiness: 0.0f);
        objSpawner.SpawnObj(eraser, new Vector3(1.0f, 1.0f, 2.0f), new Vector3(0.0f, 0.0f, 0.0f), eraserScoreParams, dropFactor: 0.1f);

        GameObject mug = Resources.Load("Mugs/sample Scene/Prefabs/mug02") as GameObject;
        ScoreParams mugScoreParams = new ScoreParams(wallScore: -2, groupScore: -1, spread: 10.0f, messiness: 0.0f);
        objSpawner.SpawnObj(mug, new Vector3(4.0f, 4.0f, 4.0f), new Vector3(0.0f, 0.0f, 0.0f), mugScoreParams, dropFactor: 0.01f);
    }
}
