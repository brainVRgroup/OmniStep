using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorEvents : MonoBehaviour
{
    public UnityEvent OnAnimationStart;
    public UnityEvent OnAnimationEnd;

    public void OnStart()
    {
        OnAnimationStart?.Invoke();
    }

    public void OnEnd()
    {
        OnAnimationEnd?.Invoke();
    }
}
