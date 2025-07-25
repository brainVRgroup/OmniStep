using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ComeToObjectsRoomTask : SectionTaskBase
{
    [SerializeField] Checkpoint secondRoomCheckpoint;
    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        await RobotController.SetInstructionsAsync("table-room-walk-in", cancellationToken);
        await RobotController.SetInstructionsAsync("stand-in-highlighted-spot", cancellationToken);

        RobotController.MoveNPCToSpot(RobotEndPosition.position);
        await WaitForAllEvents(cancellationToken, secondRoomCheckpoint.OnCheckpointReached, RobotController.OnTargetReached);
    }

    protected override void CleanupOnSkip()
    {
        secondRoomCheckpoint.SkipCheckPoint();
    }
}
