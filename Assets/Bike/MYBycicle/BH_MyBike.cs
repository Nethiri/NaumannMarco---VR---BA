using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    //public float torque = 2000;
    [Range(0f, 90f)]
    public float SteerAngle = 45;
    [Range(-100f, 100f)]
    public float TargetMpSSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Use Byscile script BH_MyBikes.cs");
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.centerOfMass = centerOfMass;
        }
    }

    // Update is called once per frame
    void Update()
    {
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

        //MoveWithViolocityModifier();
        //MoveWithWheelCollider();
        MoveWithVelocityANDwheels();
        Debug.Log($"Velocity: {rb.velocity.magnitude.ToString("F2")} m/s" );
    }

    //float targetSpeedMPS = 8.33333f;
    //public float multiplyer = 1f;
    //public bool bothwheels = false;
    //public float accelerationForce = 10f;
    ////void MoveWithWheelCollider()
    ////{
    ////    // Apply motor torque to the back wheel
    ////    List<WheelCollider> wheels = new List<WheelCollider>();
    ////    float verticalInput = Input.GetAxis("Vertical");

    ////    wheels.AddRange(wheel_col_back);
    ////    if (bothwheels) { wheels.AddRange(wheel_col_front); }

    ////    foreach (WheelCollider wheel in wheels)
    ////    {

    ////        float torque = Input.GetAxis("Vertical") * wheel.radius * targetSpeedMPS * wheel.mass * multiplyer;
    ////        Debug.Log(Input.GetAxis("Vertical"));
    ////        Debug.Log("Torque" + torque.ToString("F2"));
    ////        wheel.motorTorque = torque;
    ////    }
    ////    //Handle front wheel rotation

    ////    foreach (WheelCollider wheel in wheel_col_front)
    ////    {
    ////        wheel.steerAngle = Input.GetAxis("Horizontal") * angle;
    ////    }
    ////}

    //float movementSpeed = 5f;
    //float rotationDegPerSecond = 90f;

    //void MoveWithViolocityModifier()
    //{
    //    float horizontalInput = 0; // Input.GetAxis("Horizontal"); //left/right
    //    float verticalInput = Input.GetAxis("Vertical");

    //    float xVal = Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalInput * movementSpeed + Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * horizontalInput * movementSpeed;
    //    float yVal = Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalInput * movementSpeed - Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * horizontalInput * movementSpeed;

    //    rb.velocity = new Vector3(
    //        x: xVal,
    //        y: 0,
    //        z: yVal
    //        );

    //    //Vector3 acceleration = new Vector3(x: xVal,y: 0,z: yVal) * verticalInput * accelerationForce;
    //    //rb.AddForce(acceleration);

    //    //horizontalInpout * movementSpeed * Mathf.Sin(rb.rotation.y)
    //    if (Input.GetKey(KeyCode.Q)) { rb.rotation *= Quaternion.Euler(0f, -(rotationDegPerSecond * Time.deltaTime), 0f); }
    //    if (Input.GetKey(KeyCode.E)) { rb.rotation *= Quaternion.Euler(0f, rotationDegPerSecond * Time.deltaTime, 0f); }
    //}



    void MoveWithVelocityANDwheels()
    {
        //this takes care of the acceleration of the bike
        float verticalInput = Input.GetAxis("Vertical");

        //calc velocity in driving direction
        float xVal = Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalInput * TargetMpSSpeed;
        float zVal = Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalInput * TargetMpSSpeed;
        float yVal = 0; // we want to just have speed in the plane
        //apply the vertical speed to the target (acceleration/deceleration handled by tacx)
        rb.velocity = new Vector3(xVal, yVal, zVal);

        //lets handle turning through the front wheel collider
        float horizontalInput = Input.GetAxis("Horizontal");
        foreach(WheelCollider frontWheel in wheel_col_front)
        {
            frontWheel.steerAngle = horizontalInput * SteerAngle;
        }


    }
}