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
    public GameObject eiPanel;
    public Transform spawnPoint;
    
    [Header("Settings")]
    public bool useScriptPositioning = false;

    private bool gameStarted = false;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        
        if (guideModeButton != null)
            guideModeButton.onClick.AddListener(OnGuideModeSelected);
        
        if (testModeButton != null)
            testModeButton.onClick.AddListener(OnTestModeSelected);

        ShowWelcomeScreen();
    }

    void ShowWelcomeScreen()
    {
        if (welcomeScreen != null) welcomeScreen.SetActive(true);
        if (modeSelectionScreen != null) modeSelectionScreen.SetActive(false);
        if (guidePanel != null) guidePanel.SetActive(false);
        if (testPanel != null) testPanel.SetActive(false);

        if (playerController != null)
        {
            playerController.DisableMovement();
            playerController.UnlockCursor();
        }
    }

    void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked!");
        if (welcomeScreen != null) welcomeScreen.SetActive(false);
        if (modeSelectionScreen != null) modeSelectionScreen.SetActive(true);
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
        if (welcomeScreen != null) welcomeScreen.SetActive(false);
        if (modeSelectionScreen != null) modeSelectionScreen.SetActive(false);

        if (isGuideMode)
        {
            if (guidePanel != null) guidePanel.SetActive(true);
            if (testPanel != null) testPanel.SetActive(false);
            if (guideManager != null) guideManager.enabled = true;
            if (testModeManager != null) testModeManager.enabled = false;
        }
        else
        {
            if (guidePanel != null) guidePanel.SetActive(false);
            if (testPanel != null) testPanel.SetActive(true);
            if (guideManager != null) guideManager.enabled = false;
            if (testModeManager != null) testModeManager.enabled = true;
        }

        if (playerController != null)
        {
            playerController.EnableMovement();
            playerController.LockCursor();
        }

        if (useScriptPositioning)
        {
            PositionPlayer();
        }

        gameStarted = true;
    }

    void PositionPlayer()
    {
        if (playerController == null) return;
        
        if (spawnPoint != null)
        {
            playerController.transform.position = spawnPoint.position;
            playerController.transform.rotation = spawnPoint.rotation;
        }
    }

    public void RestartSimulator()
    {
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
