using UnityEngine;

public class ColorLerper : MonoBehaviour
{
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;
    [SerializeField] float speed;

    MeshRenderer meshRenderer;
    bool isLerping = false;
    float startTime;

    bool lerpCompleted = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (isLerping)
        {
            meshRenderer.material.color = Color.Lerp(startColor, endColor, (Time.time - startTime) * speed);

            if (meshRenderer.material.color == endColor)
            {
                isLerping = false;
                lerpCompleted = true;
            }
        }
        
    }
    public void LerpColor()
    {
        if (isLerping || lerpCompleted)
            return;

        startTime = Time.time;
        isLerping = true;
    }

    public void CancelLerp()
    {
        if (lerpCompleted)
            return;

        isLerping = false;
        meshRenderer.material.color = startColor;
    }

}
