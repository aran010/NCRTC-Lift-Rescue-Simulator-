using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class ControlPanelPhysics : MonoBehaviour
{
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.mass = 50f; // Give it some weight
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;

        // Ensure collider fits (rudimentary check, Unity usually handles this well on import if generated colliders are checked, 
        // but this script ensures there's at least a BoxCollider)
        if (GetComponent<BoxCollider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }
}
