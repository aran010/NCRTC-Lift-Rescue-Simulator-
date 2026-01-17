using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;
    public bool enableMovement = true;

    [Header("Camera Settings")]
    public Camera playerCamera;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private bool cursorLocked = false;
    private float verticalVelocity = 0f;
    private float gravity = -9.81f;

    void Start()
    {
        // Add CharacterController if not present
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.3f;
            controller.center = new Vector3(0, 0.9f, 0);
        }

        // Find camera if not assigned
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
    }

    void Update()
    {
        if (!enableMovement) return;

        // Toggle cursor lock with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }

        // Lock cursor on mouse click
        if (Input.GetMouseButtonDown(0) && !cursorLocked)
        {
            LockCursor();
        }

        HandleMovement();
        
        if (cursorLocked)
        {
            HandleMouseLook();
        }
    }

    void HandleMovement()
    {
        // WASD Movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * moveSpeed * Time.deltaTime);
        // DEBUG: Check if grounded
Debug.Log($"Is Grounded: {controller.isGrounded}, Pos Y: {transform.position.y:F2}, Velocity: {verticalVelocity:F2}");
        

        // Apply gravity properly
        if (controller.isGrounded)
        {
            // Reset vertical velocity when grounded
            verticalVelocity = -2f; // Small downward force to keep grounded
        }
        else
        {
            // Apply gravity when in air
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Apply vertical movement
        Vector3 verticalMove = new Vector3(0, verticalVelocity, 0);
        controller.Move(verticalMove * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate body left/right
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }

    public void ToggleCursorLock()
    {
        if (cursorLocked)
            UnlockCursor();
        else
            LockCursor();
    }

    public void EnableMovement()
    {
        enableMovement = true;
    }

    public void DisableMovement()
    {
        enableMovement = false;
    }
}

