using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> interactableObjects = new();

    public UnityEvent OnCheckpointReached;

    private void Start()
    {
        // If checkpoint is for the player (aka the XROrigin), add the simulator collider to the interactable objects 
        // so the funtionality works in the simulator as well
        var xrOriginPresent = interactableObjects.Any(obj => obj != null && obj.GetComponent<XROrigin>() != null);
        if (xrOriginPresent)
        {
            VRManager vrController = FindObjectOfType<VRManager>();

            interactableObjects.Add(vrController.SimulatorColliderObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsObjectInCheckpoint(other.gameObject))
        {
            Debug.Log("Trigger entered by " + other.name);
            StartCoroutine(CheckIfObjectReleased(other.gameObject));
        }
    }

    private IEnumerator CheckIfObjectReleased(GameObject obj)
    {
        XRGrabInteractable grabInteractable = obj.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // Wait until the object is no longer being held
            while (grabInteractable.isSelected)
            {
                yield return null;
            }
        }

        if (IsObjectInCheckpoint(obj))
        {
            Debug.Log("Object placed in checkpoint: " + obj.name);
            yield return new WaitForSeconds(1);
            gameObject.SetActive(false);
            OnCheckpointReached.Invoke();
        }
    }


    private bool IsObjectInCheckpoint(GameObject obj)
    {
        return interactableObjects.Contains(obj);
    }

    public void AddInteractableObject(GameObject obj)
    {
        if (!interactableObjects.Contains(obj))
        {
            interactableObjects.Add(obj);
        }
    }

    public void SkipCheckPoint()
    {
        OnCheckpointReached.Invoke();
        gameObject.SetActive(false);
    }
}
 