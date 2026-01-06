using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HorseController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float turnSpeed = 120f;

    private Rigidbody rb;
    private Animator animator;

    private Vector2 moveInput;
    private bool isRunning;
    private bool isMounted;
    private bool isJumping;
    private bool isGrounded;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        if (!isMounted) return;

        float speed = isRunning ? runSpeed : walkSpeed;

        // Forward movement
        Vector3 move = transform.forward * moveInput.y * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        // Turn
        float turn = moveInput.x * turnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));

        // Animator IWR
        float iwr = 0f;
        if (Mathf.Abs(moveInput.y) > 0.1f)
            iwr = isRunning ? 1f : 0.5f;

        animator.SetFloat("IWR", iwr, 0.1f, Time.deltaTime);
        if (isJumping && isGrounded)
        {
            isJumping = false;

            rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
        }
    }

    // ---------------- INPUT FROM PLAYER ----------------

    public void SetInput(Vector2 move, bool running,bool jump)
    {
        moveInput = move;
        isRunning = running;
        isJumping = jump;
    }

    public void SetMounted(bool mounted)
    {
        isMounted = mounted;

        if (!mounted)
            animator.SetFloat("IWR", 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded=true;
        }
        Rigidbody rb = collision.collider.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            rb.AddForce(transform.forward*2, ForceMode.Impulse);
            Debug.Log("Force Added to Rigidbody via Horse hit");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        { isGrounded=false; }
    }
}
