using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class LiftSlidingDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("The door panel that slides")]
    public Transform doorPanel;
    
    [Tooltip("Position when door is open")]
    public Vector3 openPosition;
    
    [Tooltip("Position when door is closed")]
    public Vector3 closedPosition;
    
    [Tooltip("Speed of door opening/closing")]
    public float openSpeed = 1f;
    
    [Tooltip("Is this the left or right door?")]
    public bool isLeftDoor = true;
    private bool isOpen = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private LiftRescueManager procedureManager;
    private Vector3 targetPosition;
    private bool isGrabbed = false;
    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnDoorGrabbed);
            grabInteractable.selectExited.AddListener(OnDoorReleased);
        }
        if (doorPanel == null)
        {
            doorPanel = transform;
        }
        targetPosition = closedPosition;
    }
    private void Start()
    {
        procedureManager = LiftRescueManager.Instance;
    }
    private void OnDoorGrabbed(SelectEnterEventArgs args)
    {
        if (procedureManager == null) return;
        // Step 11: Manual door opening
        if (procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step11_ManualDoorOpen))
        {
            isGrabbed = true;
            Debug.Log($"{(isLeftDoor ? "Left" : "Right")} door grabbed");
        }
        else
        {
            Debug.LogWarning($"Cannot open doors yet - wrong step. Current: {procedureManager.GetCurrentStep()}");
        }
    }
    private void OnDoorReleased(SelectExitEventArgs args)
    {
        if (isGrabbed)
        {
            isGrabbed = false;
            
            // Check if door was pulled open enough
            float openThreshold = Vector3.Distance(openPosition, closedPosition) * 0.7f; // 70% open
            float currentDistance = Vector3.Distance(doorPanel.localPosition, closedPosition);
            
            if (currentDistance >= openThreshold)
            {
                // Door opened successfully
                OpenDoor();
                
                if (procedureManager != null)
                {
                    procedureManager.OnDoorsOpened();
                }
            }
            else
            {
                // Door not opened enough, return to closed
                CloseDoor();
            }
        }
    }
    private void OpenDoor()
    {
        isOpen = true;
        targetPosition = openPosition;
        Debug.Log($"{(isLeftDoor ? "Left" : "Right")} door opened");
    }
    private void CloseDoor()
    {
        isOpen = false;
        targetPosition = closedPosition;
        Debug.Log($"{(isLeftDoor ? "Left" : "Right")} door closed");
    }
    private void Update()
    {
        if (!isGrabbed && doorPanel != null)
        {
            // Smoothly move to target position when not being grabbed
            doorPanel.localPosition = Vector3.Lerp(
                doorPanel.localPosition,
                targetPosition,
                Time.deltaTime * openSpeed
            );
        }
        else if (isGrabbed && doorPanel != null)
        {
            // Allow manual pulling when grabbed
            // The XR Grab Interactable will handle the movement
            // We just need to constrain it to sliding motion
            Vector3 currentPos = doorPanel.localPosition;
            
            // Constrain to only move along the opening axis
            if (isLeftDoor)
            {
                // Left door slides left (negative X)
                currentPos.x = Mathf.Clamp(currentPos.x, openPosition.x, closedPosition.x);
            }
            else
            {
                // Right door slides right (positive X)
                currentPos.x = Mathf.Clamp(currentPos.x, closedPosition.x, openPosition.x);
            }
            
            // Keep Y and Z fixed
            currentPos.y = closedPosition.y;
            currentPos.z = closedPosition.z;
            
            doorPanel.localPosition = currentPos;
        }
    }
    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnDoorGrabbed);
            grabInteractable.selectExited.RemoveListener(OnDoorReleased);
        }
    }
}
