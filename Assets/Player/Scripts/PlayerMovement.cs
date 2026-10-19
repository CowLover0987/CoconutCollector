using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{

    // MOVEMENT SETTINGS
    public float lookSensitivity = 20f;       // How fast the camera rotates
    public float gravity = -9.81f;           // Gravity force applied manually
    public float jumpHeight = 3f;          // How high the player can jump - default 0.5f
    public float walkSpeed = 6f;
    public float runSpeed = 12f;

    private int doubleJumpCount = 0;            // Tracks how many jumps have been performed

    // CAMERA REFERENCES
    public Transform cameraTransform;        // The actual Camera object
    public Transform target;                 // The player position the camera rig follows
    public float followSpeed = 5f;           // Smoothness of camera follow

    // COMPONENTS
    private CharacterController controller;  // Handles movement without physics
    private Animator animator;               // Controls animations
    private AnimatorStateInfo state;         // Tracks the current animation state

    // INPUT
    private Vector2 input;                   // Movement input (WASD or stick)
    private Vector2 lookInput;               // Look input (mouse or stick)

    // CAMERA STATE
    private float cameraPitch = 0f;          // Vertical camera rotation

    // MOVEMENT STATE
    private float verticalVelocity = 0f;     // Tracks falling speed for gravity
    public bool canMove = true;                     // Prevents movement when false

    //PLAYER STATS
    public int points = 0; // Player's points
    public TMP_Text pointsText; // Reference to the UI text element

    void Start()
    {
        // Get required components on the player
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        state = animator.GetCurrentAnimatorStateInfo(0);
    }

    void Update()
    {
        //// ============================================================
        //// CAMERA ROTATION (HORIZONTAL AND VERTICAL)
        //// ============================================================
        //// Rotate the camera rig horizontally (mouse X)
        //if (settings.activeSelf || MiniDialogue.activeSelf || canMove == false || ObjectiveMenuUI.activeSelf)
        //{
        //    return; // Skip camera rotation if dialogue is active
        //}
        //else
        //{
        //    // Rotate the camera rig horizontally (mouse X)
        //    Transform camRig = cameraTransform.parent;
        //    camRig.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        //    // Rotate the camera vertically (mouse Y)
        //    cameraPitch -= lookInput.y * lookSensitivity;
        //    cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f); // Prevent flipping
        //    cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        //}

        // Rotate the camera rig horizontally (mouse X)
        //Transform camRig = cameraTransform.parent;
        //camRig.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        //// Rotate the camera vertically (mouse Y)
        //cameraPitch -= lookInput.y * lookSensitivity;
        //cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f); // Prevent flipping
        //cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);

        Transform camRig = cameraTransform.parent;

        Quaternion targetRot = Quaternion.Euler(
            0f,
            transform.eulerAngles.y,
            0f
        );

        camRig.rotation = Quaternion.Slerp(
            camRig.rotation,
            targetRot,
            10f * Time.deltaTime
        );

        // ============================================================
        // MOVEMENT INPUT TO CAMERA-RELATIVE MOVEMENT
        // ============================================================

        // Raw input movement (X = left/right, Y = forward/back)
        Vector3 move = new Vector3(input.x, 0, input.y);

        // Convert movement so "forward" means "camera forward"
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // Remove vertical tilt from camera so movement stays flat
        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        // Final camera-relative movement direction
        move = camForward * move.z + camRight * move.x;


        // ============================================================
        // ANIMATION STATE
        // ============================================================

        // Player is walking if movement vector is not tiny
        bool isWalking = move.sqrMagnitude > 0.01f;
        animator.SetBool("IsWalking", isWalking);


        float currentSpeed = walkSpeed;

        // ============================================================
        // GRAVITY
        // ============================================================
        // If grounded, reset falling speed so player sticks to ground
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f; // Small downward force keeps grounding stable

        }
        else
        {
            // Apply gravity over time when in the air
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Add gravity to movement vector
        // Horizontal movement
        Vector3 horizontal = new Vector3(move.x, 0, move.z) * currentSpeed;

        // Vertical movement
        Vector3 vertical = new Vector3(0, verticalVelocity, 0);

        // Combine
        Vector3 finalMove = horizontal + vertical;


        // ============================================================
        // MOVE PLAYER
        // ============================================================

        // Apply movement (horizontal * speed + vertical gravity)
        controller.Move(finalMove * Time.deltaTime);


        // ============================================================
        // ROTATE PLAYER TOWARD MOVEMENT DIRECTION
        // ============================================================

        // Ignore vertical movement when deciding rotation
        Vector3 flatMove = horizontal;
        flatMove.y = 0;


        // Rotate only if actually moving
        if (flatMove.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatMove);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }
    }

    // Called automatically by the Input System when movement input changes
    private void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    // Called automatically by the Input System when look input changes
    //


    // ============================================================
    // JUMP FUNCTION
    // ============================================================
    private void OnJump()
    {
        if (controller.isGrounded || doubleJumpCount < 2)
        {
            animator.SetTrigger("Jump");
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            doubleJumpCount++;
        }

        if (controller.isGrounded &&doubleJumpCount >= 2)
        {
            doubleJumpCount = 0; // Reset jump count after two jumps
        }
    }

    // ============================================================
    // CAMERA RESET FUNCTION
    // ============================================================
    private void OnCameraReset()
    {
        // Reset horizontal rotation: camera rig matches player facing direction
        Transform camRig = cameraTransform.parent;
        camRig.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        // Reset vertical rotation: look straight ahead
        cameraPitch = 18f;
        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }


    // ============================================================
    // INTERACTION
    // ============================================================

    public void PointCollected()
    {
        points++;
        pointsText.text = points.ToString();
    }

    // ============================================================
    // SETTINGS MENU
    // ============================================================
}
