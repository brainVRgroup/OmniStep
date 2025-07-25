using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

[RequireComponent(typeof(XRRayInteractor))]
[RequireComponent (typeof(XRInteractorLineVisual))]
public class XRIntractorLineVisualToggle : MonoBehaviour
{
    private XRRayInteractor rayInteractor;
    private XRInteractorLineVisual lineVisual;

    void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        lineVisual = GetComponent<XRInteractorLineVisual>();
    }

    void OnEnable()
    {
        rayInteractor.uiHoverEntered.AddListener(OnHoverEntered);
        rayInteractor.uiHoverExited.AddListener(OnHoverExited);
    }

    void OnDisable()
    {
        rayInteractor.uiHoverEntered.RemoveListener(OnHoverEntered);
        rayInteractor.uiHoverExited.RemoveListener(OnHoverExited);
    }

    private void OnHoverEntered(UIHoverEventArgs args)
    {
        //if (args.interactableObject.Get)
        //{
        //    lineVisual.enabled = true;
        //}

        lineVisual.enabled = true;
    }

    private void OnHoverExited(UIHoverEventArgs args)
    {
        //if (args.interactableObject is IXRUIInteractable)
        //{
        //    lineVisual.enabled = false;
        //}
        lineVisual.enabled = false;

    }
}
