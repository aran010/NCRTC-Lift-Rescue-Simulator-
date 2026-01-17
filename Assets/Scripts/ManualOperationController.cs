using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
public class ManualOperationController : MonoBehaviour
{
    [Header("Display")]
    [Tooltip("The text display showing lift position/status")]
    public TextMeshPro displayText;
    
    [Tooltip("Current floor display value")]
    public string currentDisplay = "G";
    [Header("Buttons")]
    [Tooltip("UP button")]
    public GameObject upButton;
    
    [Tooltip("KEY1 confirmation button")]
    public GameObject key1Button;
    [Header("Lift Movement")]
    [Tooltip("The lift car GameObject")]
    public Transform liftCar;
    
    [Tooltip("How long it takes to move between floors")]
    public float movementDuration = 3f;
    
    [Tooltip("Target position when in door zone")]
    public Vector3 doorZonePosition;
    private LiftRescueManager procedureManager;
    private bool isMoving = false;
    private bool isInDoorZone = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable upButtonInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable key1ButtonInteractable;
    private void Awake()
    {
        if (upButton != null)
        {
            upButtonInteractable = upButton.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (upButtonInteractable != null)
            {
                upButtonInteractable.selectEntered.AddListener(OnUpButtonPressed);
            }
        }
        if (key1Button != null)
        {
            key1ButtonInteractable = key1Button.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (key1ButtonInteractable != null)
            {
                key1ButtonInteractable.selectEntered.AddListener(OnKey1ButtonPressed);
            }
        }
    }
    private void Start()
    {
        procedureManager = LiftRescueManager.Instance;
        UpdateDisplay("G");
    }
    private void OnUpButtonPressed(SelectEnterEventArgs args)
    {
        if (procedureManager == null) return;
        // Step 6: Press UP button to move lift to door zone
        if (procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step6_PressUpUntilDZ))
        {
            if (!isMoving && !isInDoorZone)
            {
                StartCoroutine(MoveToDoorZone());
            }
        }
    }
    private void OnKey1ButtonPressed(SelectEnterEventArgs args)
    {
        if (procedureManager == null) return;
        // Step 7: Press KEY1 when in exact door zone
        if (procedureManager.IsCurrentStep(LiftRescueManager.RescueStep.Step7_ExactDoorZoneKey))
        {
            if (isInDoorZone)
            {
                procedureManager.OnKey1ButtonPressed();
                Debug.Log("KEY1 pressed - Door zone confirmed");
            }
            else
            {
                Debug.LogWarning("KEY1 pressed but not in door zone!");
            }
        }
    }
    private System.Collections.IEnumerator MoveToDoorZone()
    {
        isMoving = true;
        
        // Simulate lift movement
        UpdateDisplay("1");
        yield return new WaitForSeconds(movementDuration * 0.33f);
        
        UpdateDisplay("2");
        yield return new WaitForSeconds(movementDuration * 0.33f);
        
        UpdateDisplay("DZ");
        yield return new WaitForSeconds(movementDuration * 0.34f);
        // Move lift car if assigned
        if (liftCar != null)
        {
            liftCar.localPosition = doorZonePosition;
        }
        isInDoorZone = true;
        isMoving = false;
        // Notify procedure manager
        if (procedureManager != null)
        {
            procedureManager.OnUpButtonPressed();
        }
        Debug.Log("Lift reached door zone (DZ)");
    }
    private void UpdateDisplay(string text)
    {
        currentDisplay = text;
        if (displayText != null)
        {
            displayText.text = text;
        }
    }
    public bool IsInDoorZone()
    {
        return isInDoorZone;
    }
    private void OnDestroy()
    {
        if (upButtonInteractable != null)
        {
            upButtonInteractable.selectEntered.RemoveListener(OnUpButtonPressed);
        }
        if (key1ButtonInteractable != null)
        {
            key1ButtonInteractable.selectEntered.RemoveListener(OnKey1ButtonPressed);
        }
    }
}