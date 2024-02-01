using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class HUD_script : MonoBehaviour
{
    public UnityEngine.UI.Text speedText;
    public Rigidbody Bike;
    public Request_Server_script serverData;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string OutPut = $"Speed: {serverData.Request_tacx_speed.ToString("F2")}\nSteering Angle:{serverData.Request_elite_angle.ToString("F2")}\r\n";
        speedText.text = OutPut;
    }
}
