using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class bicycle_behaviour : MonoBehaviour
{
    //work in progress bycicle movement behaviour
    Rigidbody rb;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpForce = 2.5f;
    [SerializeField] float rotationDegPerSecond = 90f;
     
    //webhook for bycicle controals
    [SerializeField] string serverAddress = "http://localhost:8057/";
    [Serializable] public class BikeData
    {
        public float last_update;
        public float elapsed_time;
        public float distance_travelled;
        public float speed;
        public int basic_resistance;
    }

    BikeData DATAPACKAGE;
    float curMaxSpeed = 0;


    bool canRequestData = true; // Controls if we can make a request to the server
    float requestDataInterval = 0.5f; // Interval to request data from the server (every 0.5 seconds)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(GetBikeDataPeriodically());
        DATAPACKAGE = new BikeData();
        DATAPACKAGE.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump")) //dumb test 
        {
            //rb.velocity = new Vector3(x: rb.velocity.x, y: jumpForce, z: rb.velocity.z);
            int newVal = DATAPACKAGE.basic_resistance + 10;
            if(newVal > 200) { newVal = 0; }
            StartCoroutine(SendResistanceToServer(newVal));
        }
        float horizontalInput = Input.GetAxis("Horizontal") ; //up/down
        float verticalInput = Input.GetAxis("Vertical") + DATAPACKAGE.speed;

        float xVal = Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalInput * movementSpeed + Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * horizontalInput * movementSpeed;
        float yVal = Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalInput * movementSpeed - Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * horizontalInput * movementSpeed;


        rb.velocity = new Vector3(
            x: xVal,
            y: rb.velocity.y,
            z: yVal
            );

        //horizontalInpout * movementSpeed * Mathf.Sin(rb.rotation.y)
        if (Input.GetKey(KeyCode.Q)) { rb.rotation *= Quaternion.Euler(0f, -(rotationDegPerSecond * Time.deltaTime), 0f); }
        if (Input.GetKey(KeyCode.E)) { rb.rotation *= Quaternion.Euler(0f, rotationDegPerSecond * Time.deltaTime, 0f); }

        
    }

    void printDebugData()
    {
        if (DATAPACKAGE.speed > curMaxSpeed)
        {
            curMaxSpeed = DATAPACKAGE.speed;
        }
        Debug.Log("Current speed = " + DATAPACKAGE.speed.ToString() +
            ", Current Max Speed = " + curMaxSpeed.ToString() +
            ", Reistance = " + DATAPACKAGE.basic_resistance.ToString());
    }

    IEnumerator GetBikeDataPeriodically()
    {
        while (true)
        {
            if (canRequestData)
            {
                // Make a request to the server
                StartCoroutine(GetBikeDataFromServer());
            }
            // Wait for the desired interval before making the next request
            yield return new WaitForSeconds(requestDataInterval);
        }
    }


    IEnumerator GetBikeDataFromServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(serverAddress))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                // Parse the received JSON string into a BikeData object
                BikeData bikeData = JsonUtility.FromJson<BikeData>(www.downloadHandler.text);
                DATAPACKAGE = bikeData;

                printDebugData();
            }
            else
            {
                Debug.LogError("Error " + www.error);
            }
        }
    }

    IEnumerator SendResistanceToServer(int resistanceValue)
    {
        string url = serverAddress + "set_resistance";

        // Create a JSON object with the resistance value
        string jsonData = "{\"resistance\":" + resistanceValue.ToString() + "}";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Resistance set to " + resistanceValue);
            }
            else
            {
                Debug.LogError("Error sending resistance: " + www.error);
            }
        }
    }


}
