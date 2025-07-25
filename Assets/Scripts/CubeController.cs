using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(InteractableRespawner))]
[RequireComponent (typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRGrabInteractable))]
public class CubeController : MonoBehaviour
{
    public UnityEvent OnPickUp;
    public UnityEvent OnLandingAfterThrow;

    InteractableRespawner respawner;
    Rigidbody rb;
    Collider cubeCollider;
    XRGrabInteractable grabInteractable;

    bool isPickedUp = false;
    bool hasLanded = false;
    bool isThrown = false;

    private void OnEnable()
    {
        respawner = GetComponent<InteractableRespawner>();
        rb = GetComponent<Rigidbody>();
        cubeCollider = GetComponent<Collider>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Register for pickup and drop events
        grabInteractable.selectEntered.AddListener(OnCubePickedUp);
        grabInteractable.selectExited.AddListener(OnCubeDropped);
    }

    private void OnDestroy()
    {
        // Unregister event listeners
        grabInteractable.selectEntered.RemoveListener(OnCubePickedUp);
        grabInteractable.selectExited.RemoveListener(OnCubeDropped);
    }

    private void OnCubePickedUp(SelectEnterEventArgs args)
    {
        isPickedUp = true;
        isThrown = false; // Reset thrown state when picked up
        hasLanded = false; // Reset landed state when picked up

        Debug.Log("Cube picked up!");
        OnPickUp?.Invoke(); // Trigger the event for pickup
    }

    // When the cube is dropped
    private void OnCubeDropped(SelectExitEventArgs args)
    {
        isPickedUp = false;
        isThrown = true;
        hasLanded = false;
        Debug.Log("Cube dropped!");
    }

    public void RespawnCube()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isPickedUp = false;
        isThrown = false;
        hasLanded = false;
        respawner.Respawn();

    }

    // Detect when the cube actually lands on a surface
    private void OnCollisionEnter(Collision collision)
    {
        // Ensure that the cube was thrown and not picked up
        if (isThrown && !isPickedUp && !hasLanded)
        {
            hasLanded = true;
            StartCoroutine(WaitUntilStopped());
        }
    }

    private IEnumerator WaitUntilStopped()
    {
        float timeout = 3f; 
        float elapsedTime = 0f; 

        // Wait until the velocity of the rigidbody is below a threshold or the timeout is reached
        while (rb.velocity.magnitude > 0.1f) // 0.1f is the threshold to consider the object as "stopped"
        {
            elapsedTime += Time.deltaTime; 

            // Check if the timeout has been reached
            if (elapsedTime >= timeout)
            {
                Debug.LogWarning("Timeout reached: Cube did not stop moving within the expected time.");
                break;
            }

            yield return null; 
        }

        // If the loop ended due to timeout, log a warning, otherwise confirm that the cube stopped
        if (elapsedTime < timeout)
        {
            Debug.Log("Cube has landed after being thrown!");

            OnLandingAfterThrow?.Invoke();
        }
    }


}
