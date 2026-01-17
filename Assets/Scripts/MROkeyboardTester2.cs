using UnityEngine;

public class MROKeyboardTester : MonoBehaviour
{
    private LiftRescueManager manager;

    void Start()
    {
        manager = GetComponent<LiftRescueManager>();
        
        if (manager == null)
        {
            Debug.LogError("MROKeyboardTester: LiftRescueManager not found!");
        }
        else
        {
            Debug.Log("=== MRO Keyboard Tester Active ===");
            Debug.Log("1=EI Panel | 2=Main Power | 3=OCB | 4=RPS | 5=F5C | 6=UP | 7=KEY1");
        }
    }

    void Update()
    {
        if (manager == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) { Debug.Log(">>> Test: EI Panel"); manager.OnEIPanelOpened(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { Debug.Log(">>> Test: Main Power"); manager.OnMainPowerPanelOpened(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { Debug.Log(">>> Test: OCB OFF"); manager.OnOCBSwitchedOff(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { Debug.Log(">>> Test: RPS"); manager.OnRPSChecked(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { Debug.Log(">>> Test: F5C OFF"); manager.OnF5CSwitchedOff(); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { Debug.Log(">>> Test: UP Button"); manager.OnUpButtonPressed(); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { Debug.Log(">>> Test: KEY1"); manager.OnKey1ButtonPressed(); }
    }
}

