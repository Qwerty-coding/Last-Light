using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class BossZombie : MonoBehaviour
{
    [Header("Boss Stats")]
    public string bossName = "Subject Alpha";
    public float maxHealth = 500f;
    private float currentHealth;
    public float moveSpeed = 3.5f;
    public float attackDamage = 25f;

    [Header("Attack Settings")]
    public float meleeRange = 3f;
    public float chargeRange = 15f;
    public float groundPoundRange = 8f;
    
    // Cooldowns
    public float meleeCooldown = 2f;
    public float chargeCooldown = 10f;
    public float groundPoundCooldown = 15f;

    private float lastMeleeTime;
    private float lastChargeTime;
    private float lastGroundPoundTime;

    [Header("Charge Attack")]
    public float chargeSpeed = 12f; // Increased for better feel
    public float chargeDuration = 1.5f;
    public float chargeWindupTime = 1.5f;
    public ParticleSystem chargeEffect;

    [Header("Ground Pound")]
    public float groundPoundRadius = 8f;
    public float groundPoundDamage = 40f;
    public float groundPoundKnockback = 10f;
    public GameObject groundPoundEffect;

    [Header("UI & Phase")]
    public Canvas healthBarCanvas; // Drag 'HealthBarCanvas' here
    public Image healthBarFill;    // Drag 'HealthFill' here
    public Text bossNameText;      // (Optional) Drag 'BossName' text here
    public bool battleStarted = false;
    public bool activateOnStart = false; // Check this for testing without teleporter

    private bool isEnraged = false;

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;
    public LayerMask playerLayer; 
    public AudioSource roarSound;
    private Renderer bossRenderer;

    private enum BossState { Idle, Chasing, Melee, Charge, GroundPound, Dead }
    private BossState currentState = BossState.Idle;
    private bool canAct = true;

    void Start()
    {
        // 1. Initialization
        currentHealth = maxHealth;
        bossRenderer = GetComponentInChildren<Renderer>();

        // Auto-find references if missing
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponent<Animator>();

        if (!player)
        {
            Debug.LogError("CRITICAL: Player not found! Tag your player as 'Player'.");
            return;
        }

        agent.speed = moveSpeed;

        // 2. Health Bar Setup (Fix 3 & 7)
        if (healthBarCanvas != null)
        {
            // Detach from boss so it doesn't rotate with him
            healthBarCanvas.transform.SetParent(null); 
            healthBarCanvas.gameObject.SetActive(false); // Hide until battle starts
            
            if (bossNameText) bossNameText.text = bossName;
            UpdateHealthBar();
        }

        // 3. Activation Logic
        if (activateOnStart)
        {
            StartBossFight();
        }
        else
        {
            // Dormant state
            agent.isStopped = true; 
            canAct = false;
        }
    }

    // Fix 3: Keep UI following boss even though it's detached
    void LateUpdate()
    {
        if (healthBarCanvas != null && currentHealth > 0)
        {
            // Position above boss head
            healthBarCanvas.transform.position = transform.position + Vector3.up * 3.5f; 
            // Face camera
            healthBarCanvas.transform.LookAt(Camera.main.transform);
            healthBarCanvas.transform.Rotate(0, 180, 0);
        }
    }

    void Update()
    {
        if (!battleStarted || currentState == BossState.Dead || !canAct || !player) return;

        // Fix 4: Anti-Float Logic (Ensure he sticks to NavMesh)
        if (agent.enabled && agent.isOnNavMesh)
        {
            float yDiff = Mathf.Abs(transform.position.y - agent.nextPosition.y);
            if(yDiff > 1f) agent.Warp(transform.position); // Snap if desynced
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // State Machine Logic
        if (distance <= groundPoundRange && Time.time >= lastGroundPoundTime + groundPoundCooldown)
        {
            StartGroundPound();
        }
        else if (distance > meleeRange && distance <= chargeRange && Time.time >= lastChargeTime + chargeCooldown)
        {
            StartCharge();
        }
        else if (distance <= meleeRange && Time.time >= lastMeleeTime + meleeCooldown)
        {
            StartMelee();
        }
        else
        {
            Chase();
        }
    }

    // -------- ACTIVATION --------
    public void StartBossFight()
    {
        if (battleStarted) return;

        battleStarted = true;
        canAct = true;
        currentState = BossState.Chasing;
        agent.isStopped = false;

        if (healthBarCanvas) healthBarCanvas.gameObject.SetActive(true);
        if (roarSound) roarSound.Play();
        
        anim.SetTrigger("Roar");
        Debug.Log("🧟 BOSS AWAKENED!");
    }

    // -------- MOVEMENT --------
    void Chase()
    {
        currentState = BossState.Chasing;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        
        anim.SetBool("Running", true);
        anim.SetBool("Attacking", false);
    }

    // -------- MELEE --------
    void StartMelee()
    {
        CancelInvoke();
        currentState = BossState.Melee;
        canAct = false;
        agent.isStopped = true; // Stop moving to punch

        anim.SetBool("Running", false);
        anim.SetBool("Attacking", true);
        lastMeleeTime = Time.time;

        // Safety: If Animation Event fails, deal damage anyway after delay
        Invoke(nameof(DealMeleeDamage), 0.5f); 
        Invoke(nameof(ResetState), 1.2f);
    }

    // Called by Animation Event OR Invoke
    public void DealMeleeDamage()
    {
        if (currentState != BossState.Melee) return;

        // Simple distance check for hit
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= meleeRange + 1f) // +1 buffer
        {
            // You can replace this with your PlayerHealth script logic
             Debug.Log("Boss Hit Player!");
             player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    // -------- CHARGE (Fix 5) --------

    // -------- CHARGE ATTACK (Fixed Sequence) --------
    void StartCharge()
    {
        CancelInvoke();
        currentState = BossState.Charge;
        canAct = false;
        
        // 1. HARD STOP (Fixes the Sliding!)
        agent.isStopped = true;
        agent.velocity = Vector3.zero; 
        agent.ResetPath();

        // 2. Play Roar (Windup)
        anim.SetTrigger("ChargeWindup");
        lastChargeTime = Time.time;

        // 3. Wait for Roar to finish, then Charge
        Invoke(nameof(ExecuteCharge), chargeWindupTime);
    }

    void ExecuteCharge()
    {
        // 4. Start the Sprint
        agent.enabled = false; 
        Vector3 dir = (player.position - transform.position).normalized;
        StartCoroutine(ChargeMovement(dir));
    }

    IEnumerator ChargeMovement(Vector3 direction)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (direction * chargeSpeed * chargeDuration);
        
        // Keep him strictly on the floor (Fixes Flying)
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 5f, NavMesh.AllAreas))
            targetPos = hit.position;

        if (chargeEffect) chargeEffect.Play();

        // --- THE SPRINT PHASE ---
        // Trigger the Sprint Animation (Make sure you have a Bool or Trigger for this!)
        // Or relying on the transition from Roar -> Sprint via Exit Time
        
        while (elapsed < chargeDuration)
        {
            float t = elapsed / chargeDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            // Hit Check
            Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f, playerLayer);
            if (hits.Length > 0)
            {
                Debug.Log("💥 TACKLE HIT!");
                hits[0].GetComponent<PlayerHealth>()?.TakeDamage(attackDamage * 1.5f);
                break; // Stop immediately on impact
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (chargeEffect) chargeEffect.Stop();

        // --- THE STUMBLE PHASE (New!) ---
        // 1. Snap to NavMesh
        agent.enabled = true;
        if (NavMesh.SamplePosition(transform.position, out hit, 3f, NavMesh.AllAreas))
            agent.Warp(hit.position);
            
        // 2. Play Stumble Animation
        anim.SetTrigger("Stumble"); // <--- MAKE SURE YOU ADD THIS PARAMETER!
        
        // 3. Wait for Stumble to finish (e.g., 1.5 seconds)
        yield return new WaitForSeconds(1.5f);

        // 4. Resume Chase
        ResetState();
    }
    // -------- GROUND POUND --------
    void StartGroundPound()
    {
        CancelInvoke();
        currentState = BossState.GroundPound;
        canAct = false;
        agent.isStopped = true;

        anim.SetTrigger("GroundPound");
        lastGroundPoundTime = Time.time;

        Invoke(nameof(ExecuteGroundPound), 1.0f); // Adjust based on anim length
    }

    void ExecuteGroundPound()
    {
        if (groundPoundEffect) Instantiate(groundPoundEffect, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, groundPoundRadius, playerLayer);
        foreach (var hit in hits)
        {
            hit.GetComponent<PlayerHealth>()?.TakeDamage(groundPoundDamage);
            // Add knockback logic here if needed
            Debug.Log("Ground Pound Hit!");
        }

        Invoke(nameof(ResetState), 2.0f);
    }

    // -------- HEALTH & DEATH --------
    public void TakeDamage(float amount)
    {
        if (currentState == BossState.Dead) return;

        currentHealth -= amount;
        UpdateHealthBar();

        if (currentHealth <= 0) Die();
        else anim.SetTrigger("Hit");
    }

    void UpdateHealthBar()
    {
        if (healthBarFill) healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    void Die()
    {
        currentState = BossState.Dead;
        agent.isStopped = true;
        canAct = false;
        anim.SetTrigger("Died");
        
        if (healthBarCanvas) healthBarCanvas.gameObject.SetActive(false);
        Destroy(gameObject, 5f);
    }

    void ResetState()
    {
        if (currentState == BossState.Dead) return;
        currentState = BossState.Chasing;
        canAct = true;
        agent.isStopped = false;
        anim.SetBool("Attacking", false);
       
        agent.SetDestination(player.position);
    }
}