using UnityEngine;
using UnityEngine.UI;

public class XRDeviceSimulatorConfig : MonoBehaviour
{
    [SerializeField] float startHeight = 1.5f;
    [SerializeField] Transform cameraOffset;
    [SerializeField] Button startButton;

    private void Start()
    {
        cameraOffset.position = new Vector3(cameraOffset.position.x, startHeight, cameraOffset.position.z);
        startButton.interactable = true;
    }
}
