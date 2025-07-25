using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    // Generic method to find a component in sibling GameObjects
    public static T FindComponentInSiblings<T>(Transform transform) where T : Component
    {
        Transform parentTransform = transform.parent;

        if (parentTransform == null)
        {
            Debug.LogWarning("No parent found, so no siblings exist.");
            return null;
        }

        foreach (Transform sibling in parentTransform)
        {
            // Skip the current GameObject
            if (sibling == transform)
                continue;

            T component = sibling.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }
}
