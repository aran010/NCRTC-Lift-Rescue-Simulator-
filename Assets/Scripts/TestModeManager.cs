using UnityEngine;
using TMPro;

public class TestModeManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject testPanel;
    public TextMeshProUGUI stepCounterText;
    public TextMeshProUGUI mistakeCounterText;
    public TextMeshProUGUI timerText;

    private LiftRescueManager liftManager;
    private int currentStepIndex = 0;
    private int mistakeCount = 0;
    private float elapsedTime = 0f;
    private bool procedureComplete = false;

    void Start()
    {
        liftManager = FindObjectOfType<LiftRescueManager>();
        
        if (testPanel != null)
            testPanel.SetActive(true);
        
        UpdateUI();
    }

    void Update()
    {
        if (procedureComplete) return;

        // Update timer
        elapsedTime += Time.deltaTime;
        
        if (liftManager != null)
        {
            // Check if step has advanced
            int managerStep = (int)liftManager.GetCurrentStep();
            
            if (managerStep > currentStepIndex)
            {
                currentStepIndex = managerStep;
            }

            // Check if procedure is complete
            if (liftManager.GetCurrentStep() == LiftRescueManager.RescueStep.Completed)
            {
                procedureComplete = true;
               ShowCompletion();
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        // Update step counter
        if (stepCounterText != null)
        {
            stepCounterText.text = $"Step: {currentStepIndex + 1}/11";
        }

        // Update mistake counter (can be enhanced to track actual mistakes)
        if (mistakeCounterText != null)
        {
            mistakeCounterText.text = $"Mistakes: {mistakeCount}";
        }

        // Update timer
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    public void IncrementMistake()
    {
        mistakeCount++;
    }

    void ShowCompletion()
    {
        if (stepCounterText != null)
        {
            stepCounterText.text = "COMPLETED!";
        }

        // Calculate final score
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        Debug.Log($"Test Mode Completed!");
        Debug.Log($"Time: {minutes:00}:{seconds:00}");
        Debug.Log($"Mistakes: {mistakeCount}");
        
        // Could show a completion screen here
    }
}
