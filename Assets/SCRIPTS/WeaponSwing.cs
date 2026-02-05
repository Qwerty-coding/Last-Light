using UnityEngine;

public class WeaponSwing : MonoBehaviour
{
    [Header("Swing Animation")]
    public float swingAngle = 50f;
    public float swingSpeed = 20f;
    
    [Header("Combat Settings")]
    public float chopRange = 3f; // Increased from 2f
    public float damageToZombie = 25f;
    public float damageToTree = 1f; // Trees use "Chop()" which reduces health by 1
    
    [Header("Detection Settings")]
    public LayerMask targetLayers; // Set this in Inspector to include trees and zombies
    public bool debugMode = true; // Enable to see raycast visualization

    private float currentAngle = 0f;
    private bool swingingForward = false;
    private bool swingingBack = false;
    private bool hasDealtDamage = false; // NEW: Prevent multiple hits per swing

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
        
        // Auto-setup layer mask if not configured
        if (targetLayers.value == 0)
        {
            targetLayers = LayerMask.GetMask("Default"); // Adjust to your actual layers
            Debug.LogWarning("WeaponSwing: targetLayers not set! Using Default layer.");
        }
    }

    void Update()
    {
        // Start swing on left click
        if (Input.GetMouseButtonDown(0) && !swingingForward && !swingingBack)
        {
            swingingForward = true;
            currentAngle = 0f;
            hasDealtDamage = false; // Reset damage flag for new swing
        }

        // Swing forward
        if (swingingForward)
        {
            float step = swingSpeed * Time.deltaTime;
            transform.Rotate(0, step, 0);
            currentAngle += step;

            // Try chopping during a WIDER window (30% to 70% of swing)
            if (currentAngle >= swingAngle * 0.3f && currentAngle < swingAngle * 0.7f)
            {
                if (!hasDealtDamage) // Only damage once per swing
                {
                    TryChop();
                }
            }

            if (currentAngle >= swingAngle)
            {
                swingingForward = false;
                swingingBack = true;
            }
        }
        // Swing back
        else if (swingingBack)
        {
            float step = swingSpeed * Time.deltaTime;
            transform.Rotate(0, -step, 0);
            currentAngle -= step;

            if (currentAngle <= 0f)
            {
                swingingBack = false;
                transform.localRotation = startRotation;
            }
        }
    }

    void TryChop()
    {
        // Get camera transform
        Transform camTransform = Camera.main.transform;
        
        // Method 1: Raycast from camera center
        Ray centerRay = new Ray(camTransform.position, camTransform.forward);
        RaycastHit centerHit;
        
        if (debugMode)
        {
            Debug.DrawRay(centerRay.origin, centerRay.direction * chopRange, Color.red, 0.5f);
        }

        if (Physics.Raycast(centerRay, out centerHit, chopRange, targetLayers))
        {
            ProcessHit(centerHit);
            return; // Found something, stop checking
        }

        // Method 2: SphereCast for more forgiving detection
        if (Physics.SphereCast(camTransform.position, 0.3f, camTransform.forward, out centerHit, chopRange, targetLayers))
        {
            ProcessHit(centerHit);
            return;
        }

        // Method 3: Check multiple rays in a cone (most forgiving)
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3 direction = camTransform.forward + 
                                  camTransform.right * (i * 0.1f) + 
                                  camTransform.up * (j * 0.1f);
                
                Ray coneRay = new Ray(camTransform.position, direction.normalized);
                RaycastHit coneHit;
                
                if (debugMode)
                {
                    Debug.DrawRay(coneRay.origin, coneRay.direction * chopRange, Color.yellow, 0.2f);
                }
                
                if (Physics.Raycast(coneRay, out coneHit, chopRange, targetLayers))
                {
                    ProcessHit(coneHit);
                    return;
                }
            }
        }

        if (debugMode)
        {
            Debug.Log("Swing missed - no valid target in range");
        }
    }

    void ProcessHit(RaycastHit hit)
    {
        if (hasDealtDamage) return; // Already damaged something this swing
        
        // --- TREE LOGIC ---
        TreeInteractable tree = hit.collider.GetComponentInParent<TreeInteractable>();
        if (tree != null)
        {
            tree.Chop();
            hasDealtDamage = true;
            if (debugMode) Debug.Log("✓ HIT TREE: " + tree.treeName);
            return;
        }

        // --- ZOMBIE LOGIC ---
        Zombie1 zombie1 = hit.collider.GetComponent<Zombie1>();
        if (zombie1 != null)
        {
            zombie1.TakeDamage(damageToZombie);
            hasDealtDamage = true;
            if (debugMode) Debug.Log("✓ HIT ZOMBIE (Zombie1 script)");
            return;
        }

        // Fallback to old Zombie script
        Zombie zombie = hit.collider.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.TakeDamage((int)damageToZombie);
            hasDealtDamage = true;
            if (debugMode) Debug.Log("✓ HIT ZOMBIE (Zombie script)");
            return;
        }

        // --- BOSS ZOMBIE LOGIC ---
        BossZombie boss = hit.collider.GetComponent<BossZombie>();
        if (boss != null)
        {
            boss.TakeDamage(damageToZombie);
            hasDealtDamage = true;
            if (debugMode) Debug.Log("✓ HIT BOSS ZOMBIE");
            return;
        }

        if (debugMode)
        {
            Debug.Log("Hit object with no valid script: " + hit.collider.name);
        }
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * chopRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Camera.main.transform.position + Camera.main.transform.forward * chopRange, 0.3f);
    }
}