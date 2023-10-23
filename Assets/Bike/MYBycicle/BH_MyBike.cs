using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BH_MyBike : MonoBehaviour
{
    public WheelCollider[] wheel_col;
    public Transform[] wheel_mesh;
    public MeshCollider[] handlebar_col;
    public Transform[] handlebar_mesh;

    public Vector3 centerOfMass;


    public float torque = 2000;
    float angle = 45;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.centerOfMass = centerOfMass;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Apply motor torque to the back wheel (index 0)
        wheel_col[0].motorTorque = Input.GetAxis("Vertical") * torque;

        // Handle front wheel (index 1) rotation
        wheel_col[1].steerAngle = Input.GetAxis("Horizontal") * angle;

        // Update the position and rotation of the wheels
        for (int i = 0; i < wheel_col.Length; i++)
        {
            var pos = transform.position;
            var rot = transform.rotation;
            wheel_col[i].GetWorldPose(out pos, out rot);
            wheel_mesh[i].position = pos;
            wheel_mesh[i].rotation = rot;
        }

       
    }
}