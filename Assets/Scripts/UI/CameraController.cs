using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private bool followTarget = true;

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Manual Movement")]
    [SerializeField] private float manualMoveSpeed = 10f;
    [SerializeField] private KeyCode toggleFollowKey = KeyCode.T;
    [SerializeField] private float returnToPlayerSpeed = 10f;
    [SerializeField] private KeyCode returnToPlayerKey = KeyCode.R;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 3f;  // Higher zoom level (closer)
    [SerializeField] private float maxZoom = 10f; // Lower zoom level (farther)
    [SerializeField] private float defaultZoom = 5f;

    [Header("Map Boundaries")]
    [SerializeField] private float minX = -105.4f;
    [SerializeField] private float maxX = 75.2f;
    [SerializeField] private float minY = -61f;
    [SerializeField] private float maxY = 38.4f;

    private Camera cam;
    private Vector3 targetPosition;
    private Vector3 lastTargetPosition;
    private float currentZoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        currentZoom = defaultZoom;
    }

    private void Start()
    {
        if (target == null)
        {
            // Try to find player if not assigned
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("CameraController: No target assigned!");
            }
        }

        // Set initial position
        if (target != null)
        {
            targetPosition = target.position + offset;
            lastTargetPosition = target.position;
            transform.position = targetPosition;
        }

        // Set initial zoom
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = currentZoom;
        }

        // Find and disable any GridManager.SetupCamera functionality to avoid conflicts
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            Debug.Log("CameraController: Found GridManager - will handle camera positioning");
            // We don't need to do anything here since our camera controller will handle positioning
        }
    }

    private void Update()
    {
        HandleInput();
        HandleZoom();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Update target position if following
        if (followTarget)
        {
            targetPosition = target.position + offset;
            lastTargetPosition = target.position;
        }

        // Apply bounds to target position
        ClampTargetToBounds();

        // Move camera towards target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void HandleInput()
    {
        // Toggle follow mode
        if (Input.GetKeyDown(toggleFollowKey))
        {
            followTarget = !followTarget;
            if (followTarget)
            {
                Debug.Log("Camera is now following player");
            }
            else
            {
                Debug.Log("Manual camera control enabled");
            }
        }

        // Return to player
        if (Input.GetKeyDown(returnToPlayerKey) && !followTarget && target != null)
        {
            followTarget = true;
            Debug.Log("Returning to player");
        }

        // Manual camera movement when not following
        if (!followTarget)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                Vector3 moveDirection = new Vector3(horizontal, vertical, 0);
                targetPosition += moveDirection * manualMoveSpeed * Time.deltaTime;
                ClampTargetToBounds();
            }
        }
    }

    private void HandleZoom()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            // Adjust zoom based on scroll direction
            currentZoom -= scrollDelta * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            // Apply new zoom level
            if (cam != null && cam.orthographic)
            {
                cam.orthographicSize = currentZoom;
            }
        }
    }

    private void ClampTargetToBounds()
    {
        // Calculate half screen dimensions in world space
        float vertExtent = currentZoom;
        float horizExtent = vertExtent * Screen.width / Screen.height;

        // Clamp position within bounds while accounting for camera view size
        float clampedX = Mathf.Clamp(targetPosition.x, minX + horizExtent, maxX - horizExtent);
        float clampedY = Mathf.Clamp(targetPosition.y, minY + vertExtent, maxY - vertExtent);

        targetPosition = new Vector3(clampedX, clampedY, targetPosition.z);
    }

    // Method to update the map boundaries (useful if you need to change them at runtime)
    public void UpdateMapBoundaries(float newMinX, float newMaxX, float newMinY, float newMaxY)
    {
        minX = newMinX;
        maxX = newMaxX;
        minY = newMinY;
        maxY = newMaxY;
    }

    // Method to force the camera to return to the player
    public void ReturnToPlayer()
    {
        if (target != null)
        {
            followTarget = true;
            targetPosition = target.position + offset;
        }
    }

    // Editor method to help visualize the boundaries
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));
        Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
        Gizmos.DrawLine(new Vector3(maxX, maxY, 0), new Vector3(minX, maxY, 0));
        Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(minX, minY, 0));
    }
}