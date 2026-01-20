using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Guide Manager - displays step-by-step instructions
/// UPDATED: 10 steps (Intercom removed)
/// </summary>
public class GuideManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI stepText;
    public TextMeshProUGUI instructionText;

    [Header("References")]
    public LiftRescueManager rescueManager;

       private string[] stepInstructions = new string[]
    {
        "Step 1: Open E&I and Main Power Panels",
        "Step 2: Switch OFF the OCB (Over Current Breaker) lever",
        "Step 3: Check that RPS (Rope Position Switch) is ON",
        "Step 4: Rotate the Rotary Switch from Normal to MRO position",
        "Step 5: Press the UP button until display shows DZ",
        "Step 6: Press KEY1 button when in Door Zone",
        "Step 7: Switch OFF F5C (FS2) switch",
        "Step 8: Rotate the Rotary Switch back to Normal",
        "Step 9: Close and lock both panels",
        "Step 10: Manually open the lift doors to rescue passengers"
    };

    private void Start()
    {
        if (rescueManager == null)
        {
            rescueManager = FindObjectOfType<LiftRescueManager>();
        }

        if (rescueManager != null)
        {
            rescueManager.onStepChanged.AddListener(OnStepChanged);
        }

        UpdateDisplay();
    }

    private void OnEnable()
    {
        UpdateDisplay();
    }

    private void OnStepChanged(LiftRescueManager.RescueStep newStep)
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (rescueManager == null) return;

        int currentStepIndex = (int)rescueManager.currentStep - 1;

        if (stepText != null)
        {
            stepText.text = $"Step {currentStepIndex + 1} / 10";
        }

        if (instructionText != null)
        {
            if (currentStepIndex >= 0 && currentStepIndex < stepInstructions.Length)
            {
                instructionText.text = stepInstructions[currentStepIndex];
                Debug.Log($"Guide Mode: {stepInstructions[currentStepIndex]}");
            }
            else if (rescueManager.currentStep == LiftRescueManager.RescueStep.Completed)
            {
                instructionText.text = "Rescue Complete! All passengers are safe.";
            }
        }
    }

    private void OnDestroy()
    {
        if (rescueManager != null)
        {
            rescueManager.onStepChanged.RemoveListener(OnStepChanged);
        }
    }
}