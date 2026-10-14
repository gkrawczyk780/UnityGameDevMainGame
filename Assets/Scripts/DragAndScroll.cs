using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndScroll : MonoBehaviour
{
    private Rigidbody rb;
    private Collider objectCollider;
    private Collider playerCollider;
    private Transform playerCamera;

    private bool isDragging;
    private bool wasKinematic;
    private bool wasUsingGravity;

    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 3f;

    [Header("Smoothness")]
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Collision Settings")]
    [SerializeField] private float collisionPadding = 0.02f;
    [SerializeField] private LayerMask collisionMask = ~0;

    private float currentDistance;
    private float targetDistance;
    private float distanceVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();

        if (Camera.main != null)
            playerCamera = Camera.main.transform;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerCollider = player.GetComponent<Collider>();
    }

    private void OnMouseDown()
    {
        if (rb == null || playerCamera == null)
            return;

        isDragging = true;

        // Remember the original physics settings.
        wasKinematic = rb.isKinematic;
        wasUsingGravity = rb.useGravity;

        rb.isKinematic = true;
        rb.useGravity = false;

        if (playerCollider != null && objectCollider != null)
        {
            Physics.IgnoreCollision(
                objectCollider, playerCollider, true
            );
        }

        // Measure distance along the camera's forward direction.
        currentDistance = Vector3.Dot(
            transform.position - playerCamera.position,
            playerCamera.forward
        );

        currentDistance = Mathf.Clamp(
            currentDistance, minDistance, maxDistance
        );

        targetDistance = currentDistance;
        distanceVelocity = 0f;
    }

    private void Update()
    {
        if (!isDragging || playerCamera == null)
            return;

        // Scroll only while holding the object.
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        targetDistance -= scroll * scrollSpeed * 5f;

        targetDistance = Mathf.Clamp(
            targetDistance, minDistance, maxDistance
        );

        // Smooth the distance change.
        currentDistance = Mathf.SmoothDamp(
            currentDistance,
            targetDistance,
            ref distanceVelocity,
            smoothTime
        );

        // Follow the mouse while keeping the chosen distance.
        Ray mouseRay = Camera.main.ScreenPointToRay(
            Input.mousePosition
        );

        Vector3 desiredPosition = mouseRay.GetPoint(currentDistance);

        // Move toward the target without sweeping through colliders.
        Vector3 movement = desiredPosition - rb.position;
        float distance = movement.magnitude;

        if (distance > 0.001f)
        {
            Vector3 direction = movement / distance;
            float allowedDistance = distance;

            if (rb.SweepTest(
                direction,
                out RaycastHit hit,
                distance + collisionPadding,
                QueryTriggerInteraction.Ignore))
            {
                allowedDistance = Mathf.Max(
                    0f, hit.distance - collisionPadding
                );
            }

            Vector3 safePosition = rb.position
                                 + direction * allowedDistance;

            rb.MovePosition(safePosition);
        }
    }

    private void OnMouseUp()
    {
        if (!isDragging || rb == null)
            return;

        isDragging = false;

        if (playerCollider != null && objectCollider != null)
        {
            Physics.IgnoreCollision(
                objectCollider, playerCollider, false
            );
        }

        // Restore the object's original physics settings.
        rb.isKinematic = wasKinematic;
        rb.useGravity = wasUsingGravity;

        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
