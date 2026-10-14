using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCloserFartherToPlayer : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 10f;

    [Header("Smoothness")]
    [SerializeField] private float smoothTime = 0.2f;

    private Transform playerCamera;
    private float currentDistance;
    private float targetDistance;
    private float distanceVelocity;

    void Start()
    {
        if (Camera.main == null) return;

        playerCamera = Camera.main.transform;

        // Get the object's initial distance from the player
        currentDistance = Vector3.Distance(
            playerCamera.position,
            transform.position
        );

        currentDistance = Mathf.Clamp(
            currentDistance, minDistance, maxDistance
        );

        targetDistance = currentDistance;
    }

    void Update()
    {
        if (playerCamera == null) return;

        // Use the mouse scroll wheel without clicking
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            targetDistance -= scroll * scrollSpeed * 5f;

            targetDistance = Mathf.Clamp(
                targetDistance,
                minDistance,
                maxDistance
            );
        }

        // Smoothly transition to the target distance
        currentDistance = Mathf.SmoothDamp(
            currentDistance,
            targetDistance,
            ref distanceVelocity,
            smoothTime
        );

        // Move the object along the camera's forward direction
        transform.position = playerCamera.position
                           + playerCamera.forward * currentDistance;
    }
}
