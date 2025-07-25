using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ExplainUIInteractionTask : SectionTaskBase
{
    [SerializeField] Canvas canvas;
    [SerializeField] XRInteractorLineVisual rayLine;
    [SerializeField] XRInteractorLineVisual rayLine2;
    [SerializeField] UIHoverDetector buttonHover;
    [SerializeField] Button button;
    [SerializeField] UIHoverDetector buttonHover2;
    [SerializeField] Button button2;
    [SerializeField] Button doneButton;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        // Enable rays 
        rayLine.enabled = true;
        rayLine2.enabled = true;

        Task speechTask = RobotController.SetInstructionsAsync("first-UI-interaction", cancellationToken);

        // Show buttons
        canvas.gameObject.SetActive(true);

        // wait for hover over button
        await WaitForOneOfEvents(cancellationToken, buttonHover.OnHoverDurationReached, buttonHover2.OnHoverDurationReached);
        await speechTask;

        // Highlight controller button
        VRManager.HighlightGripButton();

        Task speechTask2 = RobotController.SetInstructionsAsync("press-UI-button", cancellationToken);

        // Wait for button press
        await WaitForOneOfEvents(cancellationToken, button.onClick, button2.onClick);

        VRManager.UnhighlightGripButton();
        await speechTask2;

        Task speechTask3 = RobotController.SetInstructionsAsync("cube-customization", cancellationToken);

        // Wait for done press
        Task doneButtonClick = WaitForAllEvents(cancellationToken, doneButton.onClick);
        doneButton.gameObject.SetActive(true);
        await doneButtonClick;

        canvas.gameObject.SetActive(false);
        await speechTask3;
    }

    protected override void CleanupOnSkip()
    {
        canvas.gameObject.SetActive(false);

        doneButton.onClick.Invoke();

        VRManager.UnhighlightGripButton();
    }
}
