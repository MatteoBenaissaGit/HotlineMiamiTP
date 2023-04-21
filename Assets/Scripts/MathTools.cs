using MatteoBenaissaLibrary.TopDownCharacterController;
using UnityEngine;

public class MathTools
{
    public static Vector3 GetDirectionToPointCameraLooking(Transform transformOrigin, float distanceFromCamera)
    {
        // Get the position of the player
        Vector3 playerPos = transformOrigin.position;

        // Get the position and forward direction of the camera
        Camera mainCamera = Camera.main;
        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 cameraForward = mainCamera.transform.forward;

        // Calculate the position of the point at the specified distance from the camera along its forward direction
        Vector3 pointPos = cameraPos + cameraForward * distanceFromCamera;

        // Calculate the direction from the player to the point
        Vector3 direction = (pointPos - playerPos).normalized;

        return direction;
    }

    public static Vector3 GetVector3DirectionToPlayerLooking(Transform playerTransform)
    {
        float angle = TopDownCharacterController.GetDirectionToMousePositionFromPlayer(playerTransform);
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
    }
    
    public static Vector2 GetPointAtDistanceAndAngle(Vector2 startPoint, float distance, float angleDegrees)
    {
        // Convert the angle to radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Calculate the X and Y components of the new point using trigonometry
        float newX = startPoint.x + distance * Mathf.Cos(angleRadians);
        float newY = startPoint.y + distance * Mathf.Sin(angleRadians);

        // Create and return a new vector representing the new point
        return new Vector2(newX, newY);
    }

}