using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RobotAnimationEvents : MonoBehaviour
{
    [SerializeField] Transform attachPoint;
    [SerializeField] GameObject pickUpObject;

    Rigidbody cubeRigidBody;
    XRGrabInteractable cubeGrabInteractable; 

    private void OnEnable()
    {
        cubeRigidBody = pickUpObject.GetComponent<Rigidbody>();
        cubeGrabInteractable = pickUpObject.GetComponent<XRGrabInteractable>();
    }

    public void PickOrDropCube()
    {
        if (pickUpObject.transform.parent == attachPoint.transform)
        {
            if (cubeRigidBody != null)
            {
                cubeRigidBody.isKinematic = false; // Ensure physics are applied
            }

            pickUpObject.transform.SetParent(null);

            if (cubeGrabInteractable != null)
            {
                cubeGrabInteractable.enabled = true;
            }
        }
        else
        {
            pickUpObject.transform.SetParent(attachPoint.transform);
            pickUpObject.transform.localPosition = Vector3.zero; // Adjust if necessary
            pickUpObject.transform.localRotation = Quaternion.identity;

            if (cubeRigidBody != null)
            {
                cubeRigidBody.isKinematic = true; // Ensure physics are not applied
            }

            if (cubeGrabInteractable != null)
            {
                cubeGrabInteractable.enabled = false;
            }

        }
    }
}
