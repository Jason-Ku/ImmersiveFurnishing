using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PhysicsSimulator : MonoBehaviour
{
    public int maxIterations = 500;
    bool simulationRan = false;

    void Start()
    {
        Physics.autoSimulation = false;
    }

    void Update()
    {
        if (!simulationRan)
            RunSimulation();
    }

    public void RunSimulation()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            Rigidbody[] simulatedBodies = FindObjectsOfType<Rigidbody>();

            foreach (Rigidbody body in simulatedBodies)
            {
                // Code here for applying force/torque to ALL rigidbodies
                //float randomForceAmount = Random.Range(-2.0f, 2.0f);
                //float forceAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
                //Vector3 forceDir = new Vector3(Mathf.Sin(forceAngle), 0, Mathf.Cos(forceAngle));

                //body.AddForce(forceDir * randomForceAmount, ForceMode.Impulse);
                //body.AddTorque(new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f)));
            }

            for (int i = 0; i < maxIterations; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                if (simulatedBodies.All(rb => rb.IsSleeping()))
                    break;
            }

            Physics.autoSimulation = true;
            simulationRan = true;
        }
    }
}
