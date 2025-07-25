using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_Text))]
public class CountdownTimer : MonoBehaviour
{
    [SerializeField] int startTimeInSeconds = 3; // Set your start time in seconds
    [SerializeField] TMP_Text countdownText; // Assign your UI Text element in the inspector

    public UnityEvent OnCountdownFinished;

    private int remainingTime;

    public void StartCountdown(bool deacitvateOnFinish = true, string customTextOnFinish = "0")
    {
        remainingTime = startTimeInSeconds;
        StartCoroutine(CountdownCoroutine(deacitvateOnFinish, customTextOnFinish));
    }

    private IEnumerator CountdownCoroutine(bool deacitvateOnFinish = true, string customTextOnFinish = "0")
    {
        while (remainingTime > 0)
        {
            countdownText.text = remainingTime.ToString(); // Update the UI text
            remainingTime--;
            yield return new WaitForSeconds(1); // Wait for 1 second
        }

        countdownText.text = customTextOnFinish; // Ensure the final value is 0
        OnCountdownFinished.Invoke(); 

        if (deacitvateOnFinish)
        {
            yield return new WaitForSeconds(1); // Wait for 1 second
            gameObject.SetActive(false);
        }
        
    }

 
}
