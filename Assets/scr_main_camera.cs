using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.XR;
//using Varjo.XR;

/*
public class scr_main_camera : MonoBehaviour
{
    public LineRenderer rayRender_Left; // Assign your Line Renderer in the Inspector.
    public LineRenderer rayRender_Right;

    public GameObject ancherObject_left;
    public GameObject ancherObject_right;

    private Transform ancherObject_left_transform;
    private Transform ancherObject_right_transform;

    public Vector3 Offset = new Vector3(x: 1, y: 0, z:0);
    public bool isHidden = true;

    // Start is called before the first frame update
    void Start()
    {
        ancherObject_left_transform = ancherObject_left.transform;
        ancherObject_right_transform = ancherObject_right.transform;
        VarjoEyeTracking.GazeOutputFilterType GazeFilter = VarjoEyeTracking.GetGazeOutputFilterType();
        VarjoEyeTracking.GazeOutputFrequency GazeFrequenzy = VarjoEyeTracking.GetGazeOutputFrequency();

        

        //Debug.Log("GazeFilter: " + GazeFilter + " GazeFrequenzy: " + GazeFrequenzy);
    }

    // Update is called once per frame
    void Update()
    {
        if(isHidden)
        {
            ancherObject_left.SetActive(false);
            ancherObject_right.SetActive(false);
        }
        else
        {
            ancherObject_left.SetActive(true);
            ancherObject_right.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        // Get gaze data
        VarjoEyeTracking.GazeData recordedGazeData = VarjoEyeTracking.GetGaze();

        //Debug.Log(recordedGazeData.left.forward);

        VarjoEyeTracking.EyeMeasurements data2 = VarjoEyeTracking.GetEyeMeasurements();
        //Debug.Log(data2.leftEyeOpenness);

        // Create a ray from the camera's position and gaze direction
        // Ray gazeRay = new Ray(ancherObject.position, ancherObject.forward);
        Vector3 left_anchor = ancherObject_left_transform.position;
        Vector3 right_anchor = ancherObject_right_transform.position;
        Vector3 direction_left = ancherObject_left_transform.transform.TransformDirection(recordedGazeData.left.forward);
        Vector3 direction_right = ancherObject_right_transform.transform.TransformDirection(recordedGazeData.right.forward);

        Ray GazeRay_Left = new Ray(left_anchor, direction_left);
        Ray GazeRay_Right = new Ray(right_anchor, direction_right);

        if ( (recordedGazeData.leftStatus == VarjoEyeTracking.GazeEyeStatus.Invalid) && (recordedGazeData.rightStatus == VarjoEyeTracking.GazeEyeStatus.Invalid))
        {
            GazeRay_Left = new Ray(left_anchor, ancherObject_left_transform.forward);
            GazeRay_Right = new Ray(right_anchor, ancherObject_right_transform.forward);
        }

        

       
        // Define a maximum ray length (you can adjust this)
        float maxRayLength = 20f;

        // Perform raycasting
        RaycastHit hitInfo_left, hitInfo_right;
        if (Physics.Raycast(GazeRay_Left, out hitInfo_left, maxRayLength))
        {
            // If the ray hits something, update the Line Renderer
            rayRender_Left.SetPosition(0, left_anchor); // Start position
            rayRender_Left.SetPosition(1, hitInfo_left.point); // End position (where the ray hits)
        }
        else
        {
            // If the ray doesn't hit anything, extend it to the maximum length
            rayRender_Left.SetPosition(0, left_anchor);
            rayRender_Left.SetPosition(1, left_anchor + GazeRay_Left.direction * maxRayLength);
        }

        if(Physics.Raycast(GazeRay_Right, out hitInfo_right, maxRayLength))
        {
            // If the ray hits something, update the Line Renderer
            rayRender_Right.SetPosition(0, right_anchor); // Start position
            rayRender_Right.SetPosition(1, hitInfo_right.point); // End position (where the ray hits)
        }
        else
        {
            // If the ray doesn't hit anything, extend it to the maximum length
            rayRender_Right.SetPosition(0, right_anchor);
            rayRender_Right.SetPosition(1, right_anchor + GazeRay_Right.direction * maxRayLength);
        }

    }
}
*/

public class scr_main_camera : MonoBehaviour
{
    void Start()
    {
        Debug.Log("The Script scr_main_camera.cs is currently deactivated!!!");
    }
}