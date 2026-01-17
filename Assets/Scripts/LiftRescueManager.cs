using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class LiftRescueManager : MonoBehaviour
{
    // UPDATED: Intercom step removed - now 10 steps total
    public enum RescueStep
    {
        Step1_OpenPanels = 1,
        Step2_SwitchOffOCB,
        Step3_EnsureRPSOn,
        Step4_RotaryToMRO,
        Step5_PressUpUntilDZ,
        Step6_ExactDoorZoneKey,
        Step7_SwitchOffF5C,
        Step8_RotaryToNormal,
        Step9_LockPanels,
        Step10_ManualDoorOpen,
        Completed
    }
    [Header("State")]
    public RescueStep currentStep = RescueStep.Step1_OpenPanels;
    [Header("Audio")]
    public AudioSource audioSource;
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
    // UPDATED: 10 step instructions (Intercom removed)
    private string[] stepInstructions = new string[]
    {
        "Step 1: Open E&I and Main Power Panels",
        "Step 2: Switch OFF the OCB (Over Current Breaker)",
        "Step 3: Ensure RPS (Rope Position Switch) is ON",
        "Step 4: Turn Rotary Switch to MRO (Manual Rescue Operation) position",
        "Step 5: Press UP button until display shows DZ (Door Zone)",
        "Step 6: When in exact door zone, press KEY1 to confirm",
        "Step 7: Switch OFF F5C (FS2 switch)",
        "Step 8: Turn Rotary Switch back to Normal position",
        "Step 9: Close and lock both panels",
        "Step 10: Manually open lift doors to rescue passengers"
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
    // Called when EI Panel is opened
    public void OnEIPanelOpened()
    {
        Debug.Log(">>> Test: EI Panel");
        CheckPanelsOpened();
    }
    // Called when Main Power Panel is opened
    public void OnMainPowerPanelOpened()
    {
        Debug.Log(">>> Test: Main Power");
        CheckPanelsOpened();
    }
    private void CheckPanelsOpened()
    {
        if (currentStep == RescueStep.Step1_OpenPanels)
        {
            AdvanceStep();
        }
    }
    // Called when OCB lever is toggled OFF
    public void OnOCBSwitchedOff()
    {
        Debug.Log(">>> Test: OCB OFF");
        if (currentStep == RescueStep.Step2_SwitchOffOCB)
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
        Debug.Log(">>> Test: RPS Check");
        if (currentStep == RescueStep.Step3_EnsureRPSOn)
        {
            AdvanceStep();
        }
    }
    // Called when rotary switch is turned to MRO position (1->2)
    public void OnRotaryToMRO()
    {
        Debug.Log(">>> Test: Rotary to MRO");
        if (currentStep == RescueStep.Step4_RotaryToMRO)
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
        Debug.Log(">>> Test: UP Button");
        if (currentStep == RescueStep.Step5_PressUpUntilDZ)
        {
            AdvanceStep();
        }
    }
    // Called when KEY1 button is pressed
    public void OnKey1ButtonPressed()
    {
        Debug.Log(">>> Test: KEY1");
        if (currentStep == RescueStep.Step6_ExactDoorZoneKey)
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
        Debug.Log(">>> Test: F5C OFF");
        if (currentStep == RescueStep.Step7_SwitchOffF5C)
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
        Debug.Log(">>> Test: Rotary to Normal");
        if (currentStep == RescueStep.Step8_RotaryToNormal)
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
        Debug.Log(">>> Test: Panels Locked");
        if (currentStep == RescueStep.Step9_LockPanels)
        {
            AdvanceStep();
        }
    }
    // Called when doors are manually opened
    public void OnDoorsOpened()
    {
        Debug.Log(">>> Test: Doors Opened");
        if (currentStep == RescueStep.Step10_ManualDoorOpen)
        {
            AdvanceStep();
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
            stepCounterText.text = $"Step {(int)currentStep}/10";
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