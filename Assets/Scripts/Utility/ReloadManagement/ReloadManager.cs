using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class ReloadManager : MonoBehaviour
{
    public bool IsReloaded { get => reloaded; } 

    bool reloaded = false;

    void Awake()
    {
        if (FindObjectsOfType<ReloadManager>().Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }


        DontDestroyOnLoad(this.gameObject);
    }

    public void ReloadScene()
    {
        reloaded = true;
        TutorialSectionsManager.Instance.CancelTutorial();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
