using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PosableHand : PosableHandBase
{
    [SerializeField] bool isControllerEnabled;
    [SerializeField] private XRBaseInteractor targetInteractor;
    [SerializeField] private Transform attach;

    GameObject controller;
    HandPairPose controllerPairPose;

    Animator animator;

    public void ShowController(bool isShown)
    {
        if (controller)
            controller.SetActive(isShown);

        animator.SetBool("ControllerVisible", controller.activeSelf);

        if (controller.activeSelf && controllerPairPose)
        {
            Debug.Log("Setting controller pose");
            SetPose(controllerPairPose.GetHandPose(HandType));
            controller.transform.SetLocalPositionAndRotation(attach.localPosition, attach.localRotation);
        }
    }

    private new void Awake()
    {
        base.Awake();

        if (targetInteractor == null)
        {
            targetInteractor = ComponentExtensions.FindComponentInSiblings<XRDirectInteractor>(transform);
            targetInteractor.attachTransform = attach;
        }

        animator = GetComponent<Animator>();
        InitializeControlers();
    }

    private void InitializeControlers()
    {
        var vrManager = FindAnyObjectByType<VRManager>();
        controller = vrManager.InstantiateController(HandType, transform.position, transform);
        controller.transform.SetParent(transform);

        var poses = controller.GetComponent<Poses>();

        if (poses && poses.HandPairPoses.Count > 0)
        {
            controllerPairPose = poses.HandPairPoses[0];
        }

        ShowController(isControllerEnabled);
    }

    private void OnEnable()
    {
        targetInteractor.selectEntered.AddListener(TrySetObjectPose);
        targetInteractor.selectExited.AddListener(TrySetObjectDefaultPose);
    }

    private void OnDisable()
    {
        targetInteractor.selectEntered.RemoveListener(TrySetObjectPose);
        targetInteractor.selectExited.RemoveListener(TrySetObjectDefaultPose);
    }

    public override void ApplyOffset(Vector3 position, Quaternion rotation)
    {
        var localPosition = targetInteractor.attachTransform.parent.InverseTransformVector(position);
        var localRotation = Quaternion.Inverse(targetInteractor.attachTransform.parent.rotation) * rotation;
        Vector3 finalPosition = localPosition * -1.0f;
        Quaternion finalRotation = Quaternion.Inverse(localRotation);

        targetInteractor.attachTransform.localPosition = RotatePointAroundPivot(finalPosition, Vector3.zero, finalRotation.eulerAngles);
        targetInteractor.attachTransform.localRotation = Quaternion.Inverse(rotation);

    }

    private void TrySetObjectPose(SelectEnterEventArgs args)
    {
        if (args.interactableObject == null)
        {
            return;
        }


        // Try to get the desired component from the interactable object
        if (args.interactableObject.transform.TryGetComponent(out Poses handPoses))
        {
            // We need to disable animator if present, to override the pose
            if (animator != null)
            {
                animator.enabled = false;
            }

            ShowController(false);

            // Get the point of attachment from the interactor
            Vector3 attachPoint = args.interactorObject.transform.position;

            // Find the closest pose
            Pose closestPose = FindClosestPose(handPoses, attachPoint);

            SetPose(closestPose);
        }
    }

    private void TrySetObjectDefaultPose(SelectExitEventArgs args)
    {
        // Try to get the desired component from the interactable object
        if (args.interactableObject.transform.TryGetComponent(out Poses handPoses))
        {
            // Enable back the animator if present
            if (animator != null)
            {
                animator.enabled = true;
            }
            else
            {
                SetDefaultPose();
            }

            if (isControllerEnabled)
                ShowController(true);
        }
    }

    private Pose FindClosestPose(Poses poses, Vector3 attachPoint)
    {
        HandPairPose closestPose = null;
        float closestDistance = float.MaxValue;

        foreach (var pairPose in poses.HandPairPoses)
        {
            float distance = Vector3.Distance(pairPose.GetHandPose(HandType).attachPosition, attachPoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPose = pairPose;
            }
        }

        return closestPose.GetHandPose(HandType);
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 direction = point - pivot;
        direction = Quaternion.Euler(angles) * direction;
        return direction + pivot;
    }

}
