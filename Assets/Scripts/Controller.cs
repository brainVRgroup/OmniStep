using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour
{
    public UnityEvent OnTooltipSeen;

    [SerializeField] MeshRenderer selectButton;
    [SerializeField] MeshRenderer gripButton;
    [SerializeField] MeshRenderer trackpad;
    [SerializeField] GameObject tooltip;
    [SerializeField] GazeInteractable tooltipGazeInteractable;

    private Material defaultSelectMaterial;
    private Material defaultGripMaterialMaterial;

    private void Start()
    {
        defaultSelectMaterial = selectButton.material;
        defaultGripMaterialMaterial = gripButton.material;
    }

    public void HighlightSelectButon(Material highlightMaterial)
    {
        selectButton.material = highlightMaterial;
    }

    public void HighlightGripButton(Material highlightMaterial)
    {
        gripButton.material = highlightMaterial;

        // If tooltip provided
        if (tooltip)
        {
            tooltip.SetActive(true);
            tooltipGazeInteractable.OnGazeCompleted.AddListener(GazeCompletedAction);
        }
      
    }

    public void UnhighlightGripButton()
    {
        gripButton.material = defaultGripMaterialMaterial;
        
        // If tooltip provided
        if (tooltip)
        {
            tooltip.SetActive(false);
            tooltipGazeInteractable.OnGazeCompleted.RemoveListener(GazeCompletedAction);
        }
    
    }

    public void HighlightTrackpad(Material highlightMaterial)
    {
        trackpad.material = highlightMaterial;

    }

    public void UnhighlightTrackpad()
    {
        trackpad.material = defaultGripMaterialMaterial;
    }

    private void GazeCompletedAction()
    {
        OnTooltipSeen?.Invoke();
    }

}
