//using Oculus.Interaction.Grab;  // Specific for grabbing interactions
//using UnityEngine;
//using Oculus.Interaction;  // Ensure you're using the correct namespace for Meta Interaction SDK

//public class GrabbedLogger : MonoBehaviour {
//    private ISDK_HandGrabInteractable handGrabInteractable;
//    private GrabFreeTransformer grabFreeTransformer;

//    void Start() {
//        // Get the ISDK_HandGrabInteractable component attached to the object
//        handGrabInteractable = GetComponent<ISDK_HandGrabInteractable>();

//        if (handGrabInteractable == null) {
//            Debug.LogError("ISDK_HandGrabInteractable component not found on the object!");
//            return;
//        }

//        // Access the Grab Free Transformer inside ISDK_HandGrabInteractable
//        grabFreeTransformer = handGrabInteractable.Transformer as GrabFreeTransformer;

//        if (grabFreeTransformer == null) {
//            Debug.LogError("GrabFreeTransformer not found in ISDK_HandGrabInteractable!");
//            return;
//        }
//    }

//    void Update() {
//        if (grabFreeTransformer.isGrabbed) {
//            Debug.Log("GRABBED!");
//        }
//    }
//}