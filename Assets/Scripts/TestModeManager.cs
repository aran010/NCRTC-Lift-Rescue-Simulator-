using UnityEngine;
using TMPro;

public class TestModeManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI stepText;
    public TextMeshProUGUI mistakeText;
    public TextMeshProUGUI timeText;
    public GameObject testPanel;

    [Header("References")]
    public LiftRescueManager rescueManager;

    private int currentStep = 1;
    private int totalSteps = 10;
    private int mistakes = 0;
    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Start()
    {
        if (testPanel != null)
            testPanel.SetActive(false);
    }

    void OnEnable()
    {
        currentStep = 1;
        mistakes = 0;
        elapsedTime = 0f;
        isRunning = true;
        UpdateUI();
    }

    void OnDisable()
    {
        isRunning = false;
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay();
        }
    }

    public void OnStepAttempted(LiftRescueManager.RescueStep attemptedStep)
    {
        // Get the expected step based on current step number
        LiftRescueManager.RescueStep expectedStep = GetExpectedStep(currentStep);

        if (attemptedStep == expectedStep)
        {
            // Correct step
            currentStep++;
            if (currentStep > totalSteps)
            {
                CompleteTest();
            }
        }
        else
        {
            // Wrong step - increment mistakes
            mistakes++;
            Debug.Log($"Mistake! Expected {expectedStep}, got {attemptedStep}. Total mistakes: {mistakes}");
        }
        
        UpdateUI();
    }

    LiftRescueManager.RescueStep GetExpectedStep(int stepNum)
    {
        switch (stepNum)
        {
            case 1: return LiftRescueManager.RescueStep.Step1_OpenPanels;
            case 2: return LiftRescueManager.RescueStep.Step2_SwitchOffOCB;
            case 3: return LiftRescueManager.RescueStep.Step3_EnsureRPSOn;
            case 4: return LiftRescueManager.RescueStep.Step4_RotaryToMRO;
            case 5: return LiftRescueManager.RescueStep.Step5_PressUpUntilDZ;
            case 6: return LiftRescueManager.RescueStep.Step6_ExactDoorZoneKey;
            case 7: return LiftRescueManager.RescueStep.Step7_SwitchOffF5C;
            case 8: return LiftRescueManager.RescueStep.Step8_RotaryToNormal;
            case 9: return LiftRescueManager.RescueStep.Step9_LockPanels;
            case 10: return LiftRescueManager.RescueStep.Step10_ManualDoorOpen;
            default: return LiftRescueManager.RescueStep.Completed;
        }
    }

    void UpdateUI()
    {
        if (stepText != null)
            stepText.text = $"Step: {currentStep}/{totalSteps}";
        
        if (mistakeText != null)
            mistakeText.text = $"Mistakes: {mistakes}";
    }

    void UpdateTimeDisplay()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    void CompleteTest()
    {
        isRunning = false;
        Debug.Log($"Test Complete! Time: {elapsedTime:F1}s, Mistakes: {mistakes}");
    }
}