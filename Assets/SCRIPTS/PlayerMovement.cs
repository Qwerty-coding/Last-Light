using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float gravity = -30f;
    public float jumpHeight = 3f;

    // Ground Check variables removed as requested

    [Header("Animation")]
    public Animator animator;

    // Private variables
    Vector3 velocity;
    bool isGrounded;
    bool wasGrounded = true;

    // Fall damage tracking
    [Header("Fall Damage Tuning")]
    [Tooltip("No damage below this fall height (meters)")]
    [Range(0f, 10f)]
    public float minFallDistance = 3f;

    [Tooltip("Fall height that causes max damage")]
    [Range(1f, 30f)]
    public float maxFallDistance = 10f;

    [Tooltip("Damage applied at max fall height")]
    [Range(1f, 200f)]
    public float maxFallDamage = 100f;

    [Header("Debug (Read Only)")]
    [SerializeField] float lastFallDistance;

    float fallStartY;
    PlayerHealth playerHealth;

    void Start()
    {
        // Auto-find animator if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // Get PlayerHealth component
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // NEW JUMP LOGIC: Use Unity's built-in CharacterController ground detection
        isGrounded = controller.isGrounded;

        // Update animator with ground state
        if (animator != null)
        {
            animator.SetBool("isGrounded", isGrounded);
        }

        // Detect start of fall
        if (!isGrounded && wasGrounded)
        {
            fallStartY = transform.position.y;
        }

        // Detect landing
        if (isGrounded && !wasGrounded)
        {
            lastFallDistance = fallStartY - transform.position.y;
            ApplyFallDamage(lastFallDistance);
            Debug.Log("Player landed");
        }

        // Reset downward velocity when touching the floor
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Determine current speed based on input
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        // ====== UPDATE ANIMATOR ======
        // Calculate movement magnitude for animation
        float movementMagnitude = move.magnitude * currentSpeed;
        
        // Update Speed parameter in animator
        if (animator != null)
        {
            animator.SetFloat("Speed", movementMagnitude);
        }
        // =============================

        // Jump input - Calculate Y velocity before moving!
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            // Trigger jump animation
            if (animator != null)
            {
                animator.SetTrigger("Jump");
                Debug.Log("Jump triggered");
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // CRITICAL FIX: Combine horizontal and vertical movement into ONE Move() call.
        Vector3 finalMove = (move * currentSpeed) + velocity;
        controller.Move(finalMove * Time.deltaTime);

        // Store previous grounded state
        wasGrounded = isGrounded;
    }

    void ApplyFallDamage(float fallDistance)
    {
        if (fallDistance < minFallDistance)
            return;

        float percent = Mathf.InverseLerp(
            minFallDistance,
            maxFallDistance,
            fallDistance
        );

        float damage = percent * maxFallDamage;

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log($"Fall damage: {damage} (fell {fallDistance}m)");
        }
    }
}