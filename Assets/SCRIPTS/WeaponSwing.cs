using UnityEngine;

public class WeaponSwing : MonoBehaviour
{
    [Header("Swing Animation")]
    public float swingAngle = 50f;
    public float swingSpeed = 20f;

    [Header("Animation")]
    public Animator playerAnimator;

    [Header("Combat Settings")]
    public float chopRange = 3f;
    public float damageToZombie = 25f;
    public float damageToTree = 1f;

    [Header("Cooldown")]
    public float swingCooldown = 1.5f;
    private float nextSwingTime = 0f;

    [Header("Damage Timing")]
    public float damageDelay = 0.5f;
    private float swingStartTime = 0f;

    [Header("Detection Settings")]
    public LayerMask targetLayers;
    public bool debugMode = true;

    private float currentAngle = 0f;
    private bool swingingForward = false;
    private bool swingingBack = false;
    private bool hasDealtDamage = false;
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;

        if (targetLayers.value == 0)
        {
            targetLayers = LayerMask.GetMask("Default");
            Debug.LogWarning("WeaponSwing: targetLayers not set! Using Default layer.");
        }

        if (playerAnimator == null)
            playerAnimator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !swingingForward && !swingingBack && Time.time >= nextSwingTime)
        {
            swingingForward = true;
            currentAngle = 0f;
            hasDealtDamage = false;
            nextSwingTime = Time.time + swingCooldown;
            swingStartTime = Time.time;

            Debug.Log("[WeaponSwing] Swing started at: " + swingStartTime + ", damage fires at: " + (swingStartTime + damageDelay));

            if (playerAnimator != null)
                playerAnimator.SetTrigger("MeleeAttack");
        }

        // Swing forward
        if (swingingForward)
        {
            float step = swingSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, step, Space.Self);
            currentAngle += step;

            if (!hasDealtDamage && Time.time >= swingStartTime + damageDelay)
            {
                Debug.Log("[WeaponSwing] Damage delay reached, calling TryChop");
                TryChop();
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
            transform.Rotate(Vector3.up, -step, Space.Self);
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
        Debug.Log("[WeaponSwing] TryChop called. TargetLayers value: " + targetLayers.value);

        Transform camTransform = Camera.main.transform;

        Ray centerRay = new Ray(camTransform.position, camTransform.forward);
        RaycastHit centerHit;

        if (debugMode)
            Debug.DrawRay(centerRay.origin, centerRay.direction * chopRange, Color.red, 1f);

        if (Physics.Raycast(centerRay, out centerHit, chopRange, targetLayers))
        {
            Debug.Log("[WeaponSwing] Raycast hit: " + centerHit.collider.name + " on layer: " + LayerMask.LayerToName(centerHit.collider.gameObject.layer));
            ProcessHit(centerHit);
            return;
        }

        if (Physics.SphereCast(camTransform.position, 0.3f, camTransform.forward, out centerHit, chopRange, targetLayers))
        {
            Debug.Log("[WeaponSwing] SphereCast hit: " + centerHit.collider.name);
            ProcessHit(centerHit);
            return;
        }

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3 direction = camTransform.forward +
                                    camTransform.right * (i * 0.1f) +
                                    camTransform.up * (j * 0.1f);
                Ray coneRay = new Ray(camTransform.position, direction.normalized);
                RaycastHit coneHit;

                if (Physics.Raycast(coneRay, out coneHit, chopRange, targetLayers))
                {
                    Debug.Log("[WeaponSwing] ConeRay hit: " + coneHit.collider.name);
                    ProcessHit(coneHit);
                    return;
                }
            }
        }

        // Nothing hit at all - log what's in front without layer filter to diagnose
        RaycastHit noFilterHit;
        if (Physics.Raycast(centerRay, out noFilterHit, chopRange))
        {
            Debug.LogWarning("[WeaponSwing] MISSED due to layer filter! Object in front: " 
                + noFilterHit.collider.name 
                + " is on layer: " + LayerMask.LayerToName(noFilterHit.collider.gameObject.layer)
                + " but targetLayers only includes: " + targetLayers.value);
        }
        else
        {
            Debug.Log("[WeaponSwing] Swing missed - nothing in front within range " + chopRange);
        }
    }

    void ProcessHit(RaycastHit hit)
    {
        if (hasDealtDamage) return;

        TreeInteractable tree = hit.collider.GetComponentInParent<TreeInteractable>();
        if (tree != null)
        {
            tree.Chop();
            hasDealtDamage = true;
            if (debugMode) Debug.Log("[WeaponSwing] HIT TREE: " + tree.treeName);
            return;
        }

        Zombie1 zombie1 = hit.collider.GetComponent<Zombie1>();
        if (zombie1 != null)
        {
            zombie1.TakeDamage(damageToZombie);
            hasDealtDamage = true;
            if (debugMode) Debug.Log("[WeaponSwing] HIT ZOMBIE (Zombie1)");
            return;
        }

        Zombie zombie = hit.collider.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.TakeDamage((int)damageToZombie);
            hasDealtDamage = true;
            if (debugMode) Debug.Log("[WeaponSwing] HIT ZOMBIE (Zombie)");
            return;
        }

        BossZombie boss = hit.collider.GetComponent<BossZombie>();
        if (boss != null)
        {
            boss.TakeDamage(damageToZombie);
            hasDealtDamage = true;
            if (debugMode) Debug.Log("[WeaponSwing] HIT BOSS ZOMBIE");
            return;
        }

        if (debugMode) Debug.Log("[WeaponSwing] Hit object but no valid script found: " + hit.collider.name);
    }

    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * chopRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Camera.main.transform.position + Camera.main.transform.forward * chopRange, 0.3f);
    }
}