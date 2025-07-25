using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

public class RaceTask : SectionTaskBase
{
    [SerializeField] Canvas raceStartBoard;
    [SerializeField] Button raceStartButton;
    [SerializeField] CountdownTimer raceStartCountdown;
    [SerializeField] GameObject barrier;

    [SerializeField] Transform raceStart;
    [SerializeField] Checkpoint raceEndChecpoint;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        RobotController.MoveNPCToSpot(raceStart.transform.position);
        await WaitForAllEvents(cancellationToken, RobotController.OnTargetReached);

        raceEndChecpoint.gameObject.SetActive(true);
        await RobotController.SetInstructionsAsync("lets-race", cancellationToken);
        
        raceStartBoard.gameObject.SetActive(true);
        await WaitForAllEvents(cancellationToken, raceStartButton.onClick);
        
        raceStartCountdown.gameObject.SetActive(true);
        raceStartCountdown.StartCountdown(true, "GO!");
        await WaitForAllEvents(cancellationToken, raceStartCountdown.OnCountdownFinished);
        barrier.gameObject.SetActive(false);
        RobotController.MoveNPCToSpot(RobotEndPosition.position);

        var robotArrived = WaitForAllEvents(cancellationToken, RobotController.OnTargetReached);
        var userArrived = WaitForAllEvents(cancellationToken, raceEndChecpoint.OnCheckpointReached);

        Task completedTask = await Task.WhenAny(robotArrived, userArrived);

        var dialogueKey = completedTask == robotArrived? "player-lost" : "player-won";

        // Wait till both arrive
        await Task.WhenAll(robotArrived, userArrived);

        await RobotController.SetInstructionsAsync(dialogueKey, cancellationToken);
    }

    protected override void CleanupOnSkip()
    {
        barrier.gameObject.SetActive(false);

        raceEndChecpoint.SkipCheckPoint();

        raceStartBoard.gameObject.SetActive(false);
    }
}
