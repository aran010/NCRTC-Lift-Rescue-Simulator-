using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Generic interactable part for buttons, levers, and switches.
/// UPDATED: Enum references fixed for 10-step procedure (Intercom removed)
/// </summary>
public class InteractablePart : MonoBehaviour
{
    public enum PartType
    {
        Button,
        Lever,
        Switch,
        Key,
        Door
    }

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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip activateSound;
    public AudioClip deactivateSound;

    [Header("MRO Step Connection")]
    [Tooltip("Which MRO step does this part trigger?")]
    public LiftRescueManager.RescueStep triggerStep = LiftRescueManager.RescueStep.Step1_OpenPanels;

    [Tooltip("Reference to the Lift Rescue Manager")]
    public LiftRescueManager rescueManager;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onActivated;
    public UnityEngine.Events.UnityEvent onDeactivated;

    private void Start()
    {
        if (rescueManager == null)
        {
            rescueManager = FindObjectOfType<LiftRescueManager>();
        }

        // Set initial visual state
        UpdateVisuals();
    }

    // Called by XR Interaction or mouse click
    public void Interact()
    {
        if (isToggle)
        {
            isOn = !isOn;
        }
        else
        {
            isOn = true;
        }

        UpdateVisuals();
        PlaySound(isOn);
        TriggerMROStep();

        if (isOn)
        {
            onActivated?.Invoke();
        }
        else
        {
            onDeactivated?.Invoke();
        }
    }

    public void OnClick()
    {
        Interact();
    }

    public void Toggle()
    {
        Interact();
    }

    private void TriggerMROStep()
    {
        if (rescueManager == null) return;

        // Call the appropriate method based on the step
        switch (triggerStep)
        {
            case LiftRescueManager.RescueStep.Step1_OpenPanels:
                rescueManager.OnEIPanelOpened();
                break;
            case LiftRescueManager.RescueStep.Step2_SwitchOffOCB:
                rescueManager.OnOCBSwitchedOff();
                break;
            case LiftRescueManager.RescueStep.Step3_EnsureRPSOn:
                rescueManager.OnRPSChecked();
                break;
            case LiftRescueManager.RescueStep.Step4_RotaryToMRO:
                rescueManager.OnRotaryToMRO();
                break;
            case LiftRescueManager.RescueStep.Step5_PressUpUntilDZ:
                rescueManager.OnUpButtonPressed();
                break;
            case LiftRescueManager.RescueStep.Step6_ExactDoorZoneKey:
                rescueManager.OnKey1ButtonPressed();
                break;
            case LiftRescueManager.RescueStep.Step7_SwitchOffF5C:
                rescueManager.OnF5CSwitchedOff();
                break;
            case LiftRescueManager.RescueStep.Step8_RotaryToNormal:
                rescueManager.OnRotaryToNormal();
                break;
            case LiftRescueManager.RescueStep.Step9_LockPanels:
                rescueManager.OnPanelsLocked();
                break;
            case LiftRescueManager.RescueStep.Step10_ManualDoorOpen:
                rescueManager.OnDoorsOpened();
                break;
        }
    }

    private void UpdateVisuals()
    {
        if (movingPart == null) return;

        if (isOn)
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

    private void PlaySound(bool activated)
    {
        if (audioSource == null) return;

        if (activated && activateSound != null)
        {
            audioSource.PlayOneShot(activateSound);
        }
        else if (!activated && deactivateSound != null)
        {
            audioSource.PlayOneShot(deactivateSound);
        }
    }

    // Reset for momentary buttons
    public void Reset()
    {
        if (!isToggle)
        {
            isOn = false;
            UpdateVisuals();
        }
    }
}
