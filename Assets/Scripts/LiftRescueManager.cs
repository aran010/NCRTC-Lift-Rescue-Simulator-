using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class LiftRescueManager : MonoBehaviour
{
    public enum RescueStep
    {
        Step1_Intercom = 1,
        Step2_OpenPanels,
        Step3_SwitchOffOCB,
        Step4_EnsureRPSOn,
        Step5_RotaryToMRO,
        Step6_PressUpUntilDZ,
        Step7_ExactDoorZoneKey,
        Step8_SwitchOffF5C,
        Step9_RotaryToNormal,
        Step10_LockPanels,
        Step11_ManualDoorOpen,
        Completed
    }
    [Header("State")]
    public RescueStep currentStep = RescueStep.Step1_Intercom;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip intercomClip1;
    [Header("UI References (Optional - for displaying step info)")]
    public TextMeshProUGUI stepCounterText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI timerText;
    [Header("Interactive Components")]
    [Tooltip("EI Panel door")]
    public GameObject eiPanel;
    
    [Tooltip("Main Power Panel door")]
    public GameObject mainPowerPanel;
    
    [Tooltip("F5C Switch (FS2) inside EI Panel")]
    public GameObject f5cSwitch;
    
    [Tooltip("OCB Lever inside Main Power Panel")]
    public GameObject ocbLever;
    
    [Tooltip("RPS Switch/Indicator")]
    public GameObject rpsSwitch;
    
    [Tooltip("Rotary Switch")]
    public GameObject rotaryKnob;
    
    [Tooltip("SPB3 UP Button")]
    public GameObject upButton;
    
    [Tooltip("KEY1 Button")]
    public GameObject key1Button;
    
    [Tooltip("BRB2 Key Head")]
    public GameObject brbKey;
    
    [Tooltip("Left lift door")]
    public GameObject leftDoor;
    
    [Tooltip("Right lift door")]
    public GameObject rightDoor;
    [Header("Events")]
    public UnityEvent<RescueStep> onStepChanged;
    public UnityEvent onRescueComplete;
    // Singleton for easy access
    public static LiftRescueManager Instance;
    private float elapsedTime = 0f;
    private int mistakeCount = 0;
    private string[] stepInstructions = new string[]
    {
        "Step 1: Interact with passengers through intercom",
        "Step 2: Open E&I and Main Power Panels",
        "Step 3: Switch OFF the OCB (Over Current Breaker)",
        "Step 4: Ensure RPS (Rope Position Switch) is ON",
        "Step 5: Turn Rotary Switch to MRO (Manual Rescue Operation) position",
        "Step 6: Press UP button until display shows DZ (Door Zone)",
        "Step 7: When in exact door zone, press KEY1 to confirm",
        "Step 8: Switch OFF F5C (FS2 switch)",
        "Step 9: Turn Rotary Switch back to Normal position",
        "Step 10: Close and lock both panels",
        "Step 11: Manually open lift doors to rescue passengers"
    };
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        UpdateUI();
    }
    private void Update()
    {
        if (currentStep != RescueStep.Completed)
        {
            elapsedTime += Time.deltaTime;
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(elapsedTime / 60f);
                int seconds = Mathf.FloorToInt(elapsedTime % 60f);
                timerText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }
    }
    // Called by InteractablePart or other scripts when intercom is activated
    public void OnIntercomActivated()
    {
        if (currentStep == RescueStep.Step1_Intercom)
        {
            PlayRescueAudio(intercomClip1);
            AdvanceStep();
        }
    }
    // Called when EI Panel is opened
    public void OnEIPanelOpened()
    {
        CheckPanelsOpened();
    }
    // Called when Main Power Panel is opened
    public void OnMainPowerPanelOpened()
    {
        CheckPanelsOpened();
    }
    private void CheckPanelsOpened()
    {
        if (currentStep == RescueStep.Step2_OpenPanels)
        {
            // Check if both panels are opened
            // For now, we'll advance when either is opened
            // You can add more complex logic here
            AdvanceStep();
        }
    }
    // Called when OCB lever is toggled OFF
    public void OnOCBSwitchedOff()
    {
        if (currentStep == RescueStep.Step3_SwitchOffOCB)
        {
            AdvanceStep();
        }
        else
        {
            RecordMistake("OCB switched at wrong time");
        }
    }
    // Called to check RPS status
    public void OnRPSChecked()
    {
        if (currentStep == RescueStep.Step4_EnsureRPSOn)
        {
            // Assume RPS is ON for now
            AdvanceStep();
        }
    }
    // Called when rotary switch is turned to MRO position (1->2)
    public void OnRotaryToMRO()
    {
        if (currentStep == RescueStep.Step5_RotaryToMRO)
        {
            AdvanceStep();
        }
        else
        {
            RecordMistake("Rotary switch changed at wrong time");
        }
    }
    // Called when UP button is pressed
    public void OnUpButtonPressed()
    {
        if (currentStep == RescueStep.Step6_PressUpUntilDZ)
        {
            // In real scenario, check if display shows "DZ"
            // For now, advance immediately
            AdvanceStep();
        }
    }
    // Called when KEY1 button is pressed
    public void OnKey1ButtonPressed()
    {
        if (currentStep == RescueStep.Step7_ExactDoorZoneKey)
        {
            AdvanceStep();
        }
        else
        {
            RecordMistake("KEY1 pressed at wrong time");
        }
    }
    // Called when F5C switch is turned OFF
    public void OnF5CSwitchedOff()
    {
        if (currentStep == RescueStep.Step8_SwitchOffF5C)
        {
            AdvanceStep();
        }
        else
        {
            RecordMistake("F5C switched at wrong time");
        }
    }
    // Called when rotary switch is returned to Normal position (2->1)
    public void OnRotaryToNormal()
    {
        if (currentStep == RescueStep.Step9_RotaryToNormal)
        {
            AdvanceStep();
        }
        else
        {
            RecordMistake("Rotary switch changed at wrong time");
        }
    }
    // Called when panels are locked
    public void OnPanelsLocked()
    {
        if (currentStep == RescueStep.Step10_LockPanels)
        {
            AdvanceStep();
        }
    }
    // Called when doors are manually opened
    public void OnDoorsOpened()
    {
        if (currentStep == RescueStep.Step11_ManualDoorOpen)
        {
            AdvanceStep();
            // This should complete the rescue
            CompleteRescue();
        }
    }
    // Generic method to check and advance steps
    public void CheckStepAndAdvance(RescueStep requiredStep)
    {
        if (currentStep == requiredStep)
        {
            AdvanceStep();
        }
        else
        {
            RecordMistake($"Action performed at wrong step. Current: {currentStep}, Required: {requiredStep}");
        }
    }
    private void AdvanceStep()
    {
        if (currentStep != RescueStep.Completed)
        {
            currentStep++;
            Debug.Log($"Rescue Step Advanced to: {currentStep}");
            onStepChanged?.Invoke(currentStep);
            UpdateUI();
            if (currentStep == RescueStep.Completed)
            {
                CompleteRescue();
            }
        }
    }
    private void CompleteRescue()
    {
        Debug.Log("Rescue Operation Completed!");
        onRescueComplete?.Invoke();
        
        if (instructionText != null)
        {
            instructionText.text = "Rescue Complete! All passengers are safe.";
        }
    }
    private void RecordMistake(string reason)
    {
        mistakeCount++;
        Debug.LogWarning($"Mistake #{mistakeCount}: {reason}");
    }
    private void UpdateUI()
    {
        if (stepCounterText != null)
        {
            stepCounterText.text = $"Step {(int)currentStep}/11";
        }
        if (instructionText != null && (int)currentStep - 1 < stepInstructions.Length)
        {
            instructionText.text = stepInstructions[(int)currentStep - 1];
        }
    }
    private void PlayRescueAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    // Helper method for other scripts to check current step
    public bool IsCurrentStep(RescueStep step)
    {
        return currentStep == step;
    }
    // Public method to get current step (for debugging)
    public RescueStep GetCurrentStep()
    {
        return currentStep;
    }
}
