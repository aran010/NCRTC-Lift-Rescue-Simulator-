using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;
    public bool enableMovement = true;

    [Header("Camera Settings")]
    public Camera playerCamera;

    [Header("Interaction Settings")]
    public float interactionRange = 5f;
    public LayerMask interactionMask = -1; // All layers by default
    public Color normalCrosshairColor = Color.white;
    public Color hoverCrosshairColor = Color.green;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private bool cursorLocked = false;
    private float verticalVelocity = 0f;
    private float gravity = -9.81f;
    
    // Interaction
    private GameObject currentHoveredObject = null;

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

        // Lock cursor on mouse click (only if not hovering interactive object)
        if (Input.GetMouseButtonDown(0) && !cursorLocked && currentHoveredObject == null)
        {
            LockCursor();
        }

        HandleMovement();
        
        if (cursorLocked)
        {
            HandleMouseLook();
            HandleInteraction();
        }
    }

    void HandleMovement()
    {
        // WASD Movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * moveSpeed * Time.deltaTime);

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

    void HandleInteraction()
    {
        if (playerCamera == null) return;

        // Cast ray from center of screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactionMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            
            // Check if object is interactable
            if (IsInteractable(hitObject))
            {
                currentHoveredObject = hitObject;
                
                // Click to interact
                if (Input.GetMouseButtonDown(0))
                {
                    TriggerInteraction(hitObject);
                }
            }
            else
            {
                currentHoveredObject = null;
            }
        }
        else
        {
            currentHoveredObject = null;
        }
    }

    bool IsInteractable(GameObject obj)
    {
        // Check for XR Simple Interactable
        if (obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>() != null)
            return true;
        
        // Check for XR Grab Interactable
        if (obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() != null)
            return true;

        // Any object with a collider can potentially be interacted with
        return obj.GetComponent<Collider>() != null;
    }

  // Replace the TriggerInteraction method in FirstPersonController.cs with this:

    void TriggerInteraction(GameObject obj)
    {
        Debug.Log($"Clicked on: {obj.name}");

        // Get LiftRescueManager
        var manager = LiftRescueManager.Instance;
        if (manager == null)
        {
            manager = FindObjectOfType<LiftRescueManager>();
        }

        if (manager != null)
        {
            string objName = obj.name.ToLower();

            // Check object name and call appropriate method
            if (objName.Contains("ei") || objName.Contains("earthing"))
            {
                manager.OnEIPanelOpened();
            }
            else if (objName.Contains("main") || objName.Contains("power"))
            {
                manager.OnMainPowerPanelOpened();
            }
            else if (objName.Contains("ocb"))
            {
                manager.OnOCBSwitchedOff();
            }
            else if (objName.Contains("rps"))
            {
                manager.OnRPSChecked();
            }
            else if (objName.Contains("rotary") || objName.Contains("knob"))
            {
                // Check current step to decide which direction
                if (manager.currentStep == LiftRescueManager.RescueStep.Step4_RotaryToMRO)
                    manager.OnRotaryToMRO();
                else if (manager.currentStep == LiftRescueManager.RescueStep.Step8_RotaryToNormal)
                    manager.OnRotaryToNormal();
            }
            else if (objName.Contains("up") || objName.Contains("spb"))
            {
                manager.OnUpButtonPressed();
            }
            else if (objName.Contains("key1") || objName.Contains("key_1"))
            {
                manager.OnKey1ButtonPressed();
            }
            else if (objName.Contains("f5c") || objName.Contains("fs2"))
            {
                manager.OnF5CSwitchedOff();
            }
            else if (objName.Contains("door") || objName.Contains("lift"))
            {
                manager.OnDoorsOpened();
            }
        }

        // Also try SendMessage as fallback
        obj.SendMessage("Toggle", SendMessageOptions.DontRequireReceiver);
        obj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
    }

    // Draw crosshair on screen
    void OnGUI()
    {
        if (!cursorLocked || !enableMovement) return;

        // Crosshair size
        int size = 20;
        int thickness = 2;
        
        // Center of screen
        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;

        // Color based on hover state
        Color crosshairColor = currentHoveredObject != null ? hoverCrosshairColor : normalCrosshairColor;
        
        // Create texture for crosshair
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, crosshairColor);
        texture.Apply();

        // Draw horizontal line
        GUI.DrawTexture(new Rect(centerX - size/2, centerY - thickness/2, size, thickness), texture);
        
        // Draw vertical line
        GUI.DrawTexture(new Rect(centerX - thickness/2, centerY - size/2, thickness, size), texture);

        // Show object name when hovering
        if (currentHoveredObject != null)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = hoverCrosshairColor;
            style.fontSize = 16;
            style.alignment = TextAnchor.MiddleCenter;
            
            GUI.Label(new Rect(centerX - 100, centerY + 30, 200, 30), 
                      "[Click] " + currentHoveredObject.name, style);
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

