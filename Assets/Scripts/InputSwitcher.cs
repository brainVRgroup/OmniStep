using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputSwitcher : MonoBehaviour
{
    void Start()
    {
        CheckVRDeviceConnected();
    }

    void CheckVRDeviceConnected()
    {
        if (XRSettings.isDeviceActive)
        {
            Debug.Log("VR device is connected: " + XRSettings.loadedDeviceName);
        }
        else
        {
            Debug.Log("No VR device connected.");
        }
    }
}
