using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CowboyMove : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;

    [Header("Movement")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Grounding")]
    [SerializeField] private float groundedGraceTime = 0.15f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float rotationSmoothTime = 0.1f;

    private CharacterController controller;
    private Animator animator;

    private Vector2 moveInput;
    private Vector3 velocity;

    private bool isRunning;
    private bool isGrounded;
    private float groundedTimer;

    private float currentYaw;
    private float yawVelocity;

    [Header("Push Settings")]
    public float pushForce = 1f;
    public float pushDistance = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        var playerMap = inputActions.FindActionMap("Player");

        moveAction = playerMap.FindAction("Move");
        runAction = playerMap.FindAction("Run");
        jumpAction = playerMap.FindAction("Jump");

        playerMap.Enable();
    }

    void Update()
    {
        HandleMovementAndGravity();
        HandleAnimation();
    }

    // ---------------- MOVEMENT + GRAVITY ----------------

    void HandleMovementAndGravity()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        isRunning = runAction.IsPressed();

        Vector3 move = Vector3.zero;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();

            move = camForward * moveInput.y + camRight * moveInput.x;

            float targetYaw = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            currentYaw = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetYaw,
                ref yawVelocity,
                rotationSmoothTime
            );
            if(controller.enabled)
                transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
        }

        // ----- Ground check (BEFORE move) -----
        if (controller.isGrounded)
        {
            groundedTimer = groundedGraceTime;
            if (velocity.y < 0f)
                velocity.y = -2f;
        }
        else
        {
            groundedTimer -= Time.deltaTime;
        }

        isGrounded = groundedTimer > 0f;

        // ----- Jump -----
        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            groundedTimer = 0f;
            isGrounded = false;
            animator.SetTrigger("Jump");
        }

        // ----- Gravity -----
        velocity.y += gravity * Time.deltaTime;

        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 finalMove =
            move * speed +
            Vector3.up * velocity.y;

        if (controller.enabled)
        {
            controller.Move(finalMove * Time.deltaTime);

        }
    }

    // ---------------- ANIMATION ----------------

    void HandleAnimation()
    {
        float moveAmount = moveInput.magnitude;

        float iwr = 0f;
        if (moveAmount > 0.1f)
            iwr = isRunning ? 1f : 0.5f;

        animator.SetFloat("IWR", iwr, 0.1f, Time.deltaTime);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", velocity.y);

            
    }

    

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb != null && !rb.isKinematic && !hit.gameObject.CompareTag("Horse"))
        {
            rb.AddForce(transform.forward * pushForce, ForceMode.Impulse);
            Debug.Log("Force Added to Rigidbody via Controller hit");
        }
    }
}
