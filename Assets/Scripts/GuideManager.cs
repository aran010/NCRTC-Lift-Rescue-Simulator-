using UnityEngine;
using TMPro;

public class GuideManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject guidePanel;
    public TextMeshProUGUI stepCounterText;
    public TextMeshProUGUI instructionText;
    public GameObject highlightIndicator;

    [Header("Step Target GameObjects")]
    public GameObject intercomButton;
    public GameObject eiPanel;
    public GameObject mainPowerPanel;
    public GameObject ocbLever;
    public GameObject rpsSwitch;
    public GameObject rotaryKnob;
    public GameObject upButton;
    public GameObject key1Button;
    public GameObject f5cSwitch;
    public GameObject liftDoors;

    private LiftRescueManager liftManager;
    private int currentStepIndex = 0;
    private string[] stepInstructions;
    private GameObject[] stepTargets;

    void Start()
    {
        liftManager = FindObjectOfType<LiftRescueManager>();
        
        InitializeStepData();
        
        if (guidePanel != null)
            guidePanel.SetActive(true);
        
        ShowStep(0);
    }

    void InitializeStepData()
    {
        // Step instructions for all 11 steps
        stepInstructions = new string[]
        {
            "Step 1: Press the INTERCOM button to establish communication with the control room",
            "Step 2: Open the Earthing Indicator (EI) panel door by clicking on it",
            "Step 3: Open the Main Power Panel door to access the control switches",
            "Step 4: Turn the OCB (Oil Circuit Breaker) lever to the OFF position",
            "Step 5: Verify the RPS (Rope Position System) indicator status",
            "Step 6: Rotate the mode selector switch to MRO (Maintenance, Repair, Operations) position",
            "Step 7: Press the UP button (SPB3) to move the lift car",
            "Step 8: Insert and turn KEY1 to activate manual control",
            "Step 9: Turn the F5C switch to OFF position",
            "Step 10: Return the rotary selector to NORMAL position",
            "Step 11: Manually open the lift door to complete the rescue procedure"
        };

        // Corresponding GameObjects for each step
        stepTargets = new GameObject[]
        {
            intercomButton,
            eiPanel,
            mainPowerPanel,
            ocbLever,
            rpsSwitch,
            rotaryKnob,
            upButton,
            key1Button,
            f5cSwitch,
            rotaryKnob, // Same as step 6 but different action
            liftDoors
        };
    }

    void Update()
    {
        if (liftManager == null) return;

        // Check if step has advanced
        int managerStep = (int)liftManager.GetCurrentStep();
        
        if (managerStep > currentStepIndex)
        {
            currentStepIndex = managerStep;
            ShowStep(currentStepIndex);
        }
    }

    void ShowStep(int stepIndex)
    {
        if (stepIndex >= stepInstructions.Length)
        {
            ShowCompletion();
            return;
        }

        // Update step counter
        if (stepCounterText != null)
        {
            stepCounterText.text = $"Step {stepIndex + 1}/11";
        }

        // Update instruction text
        if (instructionText != null)
        {
            instructionText.text = stepInstructions[stepIndex];
        }

        // Position highlight indicator at target
        if (highlightIndicator != null && stepTargets[stepIndex] != null)
        {
            highlightIndicator.SetActive(true);
            highlightIndicator.transform.position = 
                stepTargets[stepIndex].transform.position + Vector3.up * 0.5f;
            highlightIndicator.transform.SetParent(stepTargets[stepIndex].transform);
        }

        Debug.Log($"Guide Mode: {stepInstructions[stepIndex]}");
    }

    void ShowCompletion()
    {
        if (stepCounterText != null)
        {
            stepCounterText.text = "COMPLETED!";
        }

        if (instructionText != null)
        {
            instructionText.text = "Congratulations! You have successfully completed the NCRTC Lift Rescue procedure!";
        }

        if (highlightIndicator != null)
        {
            highlightIndicator.SetActive(false);
        }

        Debug.Log("Guide Mode: Rescue procedure completed successfully!");
    }
}
