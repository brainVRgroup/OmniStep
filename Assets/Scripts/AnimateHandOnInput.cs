using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty grabAnimationAction;
    public InputActionProperty trackpadAnimationAction;

    public Animator handAnimator;

    // Update is called once per frame
    void Update()
    {
        float pinchValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", pinchValue);

        float grabValue = grabAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", grabValue);

        Vector2 trackpadValue = trackpadAnimationAction.action.ReadValue<Vector2>();
        handAnimator.SetFloat("TrackpadX", trackpadValue.x);
        handAnimator.SetFloat("TrackpadY", trackpadValue.y);

        //var touchingTrackpad = handAnimator.GetBool("TouchingTrackpad");

        if (trackpadValue.x == 0 && trackpadValue.y == 0)
        {
            //StartCoroutine(TrackpadTouchDetection());
            handAnimator.SetBool("TouchingTrackpad", false);
        }
        else
        {
            handAnimator.SetBool("TouchingTrackpad", true);
        }

    }

    IEnumerator TrackpadTouchDetection()
    {
        yield return new WaitForSeconds(0.05f);

        Vector2 trackpadValue = trackpadAnimationAction.action.ReadValue<Vector2>();

        if (trackpadValue.x == 0 && trackpadValue.y == 0)
        {
            handAnimator.SetBool("TouchingTrackpad", false);
        }
    }
}
