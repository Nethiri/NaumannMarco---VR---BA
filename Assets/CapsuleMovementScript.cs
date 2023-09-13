using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleMovementScript : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float Movement_Speed = 10f;
    [SerializeField] float Jump_Force = 2.5f;
    [SerializeField] float rotationDegreePerSecond = 90f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player initialisation has been done!");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(x: rb.velocity.x, y: Jump_Force, z: rb.velocity.z);
        }

        float horizontalImput = Input.GetAxis("Horizontal");
        float verticalImput = Input.GetAxis("Vertical");

        Debug.Log("Hor: " + horizontalImput + " Vert: " + verticalImput);

        float xVal = Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalImput * Movement_Speed + Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * horizontalImput * Movement_Speed;
        float yVal = Mathf.Cos((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * verticalImput * Movement_Speed 
            - Mathf.Sin((rb.rotation.eulerAngles.y * Mathf.PI) / 180) * horizontalImput * Movement_Speed;
        Debug.Log(yVal);

        rb.velocity = new Vector3(
            x: xVal,
            y: rb.velocity.y,
            z: yVal
        );
        Debug.Log(rb.velocity);

        if (Input.GetKey(KeyCode.Q)) { rb.rotation *= Quaternion.Euler(0f, -(rotationDegreePerSecond * Time.deltaTime), 0f);  }
        if(Input.GetKey(KeyCode.E)) { rb.rotation *= Quaternion.Euler(0f, rotationDegreePerSecond * Time.deltaTime, 0f); }
    }
}
