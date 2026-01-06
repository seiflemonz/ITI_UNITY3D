using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerHorseMount : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction runAction;
    private InputAction interactAction;
    private InputAction jumpAction;

    [Header("Mount Settings")]
    public string mountBoneName = "Spine1_2_M";
    public Vector3 mountedLocalPosition;
    public Vector3 mountedLocalRotation;
    public float dismountSideOffset = 1.5f;

    private CharacterController controller;
    private Animator animator;

    private GameObject nearbyHorse;
    private HorseController currentHorse;

    private bool isMounted;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        var playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        runAction = playerMap.FindAction("Run");
        interactAction = playerMap.FindAction("Interact");
        jumpAction = playerMap.FindAction("Jump");

        playerMap.Enable();
    }

    void Update()
    {
        HandleMountInput();
        SendInputToHorse();
    }

    // ---------------- MOUNT / DISMOUNT ----------------

    void HandleMountInput()
    {
        if (!interactAction.triggered) return;

        if (!isMounted && nearbyHorse != null)
            Mount();
        else if (isMounted)
            Dismount();
    }

    void Mount()
    {
        isMounted = true;

        animator.SetBool("Ride", true);
        controller.enabled = false;

        currentHorse = nearbyHorse.GetComponent<HorseController>();
        currentHorse.SetMounted(true);

        // Find mount bone directly from horse hierarchy
        Transform mountBone = nearbyHorse.transform.Find("Main/DeformationSystem/Root_M/Spine1_2_M");
        if (mountBone == null)
        {
            Debug.LogError($"Mount bone '{mountBoneName}' not found under horse '{nearbyHorse.name}'!");
            return;
        }

        transform.SetParent(mountBone);
        transform.localPosition = mountedLocalPosition;
        transform.localRotation = Quaternion.Euler(mountedLocalRotation);
    }


    void Dismount()
    {
        isMounted = false;

        animator.SetBool("Ride", false);

        transform.SetParent(null);
        controller.enabled = true;

        transform.position =
            nearbyHorse.transform.position +
            nearbyHorse.transform.right * dismountSideOffset;

        currentHorse.SetMounted(false);
        currentHorse = null;
    }

    // ---------------- INPUT BRIDGE ----------------

    void SendInputToHorse()
    {
        if (!isMounted || currentHorse == null) return;

        Vector2 move = moveAction.ReadValue<Vector2>();
        bool running = runAction.IsPressed();
        bool jump = jumpAction.IsPressed();

        currentHorse.SetInput(move, running,jump);
    }

    // ---------------- TRIGGER ----------------

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Horse"))
            nearbyHorse = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Horse") && other.gameObject == nearbyHorse)
            nearbyHorse = null;
    }
}
