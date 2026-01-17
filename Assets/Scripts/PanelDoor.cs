using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class PanelDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("The door body that rotates")]
    public Transform doorBody;
    
    [Tooltip("Rotation when door is open")]
    public Vector3 openRotation = new Vector3(0, -90, 0);
    
    [Tooltip("Rotation when door is closed")]
    public Vector3 closedRotation = new Vector3(0, 0, 0);
    
    [Tooltip("Speed of door opening/closing")]
    public float openSpeed = 2f;
    [Header("Locking")]
    [Tooltip("Is the door currently locked?")]
    public bool isLocked = false;
    
    [Tooltip("Angle threshold to consider door locked (degrees from closed)")]
    public float lockedAngle = 5f;
    
    [Tooltip("Tolerance when checking if door can be locked")]
    public float unlockTolerance = 10f;
    [Header("Hinge")]
    public Transform hinge;
    private bool isOpen = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;
    private LiftRescueManager procedureManager;
    private Quaternion targetRotation;
    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnDoorInteracted);
        }
        if (doorBody == null)
        {
            doorBody = transform;
        }
        targetRotation = doorBody.localRotation;
    }
    private void Start()
    {
        procedureManager = LiftRescueManager.Instance;
    }
    private void OnDoorInteracted(SelectEnterEventArgs args)
    {
        if (procedureManager == null) return;
        // Step 2: Open panels
        if (!isOpen && procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step2_OpenPanels))
        {
            if (!isLocked)
            {
                OpenDoor();
                
                // Notify manager based on which panel this is
                if (gameObject.name.Contains("EI"))
                {
                    procedureManager.OnEIPanelOpened();
                }
                else if (gameObject.name.Contains("Main") || gameObject.name.Contains("Power"))
                {
                    procedureManager.OnMainPowerPanelOpened();
                }
            }
        }
        // Step 10: Lock panels
        else if (isOpen && procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step10_LockPanels))
        {
            CloseDoor();
            LockDoor();
            procedureManager.OnPanelsLocked();
        }
        else
        {
            // Allow toggling door outside of procedure steps (for testing)
            if (!isLocked)
            {
                if (isOpen)
                {
                    CloseDoor();
                }
                else
                {
                    OpenDoor();
                }
            }
        }
    }
    private void OpenDoor()
    {
        if (isLocked)
        {
            Debug.LogWarning("Door is locked!");
            return;
        }
        isOpen = true;
        targetRotation = Quaternion.Euler(openRotation);
        Debug.Log($"{gameObject.name} opened");
    }
    private void CloseDoor()
    {
        isOpen = false;
        targetRotation = Quaternion.Euler(closedRotation);
        Debug.Log($"{gameObject.name} closed");
    }
    private void LockDoor()
    {
        // Check if door is close enough to closed position
        float currentAngle = Quaternion.Angle(doorBody.localRotation, Quaternion.Euler(closedRotation));
        
        if (currentAngle <= unlockTolerance)
        {
            isLocked = true;
            Debug.Log($"{gameObject.name} locked");
        }
        else
        {
            Debug.LogWarning($"Cannot lock door - not closed enough (angle: {currentAngle})");
        }
    }
    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log($"{gameObject.name} unlocked");
    }
    private void Update()
    {
        // Smoothly rotate to target rotation
        if (doorBody != null)
        {
            doorBody.localRotation = Quaternion.Slerp(
                doorBody.localRotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );
        }
    }
    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnDoorInteracted);
        }
    }
}