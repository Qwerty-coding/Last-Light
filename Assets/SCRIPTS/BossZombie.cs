using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BossZombie : MonoBehaviour
{
    [Header("Boss Stats")]
    public string bossName = "Subject Alpha";
    public float maxHealth = 500f;
    private float currentHealth;
    public float moveSpeed = 3f;
    public float attackDamage = 25f;

    [Header("Attack Settings")]
    public float meleeRange = 3f;
    public float seismicRoarRange = 12f;
    public float groundPoundRange = 8f;
    
    public float meleeCooldown = 2f;
    public float seismicRoarCooldown = 8f;
    public float groundPoundCooldown = 12f;

    [Header("Visual Indicators (NEW)")]
    public GameObject warningCirclePrefab; // Drag a Red Circle Prefab here!

    [Header("Seismic Roar Attack")]
    public float roarDamage = 35f;
    public float roarWindupTime = 1.2f;
    public ParticleSystem roarEffect;

    [Header("Ground Pound")]
    public float groundPoundRadius = 8f;
    public float groundPoundDamage = 40f;
    public float groundPoundKnockback = 10f;
    public GameObject groundPoundEffect;

    [Header("UI - Screen Space")]
    public GameObject bossHealthUI;           
    public Image healthBarFill;               
    public TMP_Text bossNameText;               
    public TMP_Text healthText;                   
    public bool activateOnStart = false;

    [Header("Victory Settings")]
    public VictoryManager victoryManager;
    public float delayBeforeVictory = 4f;

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;
    public LayerMask playerLayer;
    public AudioSource roarSound;

    public bool battleStarted = false;

    private enum State { Idle, Chasing, Melee, SeismicRoar, GroundPound, Dead }
    private State currentState = State.Idle;

    private float lastMeleeTime;
    private float lastRoarTime;
    private float lastGroundPoundTime;
    
    private bool isEnraged = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        
        if (anim == null)
            anim = GetComponent<Animator>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.angularSpeed = 120;
            agent.acceleration = 8;
            agent.stoppingDistance = 2f;
            agent.autoBraking = true;
        }

        if (bossNameText != null) 
            bossNameText.text = bossName;
        
        UpdateHealthBar();

        if (bossHealthUI != null)
            bossHealthUI.SetActive(false);

        if (!activateOnStart)
        {
            battleStarted = false;
            if (agent != null) agent.isStopped = true;
            this.enabled = false;
        }
        else
        {
            StartBossFight();
        }
    }

    public void StartBossFight()
    {
        if (battleStarted) return;

        battleStarted = true;
        this.enabled = true;

        if (agent != null) 
            agent.isStopped = false;

        if (bossHealthUI != null)
            bossHealthUI.SetActive(true);

        if (roarSound != null) 
            roarSound.Play();

        Debug.Log("🧟 BOSS FIGHT STARTED!");
    }

    void Update()
    {
        if (!battleStarted || currentState == State.Dead || player == null)
            return;

            if (!isEnraged) agent.speed = moveSpeed;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
            case State.Chasing:
                ChasePlayer(distToPlayer);
                DecideAttack(distToPlayer);
                break;
        }
    }

    void ChasePlayer(float distance)
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        currentState = State.Chasing;
        agent.isStopped = false;
        agent.SetDestination(player.position);

        float velocity = agent.velocity.magnitude;
        
        if (velocity > 0.1f)
        {
            if (anim != null)
            {
                anim.SetBool("Walking", velocity < 2f);
                anim.SetBool("Running", velocity >= 2f);
                anim.SetBool("Attacking", false);
            }
        }
        else
        {
            if (anim != null)
            {
                anim.SetBool("Walking", false);
                anim.SetBool("Running", false);
            }
        }
    }

    void DecideAttack(float distance)
    {
        if (distance <= groundPoundRange && Time.time >= lastGroundPoundTime + groundPoundCooldown)
        {
            StartGroundPound();
        }
        else if (distance > meleeRange && distance <= seismicRoarRange && Time.time >= lastRoarTime + seismicRoarCooldown)
        {
            StartSeismicRoar();
        }
        else if (distance <= meleeRange && Time.time >= lastMeleeTime + meleeCooldown)
        {
            StartMelee();
        }
    }

    // ==================== MELEE ATTACK ====================
    void StartMelee()
    {
        currentState = State.Melee;
        agent.isStopped = true;

        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

        if (anim != null)
        {
            anim.SetBool("Attacking", true);
            anim.SetBool("Running", false);
            anim.SetBool("Walking", false);
        }

        Invoke(nameof(DealMeleeDamage), 0.6f);
        Invoke(nameof(ResetAttack), 1.5f);

        lastMeleeTime = Time.time;
    }

    public void DealMeleeDamage()
    {
        // FIX: Damage is now UNCOMMENTED and working
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 1.5f, 1.5f, playerLayer);
        
        foreach (Collider hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null) 
            {
                ph.TakeDamage(attackDamage);
                Debug.Log("💥 Boss Melee hit player!");
            }
        }
    }

    // ==================== SEISMIC ROAR ====================
    void StartSeismicRoar()
    {
        currentState = State.SeismicRoar;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

        if (anim != null)
        {
            anim.SetTrigger("Roar");
            anim.SetBool("Walking", false);
            anim.SetBool("Running", false);
        }

        if (roarSound != null)
            roarSound.Play();

        // NEW: Show Red Warning Circle
        ShowAttackIndicator(seismicRoarRange, roarWindupTime);

        Invoke(nameof(ExecuteSeismicRoar), roarWindupTime);

        lastRoarTime = Time.time;
    }

    void ExecuteSeismicRoar()
    {
        if (roarEffect != null)
            roarEffect.Play();

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // FIX: Damage Logic Restored
        if (distToPlayer <= seismicRoarRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(roarDamage);
                Debug.Log($"🔊 Seismic Roar HIT for {roarDamage} damage!");
                
                // Optional: Shake camera
                if (CameraShake.Instance != null) 
                    CameraShake.Instance.Shake(0.5f, 0.5f);
            }
        }

        Invoke(nameof(ResetAttack), 1f);
    }

    // ==================== GROUND POUND ====================
    void StartGroundPound()
    {
        currentState = State.GroundPound;
        agent.isStopped = true;

        if (anim != null) 
            anim.SetTrigger("GroundPound");

        // NEW: Show Red Warning Circle (Smaller, but faster)
        ShowAttackIndicator(groundPoundRadius, 1.0f);

        Invoke(nameof(ExecuteGroundPound), 1.0f);
        lastGroundPoundTime = Time.time;
    }

    public void ExecuteGroundPound()
    {
        if (groundPoundEffect != null)
            Instantiate(groundPoundEffect, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, groundPoundRadius, playerLayer);
        
        // FIX: Damage Logic Restored
        foreach (Collider hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(groundPoundDamage);
                Debug.Log($"💥 Ground Pound HIT for {groundPoundDamage} damage!");

                // Knockback
                CharacterController cc = hit.GetComponent<CharacterController>();
                if (cc != null)
                {
                    Vector3 knockDir = (hit.transform.position - transform.position).normalized;
                    StartCoroutine(ApplyKnockback(cc, knockDir));
                }
            }
        }
        
        if (CameraShake.Instance != null) 
            CameraShake.Instance.Shake(0.8f, 1.0f); // Stronger shake for pound

        Invoke(nameof(ResetAttack), 2f);
    }

    IEnumerator ApplyKnockback(CharacterController cc, Vector3 direction)
    {
        float elapsed = 0f;
        while (elapsed < 0.2f)
        {
            cc.Move(direction * groundPoundKnockback * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // ==================== VISUAL INDICATOR SYSTEM (NEW) ====================
    void ShowAttackIndicator(float radius, float duration)
    {
        if (warningCirclePrefab == null) return;

        // Spawn the circle slightly above ground to avoid Z-fighting
        Vector3 spawnPos = transform.position + Vector3.up * 0.1f;
        GameObject indicator = Instantiate(warningCirclePrefab, spawnPos, Quaternion.identity);
        
        // Scale it to match the radius (Diameter = Radius * 2)
        // Note: Assuming the prefab is a 1x1 unit circle/quad
        indicator.transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
        
        // Make it lie flat on the ground (Rotate 90 on X)
        indicator.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Destroy it after the windup time
        Destroy(indicator, duration);
    }

    void ResetAttack()
    {
        if (anim != null) anim.SetBool("Attacking", false);
        currentState = State.Chasing;
    }

    public void TakeDamage(float damage)
    {
        if (currentState == State.Dead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (!isEnraged && currentHealth <= maxHealth * 0.5f)
        {
            EnterEnragedMode();
        }

        if (currentHealth > 0)
        {
            if (anim != null) anim.SetTrigger("Hit");
        }
        else
        {
            Die();
        }
    }

    void EnterEnragedMode()
    {
        isEnraged = true;
        Renderer r = GetComponentInChildren<Renderer>();
        if (r != null) r.material.color = Color.red;

        moveSpeed *= 1.5f;
        if (agent != null) agent.speed = moveSpeed;

        if (roarSound != null) roarSound.Play();
        if (bossNameText != null) bossNameText.text = bossName + " - ENRAGED";
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
            
            if (!isEnraged)
            {
                if (currentHealth > maxHealth * 0.6f) healthBarFill.color = Color.green;
                else if (currentHealth > maxHealth * 0.3f) healthBarFill.color = Color.yellow;
                else healthBarFill.color = Color.red;
            }
            else
            {
                healthBarFill.color = Color.red;
            }
        }

        if (healthText != null)
        {
            healthText.text = $"{(int)currentHealth} / {(int)maxHealth}";
        }
    }

    void Die()
    {
        currentState = State.Dead;
        battleStarted = false;

        CancelInvoke();
        StopAllCoroutines();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (anim != null)
        {
            anim.SetBool("Died", true);
            anim.SetBool("Walking", false);
            anim.SetBool("Running", false);
            anim.SetBool("Attacking", false);
        }

        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(false);
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log("💀 BOSS DEFEATED!");

        if (victoryManager != null)
        {
            Invoke(nameof(TriggerWinGame), delayBeforeVictory);
        }

        Destroy(gameObject, 10f);
    }

    void TriggerWinGame()
    {
        if (victoryManager != null)
            victoryManager.ShowVictory();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, seismicRoarRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, groundPoundRadius);
    }
}