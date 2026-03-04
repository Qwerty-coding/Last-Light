using UnityEngine;

public class WeaponSwing : MonoBehaviour
{
    [Header("Swing Animation")]
    public float swingAngle = 50f;
    public float swingSpeed = 20f;

    [Header("Animation")]
    public Animator playerAnimator;  // Drag your character's Animator here in Inspector

    [Header("Combat Settings")]
    public float chopRange = 3f;
    public float damageToZombie = 25f;
    public float damageToTree = 1f;

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

        // Auto-find animator on parent if not assigned in Inspector
        if (playerAnimator == null)
            playerAnimator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !swingingForward && !swingingBack)
        {
            swingingForward = true;
            currentAngle = 0f;
            hasDealtDamage = false;

            // "MeleeAttack" trigger -> AxeSwing state
            if (playerAnimator != null)
                playerAnimator.SetTrigger("MeleeAttack");
        }

        // Swing forward
        if (swingingForward)
        {
            float step = swingSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, step, Space.Self);
            currentAngle += step;

            if (currentAngle >= swingAngle * 0.3f && currentAngle < swingAngle * 0.7f)
            {
                if (!hasDealtDamage)
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
        Transform camTransform = Camera.main.transform;

        Ray centerRay = new Ray(camTransform.position, camTransform.forward);
        RaycastHit centerHit;

        if (debugMode)
            Debug.DrawRay(centerRay.origin, centerRay.direction * chopRange, Color.red, 0.5f);

        if (Physics.Raycast(centerRay, out centerHit, chopRange, targetLayers))
        { ProcessHit(centerHit); return; }

        if (Physics.SphereCast(camTransform.position, 0.3f, camTransform.forward, out centerHit, chopRange, targetLayers))
        { ProcessHit(centerHit); return; }

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
                    Debug.DrawRay(coneRay.origin, coneRay.direction * chopRange, Color.yellow, 0.2f);

                if (Physics.Raycast(coneRay, out coneHit, chopRange, targetLayers))
                { ProcessHit(coneHit); return; }
            }
        }

        if (debugMode) Debug.Log("Swing missed - no valid target in range");
    }

    void ProcessHit(RaycastHit hit)
    {
        if (hasDealtDamage) return;

        TreeInteractable tree = hit.collider.GetComponentInParent<TreeInteractable>();
        if (tree != null)
        {
            tree.Chop();
            hasDealtDamage = true;
            if (debugMode) Debug.Log("HIT TREE: " + tree.treeName);
            return;
        }

        Zombie1 zombie1 = hit.collider.GetComponent<Zombie1>();
        if (zombie1 != null)
        {
            zombie1.TakeDamage(damageToZombie);
            hasDealtDamage = true;
            if (debugMode) Debug.Log("HIT ZOMBIE (Zombie1)");
            return;
        }

        Zombie zombie = hit.collider.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.TakeDamage((int)damageToZombie);
            hasDealtDamage = true;
            if (debugMode) Debug.Log("HIT ZOMBIE (Zombie)");
            return;
        }

        BossZombie boss = hit.collider.GetComponent<BossZombie>();
        if (boss != null)
        {
            boss.TakeDamage(damageToZombie);
            hasDealtDamage = true;
            if (debugMode) Debug.Log("HIT BOSS ZOMBIE");
            return;
        }

        if (debugMode) Debug.Log("Hit object with no valid script: " + hit.collider.name);
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