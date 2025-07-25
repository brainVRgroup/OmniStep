using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PickUpCubeTask : SectionTaskBase
{
    [SerializeField] DissolvableObject interactable;
    [SerializeField] CubeController cubeController;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        await RobotController.SetInstructionsAsync("grip-button-explanation", cancellationToken);
        await RobotController.SetInstructionsAsync("grip-outline-explanation", cancellationToken);

        Task speechTask = RobotController.SetInstructionsAsync("grip-cube", cancellationToken);

        interactable.Appear();
        await WaitForAllEvents(cancellationToken, cubeController.OnPickUp);

        await speechTask;
        await RobotController.SetInstructionsAsync("well-done", cancellationToken);
    }

    protected override void CleanupOnSkip()
    {
        interactable.Appear();
    }
}
