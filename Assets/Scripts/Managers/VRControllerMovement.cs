using UnityEngine;

public class VRControllerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Transform playerTransform;
    public float moveSpeed = 1.5f; // Adjust this value to control the movement speed.
    private float rotationSpeed = 2.0f;
    private OVRCameraRig ovrCameraRig;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerTransform = transform;
        ovrCameraRig = FindObjectOfType<OVRCameraRig>(); // Find the OVRCameraRig component in the scene.
    }

    void Update()
    {
        // Get the forward direction of the HMD (Head Mounted Display) from the OVRCameraRig.
        Vector3 hmdForward = ovrCameraRig.centerEyeAnchor.forward;
        hmdForward.y = 0f;
        hmdForward.Normalize();

        // Get the thumbstick input from both left and right controllers.
        Vector2 leftThumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector2 rightThumbstickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // Movement control with left thumbstick (forward, backward, sideways).
        Vector3 moveDirection = (hmdForward * leftThumbstickInput.y) + (playerTransform.right * leftThumbstickInput.x);

        // Rotation control with right thumbstick.
        playerTransform.Rotate(Vector3.up * rightThumbstickInput.x * rotationSpeed);

        // Make the player move in the direction of the HMD facing.
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
}
