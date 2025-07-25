using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateControllerOnInput : MonoBehaviour
{
    public InputActionProperty grabAnimationAction;

    public Animator controllerAnimator;

    // Update is called once per frame
    void Update()
    {
        float grabValue = grabAnimationAction.action.ReadValue<float>();
        controllerAnimator.SetFloat("Grip", grabValue);
    }
}
