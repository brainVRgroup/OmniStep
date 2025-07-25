using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PerformFirstStepsTask : SectionTaskBase
{
    [SerializeField] GameObject loccomotionSystem;

    [SerializeField] Checkpoint firstCheckpoint;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        loccomotionSystem.gameObject.SetActive(true);

        Task speechTask = RobotController.SetInstructionsAsync("first-steps-ever", cancellationToken);

        firstCheckpoint.gameObject.SetActive(true);
        await WaitForAllEvents(cancellationToken, firstCheckpoint.OnCheckpointReached);

        await speechTask;
        await RobotController.SetInstructionsAsync("first-steps-ever-completed", cancellationToken);
    }

    protected override void CleanupOnSkip()
    {
        firstCheckpoint.SkipCheckPoint();
    }
}
