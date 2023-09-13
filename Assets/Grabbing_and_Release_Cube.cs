using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR.Input;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class Grabbing_and_Release_Cube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class CubeGrabber : MonoBehaviour
{
    private InputDevice picoController_right;
    private bool buttonPressed = false;

    private void Start()
    {
        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevices(inputDevices);
        foreach (var device in inputDevices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                picoController_right = device;
                break;
            }
        }
    }

    private void Update()
    {
        if(picoController_right == null)
        {
            Debug.LogWarning("Right controller not found!!!");
            return;
        }

        if(picoController_right.TryGetFeatureValue(CommonUsages.primaryButton, out buttonPressed) && buttonPressed)
        {
            Debug.Log("Button A is pressed");
        }
        else
        {
            Debug.Log("Button A is NOT pressed");
        }
    }
}