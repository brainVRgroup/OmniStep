using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class FindObjectsTask : SectionTaskBase
{
    [SerializeField] Checkpoint cubeChecpoint1;
    [SerializeField] Checkpoint cubeChecpoint2;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        await RobotController.SetInstructionsAsync("table-room-two-cubes", cancellationToken);
        await WaitForAllEvents(cancellationToken, cubeChecpoint1.OnCheckpointReached, cubeChecpoint2.OnCheckpointReached);
        await RobotController.SetInstructionsAsync("two-cubes-done", cancellationToken);
    }

    protected override void CleanupOnSkip()
    {
        cubeChecpoint1.SkipCheckPoint();
        cubeChecpoint2.SkipCheckPoint();
    }
}
