using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WalkShowcaseAndLeanTask : SectionTaskBase
{
    [SerializeField] Animator walls;
    [SerializeField] Button readyButton;
    [SerializeField] LeanForwardDetection leanDetection;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        await RobotController.SetInstructionsAsync("first-steps-intro", cancellationToken);
        walls.SetTrigger("Hide");
        await WalkShowcase(cancellationToken);

        readyButton.gameObject.SetActive(true);
        await WaitForAllEvents(cancellationToken, readyButton.onClick);
        leanDetection.SetStartingPosition();
        await RobotController.SetInstructionsAsync("walk-showcase-lean", cancellationToken);

        // Detect lean forward
        await WaitForAllConditions(cancellationToken, () => leanDetection.IsLeanedForward == true);
    }

    private async Task WalkShowcase(CancellationToken cancellationToken)
    {
        // Technique recap from robot
        await RobotController.SetInstructionsAsync("walk-showcase-intro", cancellationToken);
        RobotController.RotateSidewaysToPlayer();

        Task speechTask = RobotController.SetInstructionsAsync("walk-showcase-intro-lean", cancellationToken, false);
        await RobotController.Animator.SetTriggerAsync("lean", 0, cancellationToken);
        await speechTask;
        Task speechTask2 = RobotController.SetInstructionsAsync("walk-showcase-intro-step", cancellationToken, false);
        await RobotController.Animator.SetTriggerAsync("step-left", 0, cancellationToken);
        await speechTask2;
        Task speechTask3 = RobotController.SetInstructionsAsync("walk-showcase-intro-slide", cancellationToken, false);
        await RobotController.Animator.SetTriggerAsync("slide-left", 0, cancellationToken);
        await speechTask3;
        Task speechTask4 = RobotController.SetInstructionsAsync("walk-showcase-intro-other-leg", cancellationToken, false);
        await RobotController.Animator.SetTriggerAsync("step-right", 0, cancellationToken);
        await RobotController.Animator.SetTriggerAsync("slide-right", 0, cancellationToken);
        await speechTask4;
        await RobotController.Animator.SetTriggerAsync("lean-walk", 0, cancellationToken);
        RobotController.Animator.SetTrigger("stop-walk-showcase");

        RobotController.RotateToPlayer();
        VRManager.HighlightTrackpad();
        await RobotController.SetInstructionsAsync("walk-showcase-intro-end", cancellationToken, false);
        await RobotController.SetInstructionsAsync("walk-showcase-stand-up", cancellationToken, false);

        VRManager.UnhighlightTrackpad();

    }

    override protected void CleanupOnSkip()
    {
        RobotController.Animator.SetTrigger("stop-walk-showcase");
        walls.SetTrigger("Hide");
        readyButton.gameObject.SetActive(false);
        VRManager.UnhighlightTrackpad();
        RobotController.RotateToPlayer();
        
    }
}
