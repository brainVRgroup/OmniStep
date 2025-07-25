using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable 

public class GazeInteractor : MonoBehaviour
{
    [SerializeField] private float _maxDistance;
    [SerializeField] private LayerMask _layerMask;

    private Ray _ray;

    private GazeInteractable? _lastDetectedInteractable;


    // Detect whether an interactable is in gaze
    void Update()
    {
        _ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(_ray, out RaycastHit detectedObject, _maxDistance, _layerMask))
        {
            var interactable = detectedObject.collider.GetComponent<GazeInteractable>();

            if (_lastDetectedInteractable != interactable)
            {
                _lastDetectedInteractable?.GazeExit();
                _lastDetectedInteractable = null;
            }

            if (interactable != null)
            {
                interactable.GazeEnter();
                _lastDetectedInteractable = interactable;
            }
        }
    }
}
