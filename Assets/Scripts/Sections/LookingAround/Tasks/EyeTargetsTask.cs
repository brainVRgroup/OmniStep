using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EyeTargetsTask : SectionTaskBase
{
    [SerializeField] string robotTextKey;
    [SerializeField] AnimatorEvents[] eyeTargets;

    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        Task speechTask = RobotController.SetInstructionsAsync(robotTextKey, cancellationToken: cancellationToken);

        var completionEvents = new UnityEvent[eyeTargets.Length];
        for (int i = 0; i < eyeTargets.Length; i++)
        {
            eyeTargets[i].gameObject.SetActive(true);
            completionEvents[i] = (eyeTargets[i].OnAnimationStart);
        }

        await WaitForAllEvents(cancellationToken, completionEvents);
        await speechTask;
    }

    protected override void CleanupOnSkip()
    {
        for (int i = 0; i < eyeTargets.Length; i++)
        {
            eyeTargets[i].gameObject.SetActive(false);
        }
    }
}
