using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pose
{
    public Vector3 attachPosition = Vector3.zero;
    public Quaternion attachRotation = Quaternion.identity;
    public List<Quaternion> fingerRotations = new();

    public static Pose Empty => new();

    public void Save(PosableHandBase hand)
    {
        attachPosition = hand.transform.parent.TransformVector(hand.transform.localPosition);
        attachRotation = hand.transform.parent.rotation * hand.transform.localRotation;

        fingerRotations = hand.GetFingerJointRotations();
    }

    public static Pose ExtractHandPose(PosableHandBase hand)
    {
        Pose pose = new();
        pose.Save(hand);

        Debug.Log(pose.fingerRotations.Count);

        return pose;
    }

}
