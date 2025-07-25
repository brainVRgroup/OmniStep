using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProgressFiller : MonoBehaviour
{
    float fillSpeed;
    Material objectMaterial = null;

    float _FillRateValue;
    bool fillStarted = false;
    float height;

    void Initialize()
    {
        objectMaterial = new Material(Resources.Load<Material>("SpriteFillMat"));

        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.material = objectMaterial;

        height = spriteRenderer.bounds.size.y * transform.localScale.y;

        _FillRateValue = (height / 2) * -1;
        objectMaterial.SetFloat("_FillHeight", _FillRateValue);
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (fillStarted)
        {
            _FillRateValue += fillSpeed * Time.deltaTime;
            objectMaterial.SetFloat("_FillHeight", _FillRateValue);

            if (_FillRateValue >= height)
            {
                fillStarted = false;
            }
        }
        
    }

    public void StartFill(int remainingTime)
    {
        if (objectMaterial == null)
        {
            Initialize();
        }

        fillSpeed = height / remainingTime;
        fillStarted = true;
    }

    public void StopFill()
    {
        fillStarted = false;
    }
}
