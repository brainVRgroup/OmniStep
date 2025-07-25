using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private int _stepCount = 5;
    [SerializeField] private float _timeStep = 1f;

    [Header("Events")]
    public UnityEvent Start;
    public UnityEvent Stop;
    public UnityEvent OnStep;
    public UnityEvent OnEnd;

    private IEnumerator _runningTimer;

    public void StartTimer()
    {
        // Timer already running
        if (_runningTimer != null)
        {
            Debug.Log("Start timer called on running timer.");
            return;
        }

        _runningTimer = TimerCoroutine();
        StartCoroutine(_runningTimer);
        Start?.Invoke();
    }

    public void StopTimer()
    {
        if (_runningTimer == null)
        {
            Debug.Log("Stop timer called on stopped timer.");
            return;
        }

        StopCoroutine(_runningTimer);
        Stop?.Invoke();
        _runningTimer = null;
    }

    IEnumerator TimerCoroutine()
    {
        for (int i = 0; i < _stepCount; i++)
        {
            yield return new WaitForSeconds(_timeStep);
            OnStep?.Invoke();
        }

        OnEnd?.Invoke();
    }
}
