using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using static VRHeadsetDetector;

public class LeanForwardDetection : MonoBehaviour
{
    public float leanThresholdAngle = 5.5f;  // The angle threshold to detect the lean

    private Vector3 smoothedHeadPosition;
    public CharacterController characterController;

    private string filePath;

    public bool IsLeanedForward
    {
        get
        {
            // Calculate the current forward direction of the head
            //Vector3 currentPosition = Camera.main.transform.position;

            //// Calculate the displacement from the initial position
            //Vector3 displacement = currentPosition - initialPosition;

            //// Project the displacement onto the headset's forward direction
            //Vector3 forwardDisplacement = Vector3.Project(displacement, Camera.main.transform.forward);

            //// Calculate the magnitude of the displacement and forward displacement
            //float totalDisplacementMagnitude = displacement.magnitude;
            //float forwardDisplacementMagnitude = forwardDisplacement.magnitude;

            //// Calculate the angle of lean
            //float leanAngle = Mathf.Atan2(forwardDisplacementMagnitude, totalDisplacementMagnitude) * Mathf.Rad2Deg;

            return Vector3.Angle(transform.position, Camera.main.transform.position) >= leanThresholdAngle;
        }
    }

    public void SetStartingPosition()
    {
        // Store the initial upward direction, which is generally Vector3.up
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");

        // Append the timestamp to the file name
        filePath = Application.persistentDataPath + "/LeanData_" + timestamp + ".txt";

        transform.position = Camera.main.transform.position;
        InvokeRepeating(nameof(RecordLeanData), 1f, 1f);
    }

    void RecordLeanData()
    {
        // Get the current headset (camera) position
        Vector3 currentPosition = Camera.main.transform.localPosition;

        // Calculate the angle of lean
        float leanAngle = Vector3.Angle(transform.localPosition, currentPosition);

       
        // Write the data to a file
        WriteDataToFile(leanAngle, currentPosition);
    }

    void WriteDataToFile(float angle, Vector3 position)
    {
        // Open the file for writing, or create it if it doesn't exist
        using StreamWriter writer = new(filePath, true);
        writer.WriteLine($"Speed: {characterController.velocity} Lean Angle: {angle}, Position: {position.x}, {position.y}, {position.z}, Initial position: {transform.position.x}, {transform.position.y}, {transform.position.z}");

        // Log the data
        Debug.Log($"Speed: {characterController.velocity} Lean Angle: {angle}, Position: {position.x}, {position.y}, {position.z}, Initial position: {transform.position.x}, {transform.position.y}, {transform.position.z}");
    }

    //private void OnEnable()
    //{
    //    initialPosition = Camera.main.transform.position;
    //    Debug.Log(initialPosition);
    //}


    //void Update()
    //{
    //    if (initialPosition == Vector3.zero)
    //    {
    //        initialPosition = Camera.main.transform.position;
    //        Debug.Log(initialPosition);
    //    }

    //    //// Calculate the current position of the camera
    //    //Vector3 currentPosition = Camera.main.transform.position;

    //    //// Calculate the displacement from the initial position
    //    //Vector3 displacement = currentPosition - initialPosition;

    //    //// Project the displacement onto the camera's forward direction
    //    //Vector3 forwardDisplacement = Vector3.Project(displacement, Camera.main.transform.forward);

    //    //// Calculate the magnitude of the displacement and forward displacement
    //    //float totalDisplacementMagnitude = displacement.magnitude;
    //    //float forwardDisplacementMagnitude = forwardDisplacement.magnitude;

    //    //// Avoid division by zero
    //    //if (totalDisplacementMagnitude == 0)
    //    //{
    //    //    Debug.Log("No displacement detected.");
    //    //    return;
    //    //}

    //    //// Calculate the angle of lean in degrees
    //    //float leanAngle = Mathf.Atan2(forwardDisplacementMagnitude, totalDisplacementMagnitude) * Mathf.Rad2Deg;

    //    //Debug.Log("Lean Angle: " + leanAngle);

    //    // Calculate the forward bend angle
    //    //float angle = CalculateForwardBendAngle(initialPosition, Camera.main.transform.position);
    //    //Debug.Log($"Forward Bend Angle: {angle} degrees");

    //    //smoothedHeadPosition = Vector3.Lerp(smoothedHeadPosition, Camera.main.transform.position, 0.5f);
    //    //float angle = CalculateForwardBendAngle(initialPosition, Camera.main.transform.position);
    //    //Debug.Log($"Forward Bend Angle: {angle} degrees");
    //}

    //private float CalculateForwardBendAngle(Vector3 initial, Vector3 current)
    //{
    //    return Vector3.Angle(initial, current);
    //}

}
