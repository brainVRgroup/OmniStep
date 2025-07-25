using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipBillboard : MonoBehaviour
{
    Transform playerHead;  // This should be the VR headset or camera

    private void Start()
    {
        playerHead = Camera.main.transform;
    }

    private void Update()
    {
        // Make the tooltip always face the player
        transform.LookAt(playerHead);
    }
}
