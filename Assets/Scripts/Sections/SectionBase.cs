using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class SectionBase : MonoBehaviour
{
    public UnityEvent OnSectionCompleted;

    [SerializeField] public List<SectionTaskBase> tasks;

    private SectionTaskBase currentTask;

    private CancellationTokenSource sectionCancellationTokenSource;

    public async Task<List<KeyValuePair<string, double>>> StartAsync(CancellationToken externalCancellationToken)
    {
        sectionCancellationTokenSource = new CancellationTokenSource();
        var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(sectionCancellationTokenSource.Token, externalCancellationToken);

        var taskTimes = new List<KeyValuePair<string, double>>();

        try
        {
            foreach (var task in tasks)
            {
                // If not the first task, set robot endposition from previous task
                Transform lastRobotEndPosition = null;
                if (currentTask != null)
                {
                    lastRobotEndPosition = currentTask.RobotEndPosition;
                }
                
                currentTask = task;

                externalCancellationToken.ThrowIfCancellationRequested();

                var elapsedTime = await task.BeginTask(lastRobotEndPosition, linkedCancellationTokenSource.Token);

                externalCancellationToken.ThrowIfCancellationRequested();

                //Store task times
                taskTimes.Add(new KeyValuePair<string, double>(task.gameObject.name, elapsedTime));
            }
        }
        catch (OperationCanceledException)
        {
            UnityEngine.Debug.Log("Section was cancelled");
        }
        finally
        {
            linkedCancellationTokenSource.Dispose();
            sectionCancellationTokenSource.Dispose();
            OnSectionCompleted.Invoke();
        }

        // Return the list of task names and times
        return taskTimes;
    }

    public void SkipCurrentTask()
    {
        if (currentTask != null)
        {
            currentTask.SkipTask();
        }
        else
        {
            UnityEngine.Debug.LogError("No current task to skip.");
        }
    }

    public void SkipSection()
    {
        sectionCancellationTokenSource?.Cancel();
    }
}
