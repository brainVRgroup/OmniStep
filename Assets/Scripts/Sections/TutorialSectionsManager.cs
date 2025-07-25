using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TutorialSectionsManager : MonoBehaviour
{
    // Singleton instance
    private static TutorialSectionsManager _instance;

    // Singleton property
    public static TutorialSectionsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // If the instance is null, find or create it
                _instance = FindObjectOfType<TutorialSectionsManager>();

                if (_instance == null)
                {
                    Debug.LogError("TutorialSectionsManager instance not found in the scene! Please make sure it's added to the scene.");
                }
            }
            return _instance;
        }
    }

    [SerializeField] bool startOnPlay = false;
    [SerializeField] bool trackAnalytics = true;
    [SerializeField] SectionBase[] sections;

    [SerializeField] public UnityEvent OnSectionChange;
    [SerializeField] public UnityEvent OnStart;

    public bool Started { get; private set; }

    int currentSectionIndex = -1;
    bool tutorialCompleted = false;
    private Dictionary<string, List<KeyValuePair<string, double>>> allTaskTimes = new();

    // The CancellationTokenSource used for cancelling the tutorial
    private CancellationTokenSource tutorialCancellationTokenSource;

    private void Awake()
    {
        // Ensure that only one instance exists
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);  // Destroy duplicates
            Debug.LogWarning("An instance of TutorialSectionsManager already exists. Deleting duplicate.");
        }
        else
        {
            _instance = this;  // Set this as the instance
        }
    }

    private void Start()
    {
        if (startOnPlay)
        {
            StartTutorial();
        }
    }

    public async void StartTutorial()
    {
        try
        {
            // Create a new CancellationTokenSource for the tutorial
            tutorialCancellationTokenSource = new CancellationTokenSource();

            // Link the tutorial cancellation token with the application exit cancellation token
            CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                tutorialCancellationTokenSource.Token,
                Application.exitCancellationToken
            ).Token;

            await StartTutorialLoopAsync(linkedToken);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Tutorial Cancelled.");
        }
        catch (Exception ex)
        {
            // Catch any other general exceptions and log the error
            Debug.LogError($"An error occurred while running the tutorial: {ex.Message}");
            Debug.LogError($"Stack Trace: {ex.StackTrace}");
        }
    }

    private async Task StartTutorialLoopAsync(CancellationToken token)
    {
        OnStart.Invoke();
        Started = true;

        for (int i = 0; i < sections.Length; i++)
        {
            currentSectionIndex = i;

            // Check for cancellation before proceeding
            token.ThrowIfCancellationRequested();

            var taskTimes = await sections[currentSectionIndex].StartAsync(token);

            token.ThrowIfCancellationRequested();

            allTaskTimes[sections[currentSectionIndex].name] = taskTimes;
            OnSectionChange?.Invoke();
        }

        tutorialCompleted = true;
        SaveTaskTimesToFile();
    }

    private bool TutorialCompleted()
    {
        if (currentSectionIndex >= sections.Length - 1)
        {
            return true;
        }

        return false;
    }

    public void SkipCurrentTask()
    {
        if (tutorialCompleted == false)
        {
            if (currentSectionIndex < 0 || currentSectionIndex >= sections.Length)
            {
                UnityEngine.Debug.LogWarning($"Current section index is out of bounds: {currentSectionIndex}");
                return;
            }

            sections[currentSectionIndex].SkipCurrentTask();
        }
    }

    // Cancel the tutorial if it's running
    public void CancelTutorial()
    {
        if (tutorialCancellationTokenSource != null)
        {
            Debug.Log("Tutorial cancellation called");
            tutorialCancellationTokenSource.Cancel();
        }
    }

    void SkipCurrentSection()
    {
        if (tutorialCompleted == false)
        {
            sections[currentSectionIndex].SkipSection();
        }
    }

    // Save all task times to a single JSON file
    private void SaveTaskTimesToFile()
    {
        if (trackAnalytics == false)
            return;

        // Get the current timestamp formatted as "yyyy-MM-dd_HH-mm" (year, month, day, hour, minute)
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");

        // Append the timestamp to the filename
        string fileName = $"tutorial_task_times_{timestamp}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            // Serialize the task times dictionary to JSON
            string json = JsonConvert.SerializeObject(allTaskTimes, Formatting.Indented);

            File.WriteAllText(filePath, json);

            Debug.Log($"All task times saved to {filePath}");
        }
        catch (Exception ex)
        {
            // Log the error message if there was an issue
            Debug.LogError($"Failed to save task times: {ex.Message}");
        }
    }
}
