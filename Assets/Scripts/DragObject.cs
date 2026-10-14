using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Rigidbody rb;
    private float zOffset;
    private Collider objectCollider;

    private Collider playerCollider;
    private bool wasKinematic;

    void Start()
    {
        // Cache the Rigidbody reference
        rb = GetComponent<Rigidbody>();

        objectCollider = GetComponent<Collider>();

        // Store the Z distance from camera to object
        zOffset = Camera.main.WorldToScreenPoint(transform.position).z;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            playerCollider = player.GetComponent<Collider>();
        }
    }

    private void OnMouseDrag()
    {
        if (rb == null) return;

        // Make object kinematic while dragging
        if (!rb.isKinematic)
        {
            wasKinematic = false;
            rb.isKinematic = true;
        }
        if (playerCollider != null)
        {
            Physics.IgnoreCollision(objectCollider, playerCollider, true);
        }

        // Get mouse position in screen space
        Vector3 mousePosition = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            zOffset // Keep same depth
        );

        // Convert to world space
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        GameObject platform = GameObject.FindGameObjectWithTag("Platform");

        if (platform != null)
        {
            Collider platformCollider = platform.GetComponent<Collider>();

            float platformTop = platformCollider.bounds.max.y;
            float objectHalfHeight = objectCollider.bounds.extents.y;

            // Prevent the bottom of the object from going below the platform.
            float minimumY = platformTop + objectHalfHeight;

            if (objPosition.y < minimumY)
            {
                objPosition.y = minimumY;
            }
        }

        // Move object
        rb.MovePosition(objPosition);
    }

    private void OnMouseUp()
    {
        if (rb == null) return;

        if (playerCollider != null)
        {
            Physics.IgnoreCollision(objectCollider, playerCollider, false);
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Re-enable physics
        rb.isKinematic = false;
    }
}
