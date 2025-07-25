using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class GazeInteractable : MonoBehaviour
{
    [SerializeField] bool deactivated = false;
    [SerializeField] int gazeTime = 3;

    [Header("Events")]
    public UnityEvent OnGazeEnter;
    public UnityEvent OnGazeExit;
    public UnityEvent OnGazeCompleted;

    // Optional parameter to show the gaze progress
    [SerializeField] ProgressFiller progressFiller;

    private bool completed = false;
    private bool isCounting = false;
    private Coroutine countdownCoroutine;

    public void SetActive()
    {
        deactivated = false;
        if (deactivated && isCounting)
        {
            StopCoroutine(countdownCoroutine);
            isCounting = false;
        }
    }

    public void GazeEnter()
    {
        if (completed || deactivated || isCounting) return;

        OnGazeEnter?.Invoke();
        isCounting = true;

        if (progressFiller)
            progressFiller.StartFill(gazeTime);

        countdownCoroutine = StartCoroutine(Countdown());
    }

    public void GazeExit()
    {
        if (completed || deactivated || !isCounting) return;
        
        if (progressFiller)
            progressFiller.StopFill();

        StopCoroutine(countdownCoroutine);
        isCounting = false;
        OnGazeExit?.Invoke();
    }

    public void GazeCompleted()
    {
        if (completed || deactivated) return;

        completed = true;
        OnGazeCompleted?.Invoke();
    }

    IEnumerator Countdown()
    {
        while (gazeTime > 0)
        {
            yield return new WaitForSeconds(1);
            gazeTime -= 1;
        }

        GazeCompleted();
    }
}
