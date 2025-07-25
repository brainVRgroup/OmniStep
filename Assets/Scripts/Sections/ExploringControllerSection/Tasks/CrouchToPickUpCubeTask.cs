using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CrouchToPickUpCubeTask : SectionTaskBase
{
    [SerializeField] CubeController cubeController;

    [SerializeField] Checkpoint checkpoint;
    [SerializeField] Transform dropSpot;

    [SerializeField] Transform cubeRespawnOnSkip;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        Task eventTask = WaitForAllEvents(cancellationToken, RobotController.OnTargetReached);
        RobotController.MoveNPCToObject(cubeController.transform.position);
        await eventTask;

        RobotController.Animator.SetLayerWeight(1, 1.0f);
        await RobotController.Animator.SetTriggerAsync("pick-up", 1, cancellationToken);

        Task eventTask2 = WaitForAllEvents(cancellationToken, RobotController.OnTargetReached);
        RobotController.MoveNPCToSpot(dropSpot.position, false, true);
        await eventTask2;
        await RobotController.Animator.SetTriggerAsync("drop", 1, cancellationToken);

        RobotController.Animator.SetLayerWeight(1, 0.0f);
        RobotController.MoveNPCToSpot(RobotEndPosition.position);
        await WaitForAllEvents(cancellationToken, RobotController.OnTargetReached);

        checkpoint.gameObject.SetActive(true);
        Task speechTask = RobotController.SetInstructionsAsync("pick-up-cube", cancellationToken);
        await WaitForAllEvents(cancellationToken, checkpoint.OnCheckpointReached);
        cubeController.transform.SetPositionAndRotation(cubeRespawnOnSkip.position, Quaternion.identity);
        await speechTask;
        await RobotController.SetInstructionsAsync("good-job", cancellationToken);
    }

    protected override void CleanupOnSkip()
    {
        RobotController.Animator.SetTrigger("drop");
        RobotController.Animator.SetLayerWeight(1, 0.0f);
        checkpoint.SkipCheckPoint();
        cubeController.transform.SetPositionAndRotation(cubeRespawnOnSkip.position, Quaternion.identity);
    }
}
