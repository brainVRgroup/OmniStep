using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class RecenterOrigin : MonoBehaviour
{
    public Transform start;


    void Start()
    {
        start = transform;
    }

    public void Recenter()
    {
        XROrigin xrOrigin = GetComponent<XROrigin>();
        xrOrigin.MoveCameraToWorldLocation(start.position);
        xrOrigin.MatchOriginUpCameraForward(start.up, start.forward);
    }
}
