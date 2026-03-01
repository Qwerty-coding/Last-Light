using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public float speed = 5f;
    public float sprintSpeed = 10f;       // NEW: sprint speed
    public float gravity = -30f;
    public float jumpHeight = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    [Range(0.1f, 1f)] public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    // ================= FALL DAMAGE =================
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
    bool wasGrounded;

    PlayerHealth playerHealth;
    // ================================================

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        // Detect start of fall
        if (!isGrounded && wasGrounded)
            fallStartY = transform.position.y;

        // Detect landing
        if (isGrounded && !wasGrounded)
        {
            lastFallDistance = fallStartY - transform.position.y;
            ApplyFallDamage(lastFallDistance);
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // NEW: Sprint when holding Left Shift and moving forward
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && z > 0.1f && isGrounded;
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

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
            playerHealth.TakeDamage(damage);
    }
}