using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class PosableHandBase : MonoBehaviour
{
    [SerializeField] protected GameObject[] fingers;

    public HandOrientation HandType;

    [HideInInspector]
    public List<Transform> FingerJoints { get; protected set; } = new();

    public Pose DefaultPose { get; protected set; }

    protected void Awake()
    {
        GetFingerJointPoistions();
        DefaultPose = Pose.ExtractHandPose(this);
    }

    private void GetFingerJointPoistions()
    {
        Debug.Log(fingers.Length);
        foreach (GameObject finger in fingers)
            FingerJoints.AddRange(finger.GetComponentsInChildren<Transform>());
    }

    public List<Quaternion> GetFingerJointRotations()
    {
        List<Quaternion> rotations = new();

        foreach (var joint in FingerJoints)
            rotations.Add(joint.localRotation);

        return rotations;
    }

    public void SetFingerRotations(List<Quaternion> rotations)
    {
        if (rotations.Count != FingerJoints.Count)
        {
            Debug.Log($"Rotations count {rotations.Count} while trying to set finger (count: {FingerJoints.Count}) rotations");
        }

        for (int i = 0; i < FingerJoints.Count; i++)
            FingerJoints[i].localRotation = rotations[i];

    }

    public void SetDefaultPose()
    {
        SetPose(DefaultPose);
    }

    public void SetPose(Pose pose)
    {
        Debug.Log(pose);
        SetFingerRotations(pose.fingerRotations);

        // Position, and rotate, this differs on the type of hand
        ApplyOffset(pose.attachPosition, pose.attachRotation);
    }

    public abstract void ApplyOffset(Vector3 position, Quaternion rotation);


}
