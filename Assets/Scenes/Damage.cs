using UnityEngine;

public class FallDamage : MonoBehaviour

{
    [Header("Fall Settings")]
    [SerializeField] private float minFallVelocity = -12f;
    [SerializeField] private float damageMultiplier = 10f;
    [SerializeField] private LayerMask groundLayer = 1;
    
    private Rigidbody rb;
    private float previousYVelocity;
    private bool wasGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // ADD THIS COMPLETE Update() METHOD HERE (replaces partial version)
    void Update()
    {
        bool isGrounded = Physics.CheckSphere(transform.position - transform.up * 0.9f, 0.5f, groundLayer);
        
        float currentYVelocity = rb.velocity.y;
        
        // Landing after damaging fall
        if (wasGrounded == false && isGrounded && previousYVelocity < minFallVelocity)
        {
            float fallSpeed = Mathf.Abs(previousYVelocity);
            float damage = fallSpeed * damageMultiplier;
            ApplyDamage(damage);
        }
        
        wasGrounded = isGrounded;
        previousYVelocity = currentYVelocity;
    }

    // ADD THIS METHOD HERE (right after Update)
    void ApplyDamage(float damage)
    {
        Debug.Log($"Fall Damage: {damage:F1}");
        // Replace with: GetComponent<PlayerHealth>().TakeDamage(damage);
    }
}


