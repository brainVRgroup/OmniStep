using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableRespawner : MonoBehaviour
{
    private UnityEngine.Pose respawnPoint;

    private bool isCollidingWithFloor = false;
    public float requiredCollisionTime = 5f;
    private float collisionTime = 0f;

    private void OnEnable()
    {
        respawnPoint = transform.GetWorldPose();
    }

    void OnCollisionEnter(Collision collision)
           {
        // Check if the colliding object has the "Floor" tag
        if (collision.gameObject.CompareTag("Floor"))
        {
            isCollidingWithFloor = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Check if the colliding object has the "Floor" tag
        if (collision.gameObject.CompareTag("Floor"))
        {
            isCollidingWithFloor = false;
        }
    }

    public void Respawn()
    {
        transform.SetWorldPose(respawnPoint);

    }
}
