using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WalkToFirstPassageTask : SectionTaskBase
{
    [SerializeField] Animator cubePlatform;
    [SerializeField] Checkpoint secondCheckpoint;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        cubePlatform.SetTrigger("Show");
        secondCheckpoint.gameObject.SetActive(true);
        await RobotController.SetInstructionsAsync("first-steps-go-to-door", cancellationToken);
        RobotController.MoveNPCToSpot(RobotEndPosition.position);
        var robotArrived = WaitForAllEvents(cancellationToken, RobotController.OnTargetReached);

        await WaitForAllEvents(cancellationToken, secondCheckpoint.OnCheckpointReached);

        await robotArrived;
    }

    override protected void CleanupOnSkip()
    {
        secondCheckpoint.SkipCheckPoint();
    }
}
