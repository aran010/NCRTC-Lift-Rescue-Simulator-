using UnityEngine;

/// <summary>
/// Rotary switch controller for MRO/Normal mode selection.
/// UPDATED: Enum references fixed for 10-step procedure (Intercom removed)
/// </summary>
public class RotarySwitch : MonoBehaviour
{
    public enum SwitchPosition
    {
        Normal = 1,
        MRO = 2
    }

    [Header("Settings")]
    public SwitchPosition currentPosition = SwitchPosition.Normal;
    public float rotationAngle = 90f;
    public float rotateSpeed = 5f;

    [Header("References")]
    public Transform switchKnob;
    public LiftRescueManager rescueManager;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip rotateSound;

    private Quaternion targetRotation;

    private void Start()
    {
        if (rescueManager == null)
        {
            rescueManager = FindObjectOfType<LiftRescueManager>();
        }

        if (switchKnob == null)
        {
            switchKnob = transform;
        }

        UpdateRotation();
    }

    private void Update()
    {
        switchKnob.localRotation = Quaternion.Lerp(
            switchKnob.localRotation, 
            targetRotation, 
            Time.deltaTime * rotateSpeed
        );
    }

    public void Toggle()
    {
        if (currentPosition == SwitchPosition.Normal)
        {
            SetToMRO();
        }
        else
        {
            SetToNormal();
        }
    }

    public void OnClick()
    {
        Toggle();
    }

    public void RotateSwitch()
    {
        Toggle();
    }

    public void SetToMRO()
    {
        currentPosition = SwitchPosition.MRO;
        UpdateRotation();
        PlaySound();

        // Trigger MRO step (Step 4)
        if (rescueManager != null)
        {
            if (rescueManager.currentStep == LiftRescueManager.RescueStep.Step4_RotaryToMRO)
            {
                rescueManager.OnRotaryToMRO();
            }
        }
    }

    public void SetToNormal()
    {
        currentPosition = SwitchPosition.Normal;
        UpdateRotation();
        PlaySound();

        // Trigger MRO step (Step 8)
        if (rescueManager != null)
        {
            if (rescueManager.currentStep == LiftRescueManager.RescueStep.Step8_RotaryToNormal)
            {
                rescueManager.OnRotaryToNormal();
            }
        }
    }

    private void UpdateRotation()
    {
        float angle = currentPosition == SwitchPosition.MRO ? rotationAngle : 0f;
        targetRotation = Quaternion.Euler(0, angle, 0);
    }

    private void PlaySound()
    {
        if (audioSource != null && rotateSound != null)
        {
            audioSource.PlayOneShot(rotateSound);
        }
    }
}
