using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class InteractablePart : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Is this a toggle switch (stays on/off) or momentary button?")]
    public bool isToggle = false;
    
    [Tooltip("Current state (for toggles)")]
    public bool isOn = false;
    [Header("Visuals")]
    [Tooltip("The part that moves/rotates when interacted")]
    public Transform movingPart;
    
    [Tooltip("Position when active (for buttons/switches)")]
    public Vector3 activePosition;
    
    [Tooltip("Position when inactive")]
    public Vector3 inactivePosition;
    
    [Tooltip("Rotation when active")]
    public Vector3 activeRotation;
    
    [Tooltip("Rotation when inactive")]
    public Vector3 inactiveRotation;
    [Header("Events")]
    public UnityEngine.Events.UnityEvent<bool> onToggle;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;
    private LiftRescueManager procedureManager;
    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnInteracted);
        }
    }
    private void Start()
    {
        procedureManager = LiftRescueManager.Instance;
        
        // Set initial visual state
        if (movingPart != null)
        {
            UpdateVisuals();
        }
    }
    private void OnInteracted(SelectEnterEventArgs args)
    {
        // Check which step we're in and handle accordingly
        if (procedureManager == null) return;
        // F5C Switch - Step 8
        if (gameObject.name.Contains("F5C") || gameObject.name.Contains("FS2"))
        {
            if (procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step8_SwitchOffF5C))
            {
                Toggle();
                procedureManager.OnF5CSwitchedOff();
            }
        }
        // OCB Lever - Step 3
        else if (gameObject.name.Contains("OCB"))
        {
            if (procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step3_SwitchOffOCB))
            {
                Toggle();
                procedureManager.OnOCBSwitchedOff();
            }
        }
        // UP Button - Step 6
        else if (gameObject.name.Contains("UP") || gameObject.name.Contains("Btn_2"))
        {
            if (procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step6_PressUpUntilDZ))
            {
                PressButton();
                procedureManager.OnUpButtonPressed();
            }
        }
        // KEY1 Button - Step 7
        else if (gameObject.name.Contains("KEY1"))
        {
            if (procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step7_ExactDoorZoneKey))
            {
                PressButton();
                procedureManager.OnKey1ButtonPressed();
            }
        }
        else
        {
            // Generic toggle
            Toggle();
        }
    }
    private void Toggle()
    {
        if (isToggle)
        {
            isOn = !isOn;
            UpdateVisuals();
            onToggle?.Invoke(isOn);
        }
    }
    private void PressButton()
    {
        if (!isToggle)
        {
            // Momentary button press
            onToggle?.Invoke(true);
            UpdateVisuals();
            
            // Return to inactive state after a short delay
            Invoke(nameof(ReleaseButton), 0.1f);
        }
    }
    private void ReleaseButton()
    {
        onToggle?.Invoke(false);
        UpdateVisuals();
    }
    private void UpdateVisuals()
    {
        if (movingPart == null) return;
        if (isOn || !isToggle)
        {
            movingPart.localPosition = activePosition;
            movingPart.localEulerAngles = activeRotation;
        }
        else
        {
            movingPart.localPosition = inactivePosition;
            movingPart.localEulerAngles = inactiveRotation;
        }
    }
    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnInteracted);
        }
    }
}