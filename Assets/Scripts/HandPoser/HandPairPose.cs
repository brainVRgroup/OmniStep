using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPairPose : ScriptableObject
{
    public string PoseName = "New pose";

    [SerializeField] Pose leftHandPose;
    [SerializeField] Pose rightHandPose;

    public override string ToString()
    {
        return PoseName;
    }

    public Pose GetHandPose(HandOrientation orientation)
    {
        if (orientation == HandOrientation.Left)
        {
            return leftHandPose;
        }

        return rightHandPose;
    }

    public void SetHandPose(Pose pose, HandOrientation orientation)
    {
        if (orientation == HandOrientation.Left)
        {
            leftHandPose = pose;
        }
        else
        {
            rightHandPose = pose;
        }

        
    }
}
