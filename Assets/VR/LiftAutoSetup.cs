#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

public class LiftAutoSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup Lift Scene")]
    public static void SetupScene()
    {
        // 1. Find the FBX Model
        GameObject liftPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PlatformLift.fbx");
        if (liftPrefab == null)
        {
            // Try to find it by name if path differs
            string[] guids = AssetDatabase.FindAssets("PlatformLift t:GameObject");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                liftPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
        }

        if (liftPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find 'PlatformLift.fbx' in Assets. Please drag the file into Unity first.", "OK");
            return;
        }

        // 2. Instantiate in Scene
        GameObject liftInstance = (GameObject)PrefabUtility.InstantiatePrefab(liftPrefab);
        liftInstance.name = "PlatformLift_Scene";
        Undo.RegisterCreatedObjectUndo(liftInstance, "Create Lift");

        // 3. Setup MRO Stand
        Transform mroStand = FindDeepChild(liftInstance.transform, "MRO_Stand");
        if (mroStand != null)
        {
            // Add ControlPanelPhysics
            if (mroStand.GetComponent<ControlPanelPhysics>() == null)
                Undo.AddComponent<ControlPanelPhysics>(mroStand.gameObject);

            // Add DebugMouseDrag
            if (mroStand.GetComponent<DebugMouseDrag>() == null)
                Undo.AddComponent<DebugMouseDrag>(mroStand.gameObject);

            // Try Add XR Grab Interactable (Reflection to avoid compile error if package missing)
            AddXRComponent(mroStand.gameObject);
        }
        else
        {
            Debug.LogError("Could not find 'MRO_Stand' inside the lift model.");
        }

        // 4. Setup Environment Colliders (Floor/Walls) so things don't fall
        SetupEnvironment(liftInstance);

        // 5. Cleanup Camera
        if (Camera.main != null)
        {
            // Don't delete, just log
            Debug.Log("Note: Main Camera exists. If setting up VR, consider removing it.");
        }
        
        Selection.activeGameObject = liftInstance;
        EditorUtility.DisplayDialog("Success", "Lift Setup Complete!\n\nAdded colliders to Floor/Walls.\nAdded Physics/VR to MRO_Stand.", "OK");
    }

    private static void SetupEnvironment(GameObject root)
    {
        // Try to find Room parts and add MeshColliders
        string[] envNames = { "Room_Floor", "Room_Ceiling", "Wall_Back", "Wall_Left", "Wall_Right" };
        foreach (string name in envNames)
        {
            Transform t = FindDeepChild(root.transform, name);
            if (t != null)
            {
                if (t.GetComponent<Collider>() == null)
                    Undo.AddComponent<MeshCollider>(t.gameObject);
                
                // Also set static for performance/baking if needed, but strictly optional
                // GameObjectUtility.SetStaticEditorFlags(t.gameObject, StaticEditorFlags.NavigationStatic | StaticEditorFlags.OccluderStatic);
            }
        }
    }

    private static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }

    private static void AddXRComponent(GameObject target)
    {
        // We use reflection so this script doesn't break if you haven't installed the XR Toolkit yet.
        System.Type xrType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable, Unity.XR.Interaction.Toolkit");
        
        // Try legacy namespace/assembly if failed (names changed in Unity 2024/XR Toolkit 3.0)
        if (xrType == null)
             xrType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable, Unity.XR.Interaction.Toolkit");

        if (xrType != null)
        {
            if (target.GetComponent(xrType) == null)
            {
                Undo.AddComponent(target, xrType);
                Debug.Log("Added XR Grab Interactable");
            }
        }
        else
        {
            Debug.LogWarning("XR Interaction Toolkit not found. Please install the package 'com.unity.xr.interaction.toolkit' manually to enable VR grabbing.");
        }
    }
}
#endif
