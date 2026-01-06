using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;
    private InputAction lookAction;
    private InputAction switchViewAction;

    [Header("References")]
    public Transform player;          // Cowboy root (assign in inspector)
    [Tooltip("Optional: used only for pivot height. If null, player's position + y from offsets will be used.")]
    public Transform cameraPivot;     // optional; can be null

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public float minPitch = -40f;
    public float maxPitch = 80f;

    [Header("View Positions")]
    // thirdPersonOffset.z should be negative so camera sits behind the pivot
    public Vector3 thirdPersonOffset = new Vector3(0f, 1.6f, -3f);
    public Vector3 firstPersonOffset = new Vector3(0f, 1.6f, 0f);

    private float yaw;
    private float pitch;
    private bool isFirstPerson = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var playerMap = inputActions.FindActionMap("Player");

        lookAction = playerMap.FindAction("Look");
        switchViewAction = playerMap.FindAction("SwitchView");

        playerMap.Enable();

        // initialize yaw/pitch from current transforms so it doesn't snap on start
        yaw = player.eulerAngles.y;
        float initialPitch = transform.eulerAngles.x;
        if (initialPitch > 180f) initialPitch -= 360f;
        pitch = Mathf.Clamp(initialPitch, minPitch, maxPitch);

        // Minor safety: ensure thirdPersonOffset.z is not zero; negative recommended
        if (Mathf.Approximately(thirdPersonOffset.z, 0f))
            thirdPersonOffset.z = -3f;
    }

    void Update()
    {
        ReadLookInput();
        HandleViewSwitch();
    }

    void LateUpdate()
    {
        ApplyCameraTransform();
    }

    void ReadLookInput()
    {
        if (lookAction == null) return;
        Vector2 lookInput = lookAction.ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;
        yaw += lookInput.x;
        pitch -= lookInput.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void ApplyCameraTransform()
    {
        if (player == null) return;

        // pivot world position (use cameraPivot if assigned, otherwise compute from player + y)
        Vector3 pivotWorld = (cameraPivot != null)
            ? cameraPivot.position
            : (player.position + Vector3.up * (isFirstPerson ? firstPersonOffset.y : thirdPersonOffset.y));

        if (isFirstPerson)
        {
            // Rotate the player to match yaw (so player actually turns with mouse)
            player.rotation = Quaternion.Euler(0f, yaw, 0f);

            // Camera world position = player's transform * local offset (respects player's rotation)
            Vector3 fpLocal = firstPersonOffset;
            Vector3 cameraWorldPos = player.TransformPoint(fpLocal);
            transform.position = cameraWorldPos;

            // Camera rotation: look direction comes from pitch + player yaw
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            transform.rotation = rot;
        }
        else
        {
            // Third-person orbit:
            // Build rotation from pitch and yaw and compute camera position relative to pivot
            Quaternion orbitRot = Quaternion.Euler(pitch, yaw, 0f);

            // local offset from pivot: use x and z from thirdPersonOffset, y handled in pivotWorld
            Vector3 localOffset = new Vector3(thirdPersonOffset.x, 0f, thirdPersonOffset.z);

            // camera position in world
            Vector3 cameraWorldPos = pivotWorld + (orbitRot * localOffset);
            transform.position = cameraWorldPos;

            // Look at the pivot point (so camera faces the player)
            transform.LookAt(pivotWorld, Vector3.up);
            // Do NOT rotate player here — player rotation remains controlled elsewhere (e.g. movement)
        }
    }

    void HandleViewSwitch()
    {
        if (switchViewAction != null && switchViewAction.triggered)
        {
            isFirstPerson = !isFirstPerson;

            // When switching to first person, align yaw to player so there's no sudden snap
            if (isFirstPerson)
            {
                yaw = player.eulerAngles.y;
            }
            else
            {
                // when entering third person, try to keep camera behind player initially
                yaw = player.eulerAngles.y;
            }
        }
    }
}
