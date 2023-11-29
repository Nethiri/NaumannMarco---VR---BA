using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BH_MyBike : MonoBehaviour
{
    public WheelCollider[] wheel_col_front;
    public WheelCollider[] wheel_col_back;
    public GameObject[] FrontWheelObject;
    public GameObject[] BackWheelObject;
    public GameObject[] HandlebarObject;

    public Vector3 centerOfMass;

    private Rigidbody rb;
        
    public float torque = 2000;
    float angle = 45;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.centerOfMass = centerOfMass;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Apply motor torque to the back wheel
        foreach(WheelCollider wheel in wheel_col_back)
        {
            wheel.motorTorque = Input.GetAxis("Vertical") * torque;
        }
        // Handle front wheel rotation
        foreach (WheelCollider wheel in wheel_col_front)
        {
            wheel.steerAngle = Input.GetAxis("Horizontal") * angle;
        }

        //Debug.Log(rb.velocity.magnitude * 3.6);

        // Update the position and rotation of the wheels
        /*for (int i = 0; i < wheel_col.Length; i++)
        {
            var pos = transform.position;
            var rot = transform.rotation;
            wheel_col[i].GetWorldPose(out pos, out rot);
            wheel_mesh[i].position = pos;
            wheel_mesh[i].rotation = rot;
        }*/

       
    }
}