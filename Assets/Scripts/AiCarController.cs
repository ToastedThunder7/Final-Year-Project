using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class AiCarController : MonoBehaviour
{
    private Rigidbody rb;

    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    public Transform[] waypoints;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateWheelPos();
    }

    void UpdateWheelPos()
    {
        // updates wheel meshes to colliders 
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }
    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        // declares local variables
        Quaternion quat;
        Vector3 position;
        //gets position and rotation of object
        coll.GetWorldPose(out position, out quat);
        //set wheel position and rotation
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }
}