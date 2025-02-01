using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using Oculus.Interaction;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class FloatingCountdown : MonoBehaviour {
    private TextMeshProUGUI instructionText;
    [SerializeField] private GameObject[] targetObjects;  // Assign the 4 components in the inspector
    public float textDistance = 20.0f; // Distance from the camera
    private int currentCycle = 0;
    private XRHandSubsystem handSubsystem;

    private float maxHandOpenness;
    private float totalHandOpenness;
    private int opennessSamples;
    private float openTime;

    void Start() {
        CreateFloatingText();
        StartCoroutine(CountdownCoroutine());

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

    void CreateFloatingText() {
        // Create a Canvas
        GameObject canvasObject = new GameObject("FloatingCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 5000f;  // Adjust scaling to make text smaller

        canvasObject.transform.SetParent(Camera.main.transform);
        canvasObject.transform.localPosition = new Vector3(0, 0, textDistance);
        canvasObject.transform.localRotation = Quaternion.identity;
        canvasObject.transform.localScale = Vector3.one * 0.0001f;  // Adjust scale for better visibility

        // Create a TextMeshProUGUI component
        GameObject textObject = new GameObject("InstructionText");
        textObject.transform.SetParent(canvasObject.transform);

        instructionText = textObject.AddComponent<TextMeshProUGUI>();
        instructionText.fontSize = 1;  // Smaller font size
        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.color = new Color(1, 1, 1, 1); // White text with full opacity

        // Set the text position and size
        RectTransform rectTransform = instructionText.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(50, 15);
        rectTransform.anchoredPosition3D = Vector3.zero;

        // Initially set text to empty
        instructionText.text = "";
    }

    IEnumerator CountdownCoroutine() {
        if (targetObjects.Length == 0) {
            Debug.LogError("No objects assigned to targetObjects array!");
            yield break;
        }

        while (currentCycle < targetObjects.Length) {
            GameObject currentObject = targetObjects[currentCycle];
            string objectName = currentObject.name;
            instructionText.text = "3";
            yield return new WaitForSeconds(1);
            instructionText.text = "2";
            yield return new WaitForSeconds(1);
            instructionText.text = "1";
            yield return new WaitForSeconds(1);
            instructionText.text = $"Pick up the {objectName}!";
            yield return new WaitForSeconds(1.5f);
            instructionText.text = "";

            maxHandOpenness = 0f;
            totalHandOpenness = 0f;
            opennessSamples = 0;
            openTime = 0f;

            GrabFreeTransformer grabTransformer = currentObject.GetComponentInChildren<GrabFreeTransformer>();
            if (grabTransformer != null) {
                FieldInfo isGrabbedField = grabTransformer.GetType().GetField("isGrabbed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (isGrabbedField != null) {
                    while (!(bool)isGrabbedField.GetValue(grabTransformer)) {
                        TrackHandOpenness();
                        yield return null;
                    }
                    Debug.Log($"Time to grab: {openTime:F2} seconds");
                    Debug.Log($"Max openness before grab: {maxHandOpenness:F2}");

                    while ((bool)isGrabbedField.GetValue(grabTransformer)) {
                        TrackHandOpenness();
                        yield return null;
                    }
                    float averageOpenBeforeRelease = totalHandOpenness / opennessSamples;
                    Debug.Log($"Avg openness before release: {averageOpenBeforeRelease:F2}");
                    Debug.Log($"Max openness before release: {maxHandOpenness:F2}");
                }
            }

            yield return new WaitForSeconds(1);  // Wait 1 second after releasing
            currentCycle++;
        }

        instructionText.text = "";  // Clear the text after all cycles
    }

    void TrackHandOpenness() {
        if (handSubsystem != null && handSubsystem.rightHand.isTracked) {
            float openness = CalculateHandOpenness(handSubsystem.rightHand);
            maxHandOpenness = Mathf.Max(maxHandOpenness, openness);
            totalHandOpenness += openness;
            opennessSamples++;
            openTime += Time.deltaTime;
        }
    }

    private float CalculateHandOpenness(XRHand hand) {
        XRHandJoint indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        XRHandJoint thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);

        Pose indexPose, thumbPose;

        if (indexTip.TryGetPose(out indexPose) && thumbTip.TryGetPose(out thumbPose)) {
            float distance = Vector3.Distance(indexPose.position, thumbPose.position);
            float maxDistance = 0.15f;
            float minDistance = 0.02f;
            float openness = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
            return openness * 9 + 1;
        }
        return 1f;
    }
}
