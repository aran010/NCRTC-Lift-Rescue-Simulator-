using UnityEngine;

// Controls the lift physics and responds to control panel inputs
public class LiftPhysics : MonoBehaviour
{
    public Rigidbody liftRb;
    public float motorSpeed = 1.0f;
    public float inchingSpeed = 0.2f;

    [Header("Status")]
    public bool isInDoorZone = false;
    public float currentFloorLevel = 0f;
    
    // Safety Chain inputs
    public bool isRPSOn = false;
    public bool isF5COn = true; // Initially closed (ON)
    public bool isOCBOn = true; // Initially closed (ON)
    public bool isMROMode = false; // Rotary switch state

    private void FixedUpdate()
    {
        // Physics update if needed
    }

    // Called by BRB2 + UP button logic
    public void ManualInchUp()
    {
        if (CanMoveMRO())
        {
            liftRb.MovePosition(liftRb.position + Vector3.up * inchingSpeed * Time.fixedDeltaTime);
        }
    }

    // Called by BRB2 + DOWN button logic
    public void ManualInchDown()
    {
        if (CanMoveMRO())
        {
            liftRb.MovePosition(liftRb.position + Vector3.down * inchingSpeed * Time.fixedDeltaTime);
        }
    }

    private bool CanMoveMRO()
    {
        // Logic: specific safety chain for MRO
        // Typically: OCB OFF, RPS ON, Rotary MRO
        // Note: Real lifts are complex, this is simplified for the specific SOP provided
        return !isOCBOn && isRPSOn && isMROMode; 
    }
    
    // Updates from interactables
    public void SetOCB(bool state) { isOCBOn = state; }
    public void SetRPS(bool state) { isRPSOn = state; }
    public void SetF5C(bool state) { isF5COn = state; }
    public void SetMROMode(bool state) { isMROMode = state; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DoorZone"))
        {
            isInDoorZone = true;
            // Notify Manager?
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DoorZone"))
        {
            isInDoorZone = false;
        }
    }
}
