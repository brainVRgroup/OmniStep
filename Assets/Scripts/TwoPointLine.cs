using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class TwoPointLine : MonoBehaviour
{
    [SerializeField] Transform[] linePoints;

    LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = linePoints.Length;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = 0; i < linePoints.Length; i++)
        {
            lineRenderer.SetPosition(i, linePoints[i].position);
        }
    }
}
