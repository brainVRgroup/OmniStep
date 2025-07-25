using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
public class DissolvableObject : MonoBehaviour
{
    [SerializeField] Material materialSource;
    [SerializeField] Color edgeColor = new Color(100, 255, 255);
    [SerializeField] Color baseColor = new Color(100, 255, 255);
    [SerializeField] float edgeThickness = 0.03f;
    [SerializeField] float noiseScale = 30f;

    [SerializeField] UnityEvent OnAppear;
    [SerializeField] UnityEvent OnDissolve;

    Material material;
    MeshRenderer meshRenderer;

    public void Dissolve()
    {
        meshRenderer.material = material;
        StartCoroutine(RunDissolve());
    }

    public void Appear()
    {
        if (gameObject.transform.parent.gameObject.activeSelf)
            return;

        meshRenderer = GetComponent<MeshRenderer>();

        material = new Material(materialSource);
        //material = new Material(Shader.Find("Shader Graphs/Dissolve")); //creating a material with the shader
        material.SetColor("_EdgeColor", edgeColor);
        material.SetFloat("_EdgeThickness", edgeThickness);
        material.SetFloat("_NoiseScale", noiseScale);
        material.SetFloat("_DissolveControl", 1);
        material.SetColor("_BaseColor", baseColor);


        meshRenderer.material = material;

        // For cube TODO fix
        gameObject.transform.parent.gameObject.SetActive(true);
        StartCoroutine(RunAppear());


    }

    IEnumerator RunDissolve()
    {
        var increment = 0.01f;

        var currentValue = -1f;
        while (currentValue < 1)
        {
            currentValue += increment;
            material.SetFloat("_DissolveControl", currentValue);
            yield return null;
        }

        OnDissolve.Invoke();
        gameObject.SetActive(false);
    }

    IEnumerator RunAppear()
    {
        var increment = 0.01f;

        var currentValue = 1f;
        while (currentValue > -1)
        {
            currentValue -= increment;
            material.SetFloat("_DissolveControl", currentValue);
            yield return null;
        }

        OnAppear.Invoke();
    }
}
