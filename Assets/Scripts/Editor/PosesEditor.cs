using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Poses))]
public class PosesEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        var handPoses = (Poses)target;
        var editButton = new Button(() => HandPoserWindow.Open(handPoses))
        {
            text = "Edit hand poses"
        };

        root.Add(editButton);
        return root;
    }
}
