using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectsFactory
{
    public static HandPairPose CreateHandPairPose(string PoseName, Pose leftHand, Pose rightHand)
    {
        var handPairPose = CreateScriptableObject<HandPairPose>("Poses/" + PoseName);

        handPairPose.SetHandPose(leftHand, HandOrientation.Left);
        handPairPose.SetHandPose(rightHand, HandOrientation.Right);
        handPairPose.PoseName = PoseName;

        return handPairPose;
    }

    private static T CreateScriptableObject<T>(string path) where T : ScriptableObject
    {
        // Create a new instance of MyScriptableObject
        T instance = ScriptableObject.CreateInstance<T>();

        var fullPath = "Assets/Prefabs/" + path;
        var extension = ".asset";

#if UNITY_EDITOR
        // Check if the file already exists and append a number if it does
        int count = 1;
        while (AssetDatabase.LoadAssetAtPath<Object>(fullPath + extension) != null)
        {
            fullPath = fullPath + count;
            count++;
        }

        AssetDatabase.CreateAsset(instance, fullPath + extension);
        AssetDatabase.SaveAssets();
#endif


        return instance;
    }
}
