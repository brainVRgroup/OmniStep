using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(MeshRenderer))]
public class Outline : MonoBehaviour
{
    [SerializeField] XRGrabInteractable interactable;
    [SerializeField] Material outlineMaterial;
    [SerializeField] Color outlineColor = Color.yellow;
    [SerializeField] float thickness = 1.05f;

    MeshRenderer meshRenderer;
    List<Material> originalMaterials;

    bool outlined = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        //outlineMaterial = Resources.Load<Material>("Materials/OutlineMaterial");
        outlineMaterial.SetColor("_Color", outlineColor);
        outlineMaterial.SetFloat("_Thickness", thickness);
    }

    void OnEnable()
    {
        interactable.firstHoverEntered.AddListener((HoverEnterEventArgs _) => ShowOutline());
        interactable.lastHoverExited.AddListener((HoverExitEventArgs _) => HideOutline());

        interactable.firstSelectEntered.AddListener((SelectEnterEventArgs _) => HideOutline());
        interactable.lastSelectExited.AddListener((SelectExitEventArgs _) => ShowOutline());
    }


    public void ShowOutline()
    {
        if (outlined)
            return;

        outlined = true;
        originalMaterials = meshRenderer.materials.ToList();
        meshRenderer.SetMaterials(new List<Material> { originalMaterials.First(), outlineMaterial });
        gameObject.layer = LayerMask.NameToLayer("Outlined");
        
    }

    public void HideOutline()
    {
        if (!outlined)
            return;

        outlined = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
        meshRenderer.SetMaterials(originalMaterials);
    }
}
