using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    // How much to divide each unity meter tile into
    // Ex: 2.0f = 1 unity square meter will be 4 tiles.
    public int xDim = 8; // The # of unity units from the controller to the x edge of the room
    public int zDim = 8; // The # of unity units from the controller to the z edge of the room
    public int gridFactor = 2;
    public int height;

    protected GameObject objController;
    protected ObjSpawner objSpawner;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        objController = new GameObject();
        objController.transform.position = transform.position;
        objController.AddComponent<ObjSpawner>();
        objSpawner = objController.GetComponent<ObjSpawner>();
        objSpawner.init(xDim, zDim, gridFactor);
    }
}
