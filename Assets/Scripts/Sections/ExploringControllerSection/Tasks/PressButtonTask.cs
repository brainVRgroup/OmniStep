using System.Threading;
using System.Threading.Tasks;

public class PressButtonTask : SectionTaskBase
{
    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        // text look at hands
        Task lookAtHandsSpeechTask = RobotController.SetInstructionsAsync("look-at-hands", cancellationToken);
        VRManager.HighlightGripButton();

        await lookAtHandsSpeechTask;

        Task pressButtonSpeechTask = RobotController.SetInstructionsAsync("grip-button", cancellationToken);
        Task buttonPress = WaitForOneOfConditions(cancellationToken, () => VRManager.LeftGripPressed, () => VRManager.RightGripPressed);

        await buttonPress;
        VRManager.UnhighlightGripButton();

        await pressButtonSpeechTask;
    }

    protected override void CleanupOnSkip()
    {
        VRManager.UnhighlightGripButton();
    }
}
