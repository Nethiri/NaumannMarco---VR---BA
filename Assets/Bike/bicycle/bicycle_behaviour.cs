using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class bicycle_behaviour : MonoBehaviour
{
    //work in progress bycicle movement behaviour
    Rigidbody rb;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpForce = 2.5f;
    [SerializeField] float rotationDegPerSecond = 90f;

    void Start()
    {
        Debug.Log("Bike movement inits");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("Player has been updated: " + test_number.ToString() + " times.");
        //test_number++;
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(x: rb.velocity.x, y: jumpForce, z: rb.velocity.z);
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

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
}
