using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class AdminMenuController : MonoBehaviour
{
    [SerializeField] Button skipButton;
    [SerializeField] Button startButton;

    ReloadManager reloadManager;

    private void Start()
    {
        reloadManager = FindObjectOfType<ReloadManager>();

        if (TutorialSectionsManager.Instance.Started)
        {
            startButton.interactable = false;
            skipButton.interactable = true;
        }
        else
        {
            TutorialSectionsManager.Instance.OnStart.AddListener(() => startButton.interactable = false);
            TutorialSectionsManager.Instance.OnStart.AddListener(() => skipButton.interactable = true);
        }    
    }

    public void ResetScene()
    {
        reloadManager.ReloadScene();
    }
}
