using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.OpenXR.Input;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class HandPoserManager : MonoBehaviour
{
    [SerializeField] PosableHandPreview leftHandPrefab;
    [SerializeField] PosableHandPreview rightHandPrefab;

    PosableHandPreview leftHand;
    PosableHandPreview rightHand;

    public XRBaseInteractable CurretInteractable { get; private set; } = null;

    public void CreateHandPreviewInstanceIfNeeded()
    {
        if (leftHand == null || rightHand == null)
            InstantiateHands();
    }

    public (Pose Left, Pose Right) GetDefaultPose()
    {
        return (leftHand.DefaultPose, rightHand.DefaultPose);
    }

    private void InstantiateHands()
    {
        leftHand = Instantiate(leftHandPrefab, transform);
        rightHand = Instantiate(rightHandPrefab, transform);
    }


    public void MirrorHandPose(HandOrientation sourceHandOrientation)
    {
        var sourceHand = GetHand(sourceHandOrientation);
        var targetHand = GetOppositeHand(sourceHandOrientation);
        sourceHand.MirrorHandPose(targetHand);
#if UNITY_EDITOR
        Undo.RecordObject(targetHand, "Mirror Pose");
#endif
    }

    public void MirrorJointPoseOnly(HandOrientation sourceHandOrientation)
    {
        var sourceHand = GetHand(sourceHandOrientation);
        var targetHand = GetOppositeHand(sourceHandOrientation);
        sourceHand.MirrorJointPoseOnly(targetHand);
#if UNITY_EDITOR
        Undo.RecordObject(targetHand, "Mirror Pose");
#endif
    }

    // Function to get the hand prefab based on the orientation
    public PosableHandPreview GetHand
        (HandOrientation orientation)
    {
        return orientation == HandOrientation.Left ? leftHand : rightHand;
    }

    public PosableHandPreview GetOppositeHand(HandOrientation orientation)
    {
        return orientation == HandOrientation.Left ? rightHand : leftHand;
    }

    public void UpdateHands(HandPairPose pairPose, Transform parentTransform)
    {
        // Child the hands to the object we're working with
        if(leftHand == null || rightHand == null) 
        {
            InstantiateHands();
        }

        leftHand.transform.parent = parentTransform;
        rightHand.transform.parent = parentTransform;

        leftHand.SetPose(pairPose.GetHandPose(HandOrientation.Left));
        rightHand.SetPose(pairPose.GetHandPose(HandOrientation.Right));
    }

    public void DestroyHandPreviews()
    {
        // Make sure to destroy the gameobjects
#if UNITY_EDITOR

        if (leftHand != null)
            DestroyImmediate(leftHand.gameObject);

        if (rightHand != null)
            DestroyImmediate(rightHand.gameObject);

#endif
    }

    public void SavePose(HandPairPose pose)
    {
        // Mark object as dirty for saving
#if UNITY_EDITOR
        EditorUtility.SetDirty(pose);
#endif

        // Copy the hand info into
        pose.GetHandPose(HandOrientation.Left).Save(leftHand);
        pose.GetHandPose(HandOrientation.Right).Save(rightHand);
    }

    public void ResetPose(HandOrientation handOrientation)
    {
        var hand = GetHand(handOrientation);
#if UNITY_EDITOR
        Undo.RecordObject(hand.transform, "Reset Pose");
#endif
        hand.SetDefaultPose();
    }

    public bool CheckForNewInteractable()
    {
        XRBaseInteractable newInteractable = GetInteractable();

        // Update if different
        bool isDifferent = IsDifferentInteractable(CurretInteractable, newInteractable);
        CurretInteractable = isDifferent ? newInteractable : CurretInteractable;

        return isDifferent;
    }

    private XRBaseInteractable GetInteractable()
    {
        XRBaseInteractable newInteractable = null;
        GameObject selectedObject = null;

#if UNITY_EDITOR
        selectedObject = Selection.activeGameObject;

        
#endif

        if (selectedObject)
        {
            if (selectedObject.TryGetComponent(out XRBaseInteractable interactable))
                newInteractable = interactable;
        }

        return newInteractable;
    }

    private bool IsDifferentInteractable(XRBaseInteractable currentInteractable, XRBaseInteractable newInteractable)
    {
        // If we're selecting on object for the first time, it's true
        if (!currentInteractable)
            return true;

        // If we have a stored object, and we select a new one
        if (currentInteractable && newInteractable)
            return currentInteractable != newInteractable;

        return false;
    }

    public GameObject SetObjectPose(Pose pose)
    {
        GameObject selectedObject = null;

#if UNITY_EDITOR
        selectedObject = Selection.activeGameObject;
#endif

        if (selectedObject)
        {
            // Check if the object has a container to put a pose into
            if (selectedObject.TryGetComponent(out HandPairPose poseContainer))
            {
                //poseContainer.pose = pose;

                // Mark scene for saving
#if UNITY_EDITOR
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
#endif
            }
        }

        return selectedObject;
    }
}
