using UnityEngine;

[RequireComponent (typeof(Mesh))]
public class ColorIntensityBlinkingEffect : MonoBehaviour
{
    [SerializeField] float blinkSpeed = 2f; // Speed of the blinking effect
    [SerializeField] float minIntensity = 0.5f; // Minimum intensity
    [SerializeField] float maxIntensity = 2f; // Maximum intensity

    Color baseColor = Color.white; // Base color of the material
    Material material;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
            
        // Use the first material on the MeshRenderer
        material = renderer.material;
        baseColor = material.GetColor("_Color");
    }

    private void Update()
    {
        // Calculate intensity based on time (using a sine wave)
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f);

        // Adjust the intensity of the color
        Color newColor = baseColor * intensity;

        // Set the color to the material's shader graph property "_Color"
        material.SetColor("_Color", newColor);
    }
}
