using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// When a player holds the door and walks too far away, the physics breaks. 
/// This script lets go og the grip once a
/// player walk a certain distance from the door.
/// </summary>

[RequireComponent(typeof(XRGrabInteractable))]
public class DoorGrabDistanceChecker : MonoBehaviour
{
    public float maxGrabDistance = 2.0f; // Maximum distance before releasing the grab

    XRGrabInteractable grabInteractable; // The XRGrabInteractable component attached to the door
    private IXRSelectInteractor currentInteractor; // To store the interactor grabbing the door
    private XRInteractionManager interactionManager; 

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        interactionManager = FindObjectOfType<XRInteractionManager>();
        if (interactionManager == null)
        {
            Debug.LogError("No XRInteractionManager found in the scene.");
        }
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void Update()
    {
        if (currentInteractor != null)
        {
            Transform interactorTransform = currentInteractor.transform;
            float distance = Vector3.Distance(interactorTransform.position, grabInteractable.transform.position);
            if (distance > maxGrabDistance)
            {
                // If the distance exceeds the threshold, force the hand to release the door
                interactionManager.SelectExit(currentInteractor, grabInteractable);
                ManuallyHideAllOutlines();
            }
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // Store the interactor that's grabbing the door
        currentInteractor = args.interactorObject;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // Clear the current interactor when the door is released
        currentInteractor = null;
    }

    private void ManuallyHideAllOutlines()
    {
        // Check if the main object itself has an Outline component
        Outline mainOutline = grabInteractable.GetComponent<Outline>();
        if (mainOutline != null)
        {
            mainOutline.HideOutline();
        }

        // Check all the colliders associated with the grab interactable
        var colliders = grabInteractable.colliders;
        foreach (Collider collider in colliders)
        {
            Outline outline = collider.GetComponent<Outline>();

            if (outline != null)
            {
                outline.HideOutline();
            }
        }
    }
}
