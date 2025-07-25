using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class OnReloadAction : MonoBehaviour
{
    public UnityEvent OnReload;

    void Start()
    {
        var reloadManagers = FindObjectsByType<ReloadManager>(FindObjectsSortMode.None);
        if (reloadManagers.Any(manager => manager.IsReloaded))
            OnReload.Invoke();
    }

}
