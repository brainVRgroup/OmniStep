using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class HandPoserWindow : EditorWindow
{
    private Poses handPoses;

    private HandPairPose handPairPoseInEdit = null;
    private GameObject poseHelper = null;
    private HandPoserManager handPoserManager = null;

    private PopupField<HandPairPose> posesDropdownMenu;

    private void OnEnable()
    {
        CreatePoseHelper();
        Selection.selectionChanged += UpdateSelection;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= UpdateSelection;
        handPoserManager.DestroyHandPreviews();
        DestroyPoseHelper();
        base.SaveChanges();
    }

    public void InstantiateHandsPreview()
    {
        handPoserManager.CreateHandPreviewInstanceIfNeeded();

        var defaultPose = handPoserManager.GetDefaultPose();
        handPairPoseInEdit = ScriptableObjectsFactory.CreateHandPairPose("New pose", defaultPose.Left, defaultPose.Right);

        handPoses.HandPairPoses.Add(handPairPoseInEdit);
        posesDropdownMenu.choices = handPoses.HandPairPoses;
        posesDropdownMenu.value = handPairPoseInEdit;

        UpdateSelection();
    }

    public void CreateGUI()
    {

        rootVisualElement.Add(new Button(() => { InstantiateHandsPreview(); })
        {
            text = "Create New Posable Hands",

        });

        VisualElement selectionContainer = new();
        selectionContainer.style.marginTop = 10;

        // Create the dropdown menu
        posesDropdownMenu = new PopupField<HandPairPose>("Pose in edit", handPoses.HandPairPoses, 0);
        posesDropdownMenu.RegisterValueChangedCallback(evt => { handPairPoseInEdit = evt.newValue; UpdateSelection(); });
        selectionContainer.Add(posesDropdownMenu);

        selectionContainer.Add(new Button(() => { ResetHandPairPose(); })
        {
            text = "Reset Selected Pose"
        });

        selectionContainer.Add(new Button(() => { InstantiateHandsPreview(); })
        {
            text = "Delete Selected Pose"
        });

        rootVisualElement.Add(selectionContainer);

        // Create the columns container
        VisualElement columnsContainer = new();
        columnsContainer.style.flexDirection = FlexDirection.Row;
        columnsContainer.style.marginTop = 10;
        columnsContainer.style.marginBottom = 10;

        // Create left and right columns
        VisualElement leftColumn = CreateColumn("Left Hand Actions");
        VisualElement rightColumn = CreateColumn("Right Hand Actions");

        // Add buttons to the left column
        leftColumn.Add(CreateButton("Mirror Joints L > R", () => MirrorSelectedJointPoseOnly(HandOrientation.Left)));
        leftColumn.Add(CreateButton("Mirror All L > R", () => MirrorSelectedHandPose(HandOrientation.Left)));
        leftColumn.Add(CreateButton("Reset", () => ResetPose(HandOrientation.Left)));

        // Add buttons to the right column
        rightColumn.Add(CreateButton("Mirror Joints R > L", () => MirrorSelectedJointPoseOnly(HandOrientation.Right)));
        rightColumn.Add(CreateButton("Mirror All R > L", () => MirrorSelectedHandPose(HandOrientation.Right)));
        rightColumn.Add(CreateButton("Reset", () => ResetPose(HandOrientation.Right)));

        // Add columns to the container
        columnsContainer.Add(leftColumn);
        columnsContainer.Add(rightColumn);

        // Add the columns container to the root
        rootVisualElement.Add(columnsContainer);

        rootVisualElement.Add(new Button(() => { handPoserManager.SavePose(handPairPoseInEdit); })
        {
            text = "Save"
        });
    }

    private VisualElement CreateColumn(string headerText)
    {
        VisualElement column = new();
        column.style.flexDirection = FlexDirection.Column;
        column.style.flexGrow = 1;
        column.style.marginLeft = 5;
        column.style.marginRight = 5;

        Label header = new(headerText);
        header.style.unityTextAlign = TextAnchor.MiddleCenter;
        header.style.marginBottom = 10;
        column.Add(header);

        return column;
    }

    private Button CreateButton(string text, System.Action callback)
    {
        Button button = new(callback)
        {
            text = text
        };
        button.style.marginBottom = 5; // Add spacing between buttons
        return button;
    }

    public static void Open(Poses handPoses)
    {
        if (HasOpenInstances<HandPoserWindow>())
        {
            FocusWindowIfItsOpen<HandPoserWindow>();
            return;
        }

        HandPoserWindow window = GetWindow<HandPoserWindow>("Hand Poser");
        window.Focus();


    }

    private void CreatePoseHelper()
    {
        if (!poseHelper)
        {
            Object helperPrefab = Resources.Load("PoserHelper");

            // Instantiate it into the scene, mark as not to save
            poseHelper = (GameObject)PrefabUtility.InstantiatePrefab(helperPrefab);
            poseHelper.hideFlags = HideFlags.DontSave;

            handPoserManager = poseHelper.GetComponent<HandPoserManager>();

            // Set initial selection setup
            UpdateSelection();

        }
    }

    private void DestroyPoseHelper()
    {
        DestroyImmediate(poseHelper);
    }

    private void MirrorSelectedHandPose(HandOrientation sourceHand)
    {
        handPoserManager.MirrorHandPose(sourceHand);
    }

    private void MirrorSelectedJointPoseOnly(HandOrientation sourceHand)
    {
        handPoserManager.MirrorJointPoseOnly(sourceHand);
    }

    private void UpdateSelection()
    {
        var targetObject = Selection.activeGameObject;

        if (targetObject == null)
        {
            return;
        }

        if (targetObject.TryGetComponent(out handPoses))
        {
            // Pose must first be created before we update hands
            if (!handPoses || handPoses.HandPairPoses.Count == 0)
            {
                return;
            }

            if (handPoserManager.CheckForNewInteractable() || handPairPoseInEdit == null)
            {

                handPairPoseInEdit = handPoses.HandPairPoses[0];

            }

            UpdateActivePose(targetObject, handPairPoseInEdit);
        }



    }

    private void UpdateActivePose(GameObject targetObject, HandPairPose newHandPairPose)
    {
        handPoserManager.UpdateHands(newHandPairPose, targetObject.transform);
    }

    private void ResetPose(HandOrientation handOrientation)
    {
        handPoserManager.ResetPose(handOrientation);
    }

    private void ResetHandPairPose()
    {
        ResetPose(HandOrientation.Left);
        ResetPose(HandOrientation.Right);
    }
}
