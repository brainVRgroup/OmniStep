using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRDeviceSimulator;

[SelectionBase]
[ExecuteInEditMode]
public class PosableHandPreview: PosableHandBase
{
    public PosableHandPreview MirrorHandPose(PosableHandPreview targetHand)
    {
     
        targetHand.SetMirrorJointRotations(FingerJoints);
        targetHand.transform.SetLocalPositionAndRotation(MirrorPosition(transform), MirrorRotation(transform));

        return targetHand;
    }

    public PosableHandPreview MirrorJointPoseOnly(PosableHandPreview targetHand)
    {
        targetHand.SetMirrorJointRotations(FingerJoints);

        return targetHand;
    }

    private void SetMirrorJointRotations(List<Transform> joints)
    {
        List<Quaternion> mirroredJoints = new();

        foreach (Transform joint in joints)
        {
            Quaternion inversedRotation = MirrorJoint(joint);
            mirroredJoints.Add(inversedRotation);
        }

        SetFingerRotations(mirroredJoints);
    }

    private Quaternion MirrorJoint(Transform sourceTransform)
    {
        Quaternion mirrorRotation = sourceTransform.localRotation;
        mirrorRotation.x *= 1.0f;

        return mirrorRotation;
    }

    private Quaternion MirrorRotation(Transform sourceTransform)
    {
        Quaternion originalRotation = sourceTransform.localRotation;
        return new Quaternion(-originalRotation.x, originalRotation.y, -originalRotation.z, originalRotation.w); ;
    }

    private Vector3 MirrorPosition(Transform sourceTransform)
    {
        Vector3 mirroredPosition = sourceTransform.localPosition;
        mirroredPosition.x *= 1.0f;
        return mirroredPosition;
    }

    public override void ApplyOffset(Vector3 position, Quaternion rotation)
    {
        transform.localPosition = transform.parent.InverseTransformVector(position);
        transform.localRotation = Quaternion.Inverse(transform.parent.rotation) * rotation;
    }
}
