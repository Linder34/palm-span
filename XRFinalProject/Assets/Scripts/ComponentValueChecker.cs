using UnityEngine;
using System.Reflection;  // Required for BindingFlags and reflection
using Oculus.Interaction;  // Add this at the top of your script


public class ComponentValueChecker : MonoBehaviour {
    [SerializeField] private GameObject targetObject;  // Assign your object in the Inspector

    private Component targetComponent;

    void Start() {
        // Find the GrabFreeTransformer component in children
        GrabFreeTransformer grabTransformer = targetObject.GetComponentInChildren<GrabFreeTransformer>();

        if (grabTransformer != null) {
            Debug.Log("GrabFreeTransformer found!");

            // Use reflection to get the isGrabbed field
            FieldInfo isGrabbedField = grabTransformer.GetType().GetField("isGrabbed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (isGrabbedField != null) {
                bool isGrabbedValue = (bool)isGrabbedField.GetValue(grabTransformer);
                Debug.Log($"isGrabbed value: {isGrabbedValue}");
            }
            else {
                Debug.LogError("Field 'isGrabbed' not found in GrabFreeTransformer.");
            }
        }
        else {
            Debug.LogError("GrabFreeTransformer component not found on target object or its children.");
        }
    }

    void Update() {
        // Example of continuously checking the isGrabbed state
        GrabFreeTransformer grabTransformer = targetObject.GetComponentInChildren<GrabFreeTransformer>();

        if (grabTransformer != null) {
            FieldInfo isGrabbedField = grabTransformer.GetType().GetField("isGrabbed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (isGrabbedField != null) {
                bool isGrabbedValue = (bool)isGrabbedField.GetValue(grabTransformer);
                Debug.Log($"YOAV - isGrabbed value: {isGrabbedValue}");
            }
        }
    }
}