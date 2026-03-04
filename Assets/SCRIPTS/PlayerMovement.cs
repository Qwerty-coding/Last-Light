using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float gravity = -30f;
    public float jumpHeight = 3f;

    [Header("Animation")]
    public Animator animator;

    Vector3 velocity;
    bool isGrounded;
    bool wasGrounded = true;

    [Header("Fall Damage Tuning")]
    [Range(0f, 10f)]  public float minFallDistance = 3f;
    [Range(1f, 30f)]  public float maxFallDistance = 10f;
    [Range(1f, 200f)] public float maxFallDamage = 100f;
    [SerializeField]  float lastFallDistance;

    float fallStartY;
    PlayerHealth playerHealth;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        // Fall tracking
        if (!isGrounded && wasGrounded)
            fallStartY = transform.position.y;

        if (isGrounded && !wasGrounded)
        {
            lastFallDistance = fallStartY - transform.position.y;
            ApplyFallDamage(lastFallDistance);
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? runSpeed : walkSpeed;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null)
                animator.SetTrigger("Jump");        // "Jump" trigger -> JumpStart state
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move((move * currentSpeed + velocity) * Time.deltaTime);

        wasGrounded = isGrounded;

        // ---- ANIMATOR ----
        if (animator != null)
        {
            float moveMag = new Vector2(x, z).magnitude; // 0 to 1

            // 0 = idle, 0.5 = walk, 1 = run  (normalized so Blend Tree thresholds work)
            float animSpeed = 0f;
            if (moveMag > 0.1f)
                animSpeed = isSprinting ? 1f : 0.5f;

            animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
            animator.SetBool("isGrounded", isGrounded);  // "isGrounded" bool
        }
    }

    void ApplyFallDamage(float fallDistance)
    {
        if (fallDistance < minFallDistance) return;
        float percent = Mathf.InverseLerp(minFallDistance, maxFallDistance, fallDistance);
        float damage = percent * maxFallDamage;
        if (playerHealth != null)
            playerHealth.TakeDamage(damage);
    }
}