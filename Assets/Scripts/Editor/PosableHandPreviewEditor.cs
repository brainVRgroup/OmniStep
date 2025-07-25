using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PosableHandPreview))]
public class PosableHandPreviewEditor : Editor
{
    private PosableHandPreview handPreview;
    private Transform selectedJoint;

    private void OnEnable()
    {
        handPreview = (PosableHandPreview)target;
    }

    private void OnSceneGUI()
    {
        DrawJointGizmos();
        DrawJointHandles();
    }

    private void DrawJointGizmos()
    {
        foreach (Transform joint in handPreview.FingerJoints)
        {
            bool pressed = Handles.Button(joint.position, joint.rotation, 0.006f, 0.008f, Handles.SphereHandleCap);

            if (pressed)
            {
                selectedJoint = joint == selectedJoint ? null : joint;
            }
        }
    }

    private void DrawJointHandles()
    {
        if (selectedJoint)
        {
            Quaternion currentRotation = selectedJoint.rotation;
            Quaternion newRotation = Handles.RotationHandle(currentRotation, selectedJoint.position);

            if (currentRotation != newRotation)
            {
                Undo.RecordObject(selectedJoint.transform, "Joint Rotated");
                selectedJoint.rotation = newRotation;
                
                PrefabUtility.RecordPrefabInstancePropertyModifications(selectedJoint.transform);
            }
        }
    }
}
