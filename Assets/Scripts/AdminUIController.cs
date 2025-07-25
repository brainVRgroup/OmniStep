using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class AdminUIController : MonoBehaviour
{
    public GameObject EscapeMenu;
    public InputActionProperty EscapeMenuAction;

    void Update()
    {
        if (EscapeMenuAction.action.triggered)
        {
            EscapeMenu.SetActive(!EscapeMenu.activeInHierarchy);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
