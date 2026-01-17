using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Screen References")]
    public GameObject welcomeScreen;
    public GameObject modeSelectionScreen;
    public GameObject guidePanel;
    public GameObject testPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button guideModeButton;
    public Button testModeButton;

    [Header("Managers")]
    public FirstPersonController playerController;
    public GuideManager guideManager;
    public TestModeManager testModeManager;

    [Header("Camera Positioning")]
    public GameObject eiPanel; // Assign this in Inspector
    public Transform spawnPoint; // Create an empty GameObject and position it where you want the player to start

    private bool gameStarted = false;

    void Start()
    {
        // Setup button listeners
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        
        if (guideModeButton != null)
            guideModeButton.onClick.AddListener(OnGuideModeSelected);
        
        if (testModeButton != null)
            testModeButton.onClick.AddListener(OnTestModeSelected);

        // Show welcome screen
        ShowWelcomeScreen();
    }

    void ShowWelcomeScreen()
    {
        welcomeScreen.SetActive(true);
        modeSelectionScreen.SetActive(false);
        guidePanel.SetActive(false);
        testPanel.SetActive(false);

        // Disable player movement
        if (playerController != null)
        {
            playerController.DisableMovement();
            playerController.UnlockCursor();
        }
    }

    void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked!");
        welcomeScreen.SetActive(false);
        modeSelectionScreen.SetActive(true);
    }

    void OnGuideModeSelected()
    {
        Debug.Log("Guide Mode selected!");
        StartGame(true);
    }

    void OnTestModeSelected()
    {
        Debug.Log("Test Mode selected!");
        StartGame(false);
    }

    void StartGame(bool isGuideMode)
    {
        // Hide menus
        welcomeScreen.SetActive(false);
        modeSelectionScreen.SetActive(false);

        // Enable appropriate mode
        if (isGuideMode)
        {
            guidePanel.SetActive(true);
            testPanel.SetActive(false);
            
            if (guideManager != null)
                guideManager.enabled = true;
            
            if (testModeManager != null)
                testModeManager.enabled = false;
        }
        else
        {
            guidePanel.SetActive(false);
            testPanel.SetActive(true);
            
            if (guideManager != null)
                guideManager.enabled = false;
            
            if (testModeManager != null)
                testModeManager.enabled = true;
        }

        // Enable player movement
        if (playerController != null)
        {
            playerController.EnableMovement();
            playerController.LockCursor();
        }

        // Position camera to face EI panel
        //PositionCameraToFacePanel();

        gameStarted = true;
    }

    void PositionCameraToFacePanel()
    {
        if (playerController == null)
        {
            Debug.LogWarning("Player Controller not assigned!");
            return;
        }
        
        // Use FIXED spawn position that we KNOW is on the floor
        // Adjust these values based on your scene layout
        Vector3 spawnPosition = new Vector3(0f, 0.1f, -2f); // Standing on floor, 2m back from origin
        Vector3 lookAtTarget = new Vector3(0f, 1.2f, 0f); // Looking at chest height
        
        // If EI Panel is assigned, use it to calculate look direction
        if (eiPanel != null)
        {
            lookAtTarget = eiPanel.transform.position;
            // Position player 1.5m back from panel, but at Y=0.1 (floor level)
            spawnPosition = new Vector3(
                eiPanel.transform.position.x,
                0.1f, // Always spawn at floor level!
                eiPanel.transform.position.z - 1.5f
            );
        }
        
        // Set player position
        playerController.transform.position = spawnPosition;
        
        // Rotate to look at target
        Vector3 direction = lookAtTarget - spawnPosition;
        direction.y = 0; // Keep rotation level (no looking up/down initially)
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            playerController.transform.rotation = lookRotation;
        }
        
        Debug.Log($"Player spawned at {spawnPosition}, looking at {lookAtTarget}");
    }

    public void RestartSimulator()
    {
        // Reload the scene or reset state
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void QuitSimulator()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}