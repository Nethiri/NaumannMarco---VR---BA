using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class BH_MyBike : MonoBehaviour
{
    public bool readRequestServer = false;
    public Request_Server_script serverData;

    //bike objects
    public WheelCollider[] wheel_col_front;
    public WheelCollider[] wheel_col_back;
    public GameObject[] FrontWheelObject;
    public GameObject[] BackWheelObject;
    public GameObject[] HandlebarObject;
    //settings - modifyable from the outside
    public Vector3 centerOfMass;
    [Range(0f, 90f)]
    public float SteerAngle = 45;
    [Range(-100f, 100f)]
    public float TargetMpSSpeed = 10f;

    private Rigidbody rb;
    public int Break_dead_zone = 30;

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
        if (rb != null)
        {
            rb.centerOfMass = centerOfMass;
        }

        if (readRequestServer == true) { MoveWithServerData(); }
        else { MoveWithVelocityANDwheels(); }

        Debug.Log($"Velocity: {rb.velocity.magnitude.ToString("F2")} m/s" );
    }

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

    private float elapsedTime = 0f;
    private float updateInterval = 0.25f; // 0.25 second

    void MoveWithServerData()
    { 
        
        if(serverData.Request_tacx_elapsed_time != 0)
        {   
            //calc velocity in driving direction
            float xVal = Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * serverData.Request_tacx_speed;
            float zVal = Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * serverData.Request_tacx_speed;
            float yVal = 0; // we want to just have speed in the plane
            //apply the vertical speed to the target (acceleration/deceleration handled by tacx)
            rb.velocity = new Vector3(xVal, yVal, zVal);
        }

        
        if(serverData.Request_elite_last_update != 0)
        {
            //lets handle turning through the front wheel collider
            foreach (WheelCollider frontWheel in wheel_col_front)
            {
                frontWheel.steerAngle = serverData.Request_elite_angle;
            }
        }

        if(serverData.Request_break_least_update != 0)
        {
            if(serverData.Request_break_back > Break_dead_zone) 
            {
                //// Check if one second has passed
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= updateInterval)
                {
                    if (serverData.Request_break_back > Break_dead_zone && serverData.Request_break_back < 512) { serverData.SetResistance(50); }
                    else if (serverData.Request_break_back >= 512 && serverData.Request_break_back < 800) { serverData.SetResistance(100); }
                    else { serverData.SetResistance(150); }
                }
                elapsedTime = 0f;
            }
        }

    }
}