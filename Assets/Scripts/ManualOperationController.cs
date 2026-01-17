using UnityEngine;
/// <summary>
/// Manual operation controller for managing the MRO procedure.
/// UPDATED: Enum references fixed for 10-step procedure (Intercom removed)
/// </summary>
public class ManualOperationController : MonoBehaviour
{
    [Header("References")]
    public LiftRescueManager rescueManager;
    [Header("Components")]
    public GameObject eiPanel;
    public GameObject mainPowerPanel;
    public GameObject ocbLever;
    public GameObject rpsIndicator;
    public GameObject rotarySwitch;
    public GameObject upButton;
    public GameObject key1Button;
    public GameObject f5cSwitch;
    public GameObject liftDoors;
    private void Start()
    {
        if (rescueManager == null)
        {
            rescueManager = FindObjectOfType<LiftRescueManager>();
        }
    }
    // Called when EI Panel is opened
    public void OnEIPanelOpened()
    {
        Debug.Log(">>> EI Panel Opened");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step1_OpenPanels)
        {
            rescueManager.OnEIPanelOpened();
        }
    }
    // Called when Main Power Panel is opened
    public void OnMainPowerOpened()
    {
        Debug.Log(">>> Main Power Panel Opened");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step1_OpenPanels)
        {
            rescueManager.OnMainPowerPanelOpened();
        }
    }
    // Called when OCB is switched off
    public void OnOCBOff()
    {
        Debug.Log(">>> OCB Switched OFF");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step2_SwitchOffOCB)
        {
            rescueManager.OnOCBSwitchedOff();
        }
    }
    // Called when RPS is checked
    public void OnRPSChecked()
    {
        Debug.Log(">>> RPS Checked");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step3_EnsureRPSOn)
        {
            rescueManager.OnRPSChecked();
        }
    }
    // Called when Rotary switch moves to MRO
    public void OnRotaryToMRO()
    {
        Debug.Log(">>> Rotary to MRO");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step4_RotaryToMRO)
        {
            rescueManager.OnRotaryToMRO();
        }
    }
    // Called when UP button is pressed
    public void OnUpPressed()
    {
        Debug.Log(">>> UP Button Pressed");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step5_PressUpUntilDZ)
        {
            rescueManager.OnUpButtonPressed();
        }
    }
    // Called when KEY1 is pressed
    public void OnKey1Pressed()
    {
        Debug.Log(">>> KEY1 Pressed");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step6_ExactDoorZoneKey)
        {
            rescueManager.OnKey1ButtonPressed();
        }
    }
    // Called when F5C is switched off
    public void OnF5COff()
    {
        Debug.Log(">>> F5C Switched OFF");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step7_SwitchOffF5C)
        {
            rescueManager.OnF5CSwitchedOff();
        }
    }
    // Called when Rotary switch moves to Normal
    public void OnRotaryToNormal()
    {
        Debug.Log(">>> Rotary to Normal");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step8_RotaryToNormal)
        {
            rescueManager.OnRotaryToNormal();
        }
    }
    // Called when panels are locked
    public void OnPanelsLocked()
    {
        Debug.Log(">>> Panels Locked");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step9_LockPanels)
        {
            rescueManager.OnPanelsLocked();
        }
    }
    // Called when doors are manually opened
    public void OnDoorsOpened()
    {
        Debug.Log(">>> Doors Opened");
        if (rescueManager != null && rescueManager.currentStep == LiftRescueManager.RescueStep.Step10_ManualDoorOpen)
        {
            rescueManager.OnDoorsOpened();
        }
    }
}