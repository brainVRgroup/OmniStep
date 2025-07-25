using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameAdminMenuController : MonoBehaviour
{
    public InputActionProperty leftControllerMenuClick;
    public InputActionProperty rightControllerMenuClick;

    public GameObject leftControllerAdminMenu;
    public GameObject rightControllerAdminMenu;

    private void Update()
    {
        // Check for button presses
        if (leftControllerMenuClick.action.triggered)
        {
            leftControllerAdminMenu.SetActive(!leftControllerAdminMenu.activeSelf);
            rightControllerAdminMenu.SetActive(false);
        }

        if (rightControllerMenuClick.action.triggered)
        {
            rightControllerAdminMenu.SetActive(!rightControllerAdminMenu.activeSelf);
            leftControllerAdminMenu.SetActive(false);
        }
    }

}
