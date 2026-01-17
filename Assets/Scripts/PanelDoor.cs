using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Panel door controller for EI Panel and Main Power Panel.
/// UPDATED: Enum references fixed for 10-step procedure (Intercom removed)
/// </summary>
public class PanelDoor : MonoBehaviour
{
    [Header("Settings")]
    public bool isOpen = false;
    public float openAngle = 90f;
    public float closeAngle = 0f;
    public float openSpeed = 5f;

    [Header("References")]
    public Transform doorPivot;
    public LiftRescueManager rescueManager;

    [Header("Door Type")]
    public bool isEIPanel = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private Quaternion targetRotation;

    private void Start()
    {
        if (rescueManager == null)
        {
            rescueManager = FindObjectOfType<LiftRescueManager>();
        }

        if (doorPivot == null)
        {
            doorPivot = transform;
        }

        targetRotation = doorPivot.localRotation;
    }

    private void Update()
    {
        doorPivot.localRotation = Quaternion.Lerp(
            doorPivot.localRotation, 
            targetRotation, 
            Time.deltaTime * openSpeed
        );
    }

    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void OnClick()
    {
        Toggle();
    }

    public void Open()
    {
        isOpen = true;
        targetRotation = Quaternion.Euler(0, openAngle, 0);

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        // Trigger MRO step for opening panels (Step 1)
        if (rescueManager != null)
        {
            if (rescueManager.currentStep == LiftRescueManager.RescueStep.Step1_OpenPanels)
            {
                if (isEIPanel)
                {
                    rescueManager.OnEIPanelOpened();
                }
                else
                {
                    rescueManager.OnMainPowerPanelOpened();
                }
            }
        }
    }

    public void Close()
    {
        isOpen = false;
        targetRotation = Quaternion.Euler(0, closeAngle, 0);

        if (audioSource != null && closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }

        // Trigger MRO step for locking panels (Step 9)
        if (rescueManager != null)
        {
            if (rescueManager.currentStep == LiftRescueManager.RescueStep.Step9_LockPanels)
            {
                rescueManager.OnPanelsLocked();
            }
        }
    }

    public void ToggleDoor()
    {
        Toggle();
    }
}
