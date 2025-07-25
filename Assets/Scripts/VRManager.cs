using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class VRManager : MonoBehaviour
{
    [SerializeField] ControllerPair[] controllerPrefab;
    [SerializeField] Material highlightMaterial;
    [SerializeField] VRHeadsetDetector.VRHeadset defaultHeadset;

    [SerializeField] InputActionReference gripActionLeft;
    [SerializeField] InputActionReference gripActionRight;

    [SerializeField] XRDeviceSimulator simulator;
    [SerializeField] Collider simulatorCollider;

    // Used in inspector
    public bool LeftGripPressed { get => gripActionLeft.action.WasPressedThisFrame(); }
    public bool RightGripPressed { get => gripActionRight.action.WasPressedThisFrame(); }

    public GameObject SimulatorColliderObject { get => simulatorCollider.gameObject; }

    Controller instantiatedRightHandController;
    Controller instantiatedLeftHandController;

    private void Start()
    {
        var detectedHeadset = VRHeadsetDetector.GetConnectedHeadset();
        if (detectedHeadset == VRHeadsetDetector.VRHeadset.None)
        {
            simulator.gameObject.SetActive(true);
            simulatorCollider.enabled = true;
        }
    }

    private Controller GetAppropriateController(HandOrientation handOrientation)
    {
        var detectedHeadset = VRHeadsetDetector.GetConnectedHeadset();

        if (detectedHeadset == VRHeadsetDetector.VRHeadset.None || detectedHeadset == VRHeadsetDetector.VRHeadset.Unknown)
        {
            Debug.LogWarning(detectedHeadset == VRHeadsetDetector.VRHeadset.None
                ? "No headset connected, turning on the XR Device Simulator"
                : $"Unknown headset type connected, defaulting to {defaultHeadset}");

            detectedHeadset = defaultHeadset;
        }

        var controllerPair = controllerPrefab.FirstOrDefault(pair => pair.HeadsetType == detectedHeadset);

        if (controllerPair == null)
        {
            Debug.LogError($"No matching controller pair found for headset {detectedHeadset}");
            return null;
        }

        return controllerPair.GetController(handOrientation);

    }

    public GameObject InstantiateController(HandOrientation handOrientation, Vector3 position, Quaternion rotation, Transform parent)
    {
        var controllerPrefab = GetAppropriateController(handOrientation);
        GameObject controller = Instantiate(controllerPrefab.gameObject, position, Quaternion.identity, parent);

        if (handOrientation == HandOrientation.Left)
        {
            instantiatedLeftHandController = controller.GetComponent<Controller>();
        }
        else
        {
            instantiatedRightHandController = controller.GetComponent<Controller>();
        }

        return controller;
    }

    public GameObject InstantiateController(HandOrientation handOrientation, Vector3 position, Transform parent)
    {
        return InstantiateController(handOrientation, position, Quaternion.identity, parent);
    }

    public void HighlightTrackpad()
    {
        HighlightControllerButton(controller => controller.HighlightTrackpad(highlightMaterial));
    }

    public void UnhighlightTrackpad()
    {
        HighlightControllerButton(controller => controller.UnhighlightTrackpad());
    }

    public void HighlightGripButton()
    {
        HighlightControllerButton(controller => controller.HighlightGripButton(highlightMaterial));
    }

    public void UnhighlightGripButton()
    {
        HighlightControllerButton(controller => controller.UnhighlightGripButton());
    }

    private void HighlightControllerButton(Action<Controller> highlightAction)
    {
        if (instantiatedLeftHandController != null)
        {
            highlightAction(instantiatedLeftHandController);
        }
        else
        {
            Debug.LogWarning("Left controller is not registered");
        }

        if (instantiatedRightHandController != null)
        {
            highlightAction(instantiatedRightHandController);
        }
        else
        {
            Debug.LogWarning("Right controller is not registered");
        }

    }

}
