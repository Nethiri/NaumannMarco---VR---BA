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
        MoveWithWheelCollider();
    }

    void MoveWithWheelCollider()
    {
        // Apply motor torque to the back wheel
        foreach (WheelCollider wheel in wheel_col_back)
        {
            wheel.motorTorque = Input.GetAxis("Vertical") * torque;
        }
        // Handle front wheel rotation
        foreach (WheelCollider wheel in wheel_col_front)
        {
            wheel.steerAngle = Input.GetAxis("Horizontal") * angle;
        }
    }

    float movementSpeed = 5f;
    float rotationDegPerSecond = 90f;

    void MoveWithViolocityModifier()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); //up/down
        float verticalInput = Input.GetAxis("Vertical");

        float xVal = Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalInput * movementSpeed + Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * horizontalInput * movementSpeed;
        float yVal = Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalInput * movementSpeed - Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * horizontalInput * movementSpeed;

        rb.velocity = new Vector3(
            x: xVal,
            y: 0,
            z: yVal
            );

        //horizontalInpout * movementSpeed * Mathf.Sin(rb.rotation.y)
        if (Input.GetKey(KeyCode.Q)) { rb.rotation *= Quaternion.Euler(0f, -(rotationDegPerSecond * Time.deltaTime), 0f); }
        if (Input.GetKey(KeyCode.E)) { rb.rotation *= Quaternion.Euler(0f, rotationDegPerSecond * Time.deltaTime, 0f); }
    }
}