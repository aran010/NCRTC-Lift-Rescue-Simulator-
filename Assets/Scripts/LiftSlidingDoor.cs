using UnityEngine;

/// <summary>
/// Lift door controller for manual rescue operation.
/// UPDATED: Enum references fixed for 10-step procedure (Intercom removed)
/// </summary>
public class LiftSlidingDoor : MonoBehaviour
{
    [Header("Settings")]
    public bool isOpen = false;
    public float openDistance = 0.5f;
    public float slideSpeed = 2f;

    [Header("References")]
    public Transform leftDoor;
    public Transform rightDoor;
    public LiftRescueManager rescueManager;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip slideSound;

    private Vector3 leftDoorClosedPos;
    private Vector3 rightDoorClosedPos;
    private Vector3 leftDoorOpenPos;
    private Vector3 rightDoorOpenPos;

    private void Start()
    {
        if (rescueManager == null)
        {
            rescueManager = FindObjectOfType<LiftRescueManager>();
        }

        if (leftDoor != null)
        {
            leftDoorClosedPos = leftDoor.localPosition;
            leftDoorOpenPos = leftDoorClosedPos + Vector3.left * openDistance;
        }

        if (rightDoor != null)
        {
            rightDoorClosedPos = rightDoor.localPosition;
            rightDoorOpenPos = rightDoorClosedPos + Vector3.right * openDistance;
        }
    }

    private void Update()
    {
        if (leftDoor != null)
        {
            leftDoor.localPosition = Vector3.Lerp(
                leftDoor.localPosition,
                isOpen ? leftDoorOpenPos : leftDoorClosedPos,
                Time.deltaTime * slideSpeed
            );
        }

        if (rightDoor != null)
        {
            rightDoor.localPosition = Vector3.Lerp(
                rightDoor.localPosition,
                isOpen ? rightDoorOpenPos : rightDoorClosedPos,
                Time.deltaTime * slideSpeed
            );
        }
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

        if (audioSource != null && slideSound != null)
        {
            audioSource.PlayOneShot(slideSound);
        }

        // Trigger MRO step for manual door opening (Step 10)
        if (rescueManager != null)
        {
            if (rescueManager.currentStep == LiftRescueManager.RescueStep.Step10_ManualDoorOpen)
            {
                rescueManager.OnDoorsOpened();
            }
        }
    }

    public void Close()
    {
        isOpen = false;

        if (audioSource != null && slideSound != null)
        {
            audioSource.PlayOneShot(slideSound);
        }
    }
}
