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
        
        // Make sure we spawn objects in the right spots!
        Vector3 objSize = this.GetComponent<BoxCollider>().bounds.size;
        objController.transform.position = this.transform.position + new Vector3(-objSize.x / 2.0f, objSize.y, -objSize.z / 2.0f);
        objController.AddComponent<ObjSpawner>();
        ObjSpawner objSpawner = objController.GetComponent<ObjSpawner>();   
        objSpawner.init(Mathf.RoundToInt(objSize.x) * 32, Mathf.RoundToInt(objSize.z) * 32, 40);

        // Spawn items that should only be present once on the desk
        GameObject eraser = Resources.Load("Office Supplies Low Poly/Assets/Prefabs/Eraser") as GameObject;
        objSpawner.SpawnObj(eraser, new Vector3(1.0f, 1.0f, 2.0f), new Vector3(0.0f, 0.0f, 0.0f), -1, -1, 0.0f);

        GameObject mug = Resources.Load("Mugs/sample Scene/Prefabs/mug02") as GameObject;
        objSpawner.SpawnObj(mug, new Vector3(4.0f, 4.0f, 4.0f), new Vector3(0.0f, 0.0f, 0.0f), 1, -1, 0.0f);

        // Spawn other desk items...
        for (int i = 0; i < 6; i++)
        {
            GameObject resource;
            int item = Random.Range(0, 3);

            if (item == 0)
                resource = Resources.Load("Office Supplies Low Poly/Assets/Prefabs/Pencil black") as GameObject;
            else if (item == 1)
                resource = Resources.Load("Office Supplies Low Poly/Assets/Prefabs/Pen blue") as GameObject;
            else
                resource = Resources.Load("Office Supplies Low Poly/Assets/Prefabs/Pencil black") as GameObject;

            objSpawner.SpawnObj(resource, new Vector3(8.0f, 1.0f, 1.0f), new Vector3(0.0f, -90.0f, 0.0f), 0, 1, 0.0f);
        }
    }
}
