using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PosableHandBase;

[CreateAssetMenu(fileName = "ControllerPair", menuName = "ControllerPair")]
public class ControllerPair : ScriptableObject
{
    public VRHeadsetDetector.VRHeadset HeadsetType;

    [SerializeField]
    private Controller rightController;

    [SerializeField]
    private Controller leftController;

    public Controller GetController(HandOrientation orientation)
    {
        if (orientation == HandOrientation.Left)
        {
            return leftController;
        }
        else
        {
            return rightController;
        }
    }
}
