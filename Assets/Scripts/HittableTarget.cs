using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class HittableTarget : MonoBehaviour
{
    // This function is called when the GameObject collides with another Collider
    public UnityEvent OnTargetHit;

    public bool wasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        // If it does, deactivate this target GameObject
        if (other.gameObject.TryGetComponent<XRGrabInteractable>(out _))
        {
            //gameObject.SetActive(false);
            wasHit = true;
            OnTargetHit?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If it does, deactivate this target GameObject
        if (collision.gameObject.TryGetComponent<XRGrabInteractable>(out _))
        {
            //gameObject.SetActive(false);
            wasHit = true;
            OnTargetHit?.Invoke();
        }
    }


}