using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class SectionTaskBase : MonoBehaviour
{
    [Tooltip("Optional specified end position for the robot")]
    [SerializeField] Transform robotEndPosition;

    public Transform RobotEndPosition {  get { return robotEndPosition; } }

    protected VRManager VRManager { get; private set; }
    protected RobotController RobotController { get; private set; }

    private CancellationTokenSource cancellationTokenSource;

    private void Start()
    {
        RobotController = FindAnyObjectByType<RobotController>();
        VRManager = FindAnyObjectByType<VRManager>();

        if (RobotController == null)
        {
            UnityEngine.Debug.LogError($"{nameof(RobotController)} not found in scene");
        }

        if (VRManager == null)
        {
            UnityEngine.Debug.LogError($"{nameof(VRManager)} not found  in scene");
        }
    }

    public async Task<double> BeginTask(Transform robotStartPosition, CancellationToken externalCancellationToken)
    {
        cancellationTokenSource = new CancellationTokenSource();
        var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, externalCancellationToken);

        Stopwatch stopwatch = new();
        double elapsedTime = -1;

        try
        {
            var cancellationToken = linkedCancellationTokenSource.Token;

            // Check if the robot is at the proper position, otherwise wait for him to move there
            if (robotStartPosition != null && robotStartPosition.position != RobotController.transform.position)
            {
                // This makes sure the position is propagated in case cancel is called before 
                // the robot reaches the desired position
                if (robotEndPosition == null)
                {
                    robotEndPosition = robotStartPosition;
                }

                RobotController.MoveNPCToSpot(robotStartPosition.position);
                await WaitForAllEvents(cancellationToken, RobotController.OnTargetReached);
            }

            // Start timing the task
            stopwatch.Reset();
            stopwatch.Start();

            await RunTask(linkedCancellationTokenSource.Token);

            stopwatch.Stop();
            elapsedTime = stopwatch.Elapsed.TotalSeconds;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            UnityEngine.Debug.Log("Task was cancelled");
        }
        finally
        {
            linkedCancellationTokenSource.Dispose();
            cancellationTokenSource.Dispose();
        }

        return elapsedTime;

    }

    public void SkipTask()
    {
        cancellationTokenSource?.Cancel();
        CleanupOnSkip();
    }

    protected abstract Task RunTask(CancellationToken cancellationToken);

    protected virtual void CleanupOnSkip() { }

    protected async Task WaitForOneOfConditions(CancellationToken cancellationToken, params Func<bool>[] conditions)
    {
        while (!conditions.Any(condition => condition()))
        {
            // Check if the cancellation token is triggered
            if (cancellationToken.IsCancellationRequested)
            {
                // Throw a TaskCanceledException to propagate the cancellation
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.Yield(); // Yield execution until the next frame
        }
    }

    protected async Task WaitForAllConditions(CancellationToken cancellationToken, params Func<bool>[] conditions)
    {
        while (!conditions.All(condition => condition()))
        {
            // Check if the cancellation token is triggered
            if (cancellationToken.IsCancellationRequested)
            {
                // Throw a TaskCanceledException to propagate the cancellation
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.Yield(); // Yield execution until the next frame
        }
    }

    protected async Task WaitForOneOfEvents(CancellationToken cancellationToken, params UnityEvent[] unityEvents)
    {
        var tcsList = new TaskCompletionSource<bool>[unityEvents.Length];
        var tasks = new Task[unityEvents.Length];
        for (int i = 0; i < unityEvents.Length; i++)
        {
            int localIndex = i; // Local copy of i
            var tcs = new TaskCompletionSource<bool>();
            tcsList[localIndex] = tcs;
            void action()
            {
                unityEvents[localIndex].RemoveListener(action);
                tcs.SetResult(true);
                // Remove other listeners to prevent multiple completions
                for (int j = 0; j < unityEvents.Length; j++)
                {
                    if (localIndex != j)
                    {
                        unityEvents[j].RemoveListener(tcsList[j].SetCanceled);
                    }
                }
            }
            unityEvents[localIndex].AddListener(action);
            tasks[localIndex] = tcs.Task;
            // Add listeners to cancel other TCS when one completes
            for (int j = 0; j < unityEvents.Length; j++)
            {
                if (localIndex != j)
                {
                    unityEvents[j].AddListener(tcs.SetCanceled);
                }
            }
            // Register cancellation token to cancel the TCS
            cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled();
                unityEvents[localIndex].RemoveListener(action);
            });
        }
        try
        {
            await Task.WhenAny(tasks);
        }
        finally
        {
            // Cleanup: Remove all listeners
            for (int i = 0; i < unityEvents.Length; i++)
            {
                unityEvents[i].RemoveAllListeners();
            }
        }
        // Check for cancellation after the wait completes
        cancellationToken.ThrowIfCancellationRequested();
    }

    protected async Task WaitForAllEvents(CancellationToken cancellationToken, params UnityEvent[] unityEvents)
    {
        var tasks = new List<Task>();

        foreach (var selector in unityEvents)
        {
            var tcs = new TaskCompletionSource<bool>();

            void action()
            {
                selector.RemoveListener(action);
                tcs.SetResult(true);
            }

            selector.AddListener(action);

            // Add the task to the list
            tasks.Add(tcs.Task);

            // Register cancellation to remove the listener and cancel the task
            cancellationToken.Register(() =>
            {
                selector.RemoveListener(action);
                tcs.TrySetCanceled();
            });
        }

        try
        {
            // Wait for all tasks to complete
            await Task.WhenAll(tasks);
        }
        finally
        {
            // Cleanup: Remove all listeners
            foreach (var selector in unityEvents)
            {
                selector?.RemoveAllListeners();
            }
        }

        // Check for cancellation after the wait completes
        cancellationToken.ThrowIfCancellationRequested();
    }

    protected async Task WaitForEvent<T>(UnityEvent<T> unityEvent, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();

        void action(T _)
        {
            unityEvent.RemoveListener(action);
            tcs.TrySetResult(true);
        }

        unityEvent.AddListener(action);

        // Register cancellation to remove the listener and cancel the task
        cancellationToken.Register(() =>
        {
            unityEvent.RemoveListener(action);
            tcs.TrySetCanceled();
        });

        try
        {
            await tcs.Task; // Wait for the event or cancellation
        }
        finally
        {
            // Ensure the listener is removed
            unityEvent.RemoveListener(action);
        }

        // Check for cancellation after the wait completes
        cancellationToken.ThrowIfCancellationRequested();
    }

}
