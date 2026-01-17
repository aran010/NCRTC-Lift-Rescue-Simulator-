using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class RotarySwitch : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("How many positions does the rotary switch have?")]
    public int positionCount = 3;
    
    [Tooltip("Current position (0-indexed)")]
    public int currentPosition = 0;
    
    [Tooltip("Angle between each position (degrees)")]
    public float anglePerPosition = 45f;
    
    [Tooltip("The knob that rotates")]
    public Transform knob;
    [Header("Interaction")]
    public float rotationSpeed = 5f;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private LiftRescueManager procedureManager;
    private Quaternion targetRotation;
    private int previousPosition = 0;
    private bool isGrabbed = false;
    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
        if (knob == null)
        {
            knob = transform;
        }
        UpdateRotation();
    }
    private void Start()
    {
        procedureManager = LiftRescueManager.Instance;
    }
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        previousPosition = currentPosition;
    }
    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        // Snap to nearest position
        SnapToNearestPosition();
        // Check if position changed
        if (previousPosition != currentPosition)
        {
            OnPositionChanged(previousPosition, currentPosition);
        }
    }
    private void Update()
    {
        if (isGrabbed)
        {
            // Allow user to rotate the knob while grabbed
            // The actual rotation is handled by XRGrabInteractable
            // We just need to track the position
            
            // Calculate current rotation angle
            float currentAngle = knob.localEulerAngles.y;
            if (currentAngle > 180) currentAngle -= 360;
            
            // Determine which position we're closest to
            int closestPosition = Mathf.RoundToInt(currentAngle / anglePerPosition);
            closestPosition = Mathf.Clamp(closestPosition, 0, positionCount - 1);
            
            // Update current position (but don't trigger events until released)
            currentPosition = closestPosition;
        }
        else
        {
            // Smoothly rotate to target position when not grabbed
            if (knob != null)
            {
                knob.localRotation = Quaternion.Slerp(
                    knob.localRotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
            }
        }
    }
    private void SnapToNearestPosition()
    {
        // Snap to the current position
        currentPosition = Mathf.Clamp(currentPosition, 0, positionCount - 1);
        UpdateRotation();
    }
    private void UpdateRotation()
    {
        float targetAngle = currentPosition * anglePerPosition;
        targetRotation = Quaternion.Euler(0, targetAngle, 0);
    }
    private void OnPositionChanged(int from, int to)
    {
        Debug.Log($"Rotary switch moved from position {from} to {to}");
        if (procedureManager == null) return;
        // Step 5: Rotate from Normal (position 0) to MRO (position 1)
        if (from == 0 && to == 1 && procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step5_RotaryToMRO))
        {
            procedureManager.OnRotaryToMRO();
        }
        // Step 9: Rotate from MRO (position 1) back to Normal (position 0)
        else if (from == 1 && to == 0 && procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step9_RotaryToNormal))
        {
            procedureManager.OnRotaryToNormal();
        }
    }
    public void SetPosition(int position)
    {
        if (position >= 0 && position < positionCount)
        {
            previousPosition = currentPosition;
            currentPosition = position;
            UpdateRotation();
            
            if (previousPosition != currentPosition)
            {
                OnPositionChanged(previousPosition, currentPosition);
            }
        }
    }
    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}