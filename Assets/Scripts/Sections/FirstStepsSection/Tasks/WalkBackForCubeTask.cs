using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WalkBackForCubeTask : SectionTaskBase
{
    [SerializeField] XRGrabInteractable interactable;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        var speechTask = RobotController.SetInstructionsAsync("first-steps-grab-cube", cancellationToken);
        await WaitForEvent(interactable.firstSelectEntered, cancellationToken);

        await speechTask;
    }
}
