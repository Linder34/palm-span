using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class RightHandOpennessChecker : MonoBehaviour {
    private XRHandSubsystem handSubsystem;

    void Start() {
        if (XRGeneralSettings.Instance.Manager.activeLoader != null) {
            handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
        }

        if (handSubsystem == null) {
            Debug.LogError("XR Hand Tracking subsystem not found. Make sure XR Hands package is installed and enabled.");
        }
        else if (!handSubsystem.running) {
            handSubsystem.Start();
        }
    }

    void Update() {
        Debug.Log("Entered Update!");
        if (handSubsystem != null && handSubsystem.rightHand.isTracked) {
            Debug.Log("Entered first if!");
            float normalizedOpenness = CalculateHandOpenness(handSubsystem.rightHand);
            Debug.Log($"Hand openness (1-10): {normalizedOpenness:F0}");
            
            if (normalizedOpenness <= 3) {
                Debug.Log("Hand is closed!");
            }
        }
    }

    private float CalculateHandOpenness(XRHand hand) {
        XRHandJoint indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        XRHandJoint thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);

        Pose indexPose, thumbPose;

        // Verify if the joints are valid before accessing them
        if (indexTip.TryGetPose(out indexPose) && thumbTip.TryGetPose(out thumbPose)) {
            float distance = Vector3.Distance(indexPose.position, thumbPose.position);
            float maxDistance = 0.15f;  // Assumed max distance when hand is fully open
            float minDistance = 0.02f;  // Assumed min distance when hand is closed
            
            float openness = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
            float scaledOpenness = openness * 9 + 1;  // Scale from 0-1 to 1-10
            return scaledOpenness;
        }

        return 1f;  // Default to fully closed
    }
}
