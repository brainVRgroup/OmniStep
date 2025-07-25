using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EnterFinalRoomTask : SectionTaskBase
{
    [SerializeField] RobotTvManager robotTV;
    [SerializeField] TriggerController endTrigger;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        await RobotController.SetInstructionsAsync("walk-through-door", cancellationToken);
        await WaitForAllEvents(cancellationToken, endTrigger.TriggerEntered);
        await robotTV.ToggleAsync();
        await robotTV.SetEntryAsync("good-job", cancellationToken: cancellationToken);
        await robotTV.SetEntryAsync("successful-finish", cancellationToken: cancellationToken);
    }
}
