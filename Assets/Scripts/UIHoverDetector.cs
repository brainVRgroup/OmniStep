using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverDuration = 0.2f; // Set the hover duration in seconds
    private bool isHovering = false;
    private float hoverTime = 0f;

    // Event to notify when hover duration is reached
    public UnityEvent OnHoverDurationReached;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        hoverTime = 0f; // Reset the hover time
        StartCoroutine(HoverCoroutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    private IEnumerator HoverCoroutine()
    {
        while (isHovering)
        {
            hoverTime += Time.deltaTime;
            if (hoverTime >= hoverDuration)
            {
                OnHoverDurationReached?.Invoke();
                isHovering = false; // Stop checking hover after duration is reached
            }
            yield return null;
        }
    }
}
