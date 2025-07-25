using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerController : MonoBehaviour
{
    public UnityEvent TriggerEntered;

    private void OnTriggerEnter(Collider other)
    {
        // Invoke the event, passing the player GameObject as a parameter
        TriggerEntered?.Invoke();
    }
}
