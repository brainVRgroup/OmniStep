using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PutDownCubeTask : SectionTaskBase
{
    [SerializeField] Checkpoint secondRoomCubeCheckpoint;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        var speechTask = RobotController.SetInstructionsAsync("table-room-put-cube-down", cancellationToken);
        await WaitForAllEvents(cancellationToken, secondRoomCubeCheckpoint.OnCheckpointReached);
        await speechTask;
    }

    protected override void CleanupOnSkip()
    {
        secondRoomCubeCheckpoint.SkipCheckPoint();
    }
}
