using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ThrowCubeTask : SectionTaskBase
{
    [SerializeField] HittableTarget target;
    [SerializeField] HittableTarget zoneAroundTarget;
    [SerializeField] CubeController cubeController;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        //set text throw-cube

        Task speechTask = RobotController.SetInstructionsAsync("throw-cube", cancellationToken);

        target.gameObject.SetActive(true);

        //wait for the event on cube that it landed, if it does not land within specified region, respawn cube and tell them to try again.
        await WaitForAllEvents(cancellationToken, cubeController.OnLandingAfterThrow);
        await speechTask;

        while (!target.wasHit && !zoneAroundTarget.wasHit)
        {
            // Respawn the cube since the target wasn't hit
            cubeController.RespawnCube();

            // Provide feedback to the player to try again
            Task speechAgainTask = RobotController.SetInstructionsAsync("try-again", cancellationToken);
            await WaitForAllEvents(cancellationToken, cubeController.OnLandingAfterThrow);
            await speechAgainTask;

        }

        target.gameObject.SetActive(false);

        if (target.wasHit)
        {
            await RobotController.SetInstructionsAsync("target-hit", cancellationToken);
        }
        else
        {
            await RobotController.SetInstructionsAsync("nice-try", cancellationToken);
        }
    }

    protected override void CleanupOnSkip()
    {
        target.gameObject.SetActive(false);
    }
}
