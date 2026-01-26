using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    public float chargeRange = 15f;
    public float groundPoundRange = 8f;
    public float meleeCooldown = 2f;
    public float chargeCooldown = 8f;
    public float groundPoundCooldown = 12f;

    [Header("Charge Attack")]
    public float chargeSpeed = 10f;
    public float chargeDuration = 2f;
    public float chargeWindupTime = 1f;
    public ParticleSystem chargeEffect;

    [Header("Ground Pound")]
    public float groundPoundRadius = 8f;
    public float groundPoundDamage = 40f;
    public float groundPoundKnockback = 10f;
    public GameObject groundPoundEffect;

    [Header("UI & Phase")]
    public Canvas healthBarCanvas;
    public Image healthBarFill;
    public TMP_Text bossNameText;
    public bool battleStarted = false;
    public bool activateOnStart = false;

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;
    public LayerMask playerLayer;
    public AudioSource roarSound;

    private enum State { Idle, Chasing, Attacking, Charging, GroundPound, Dead }
    private State currentState = State.Idle;

    private float lastMeleeTime;
    private float lastChargeTime;
    private float lastGroundPoundTime;
    private bool isCharging = false;
    private bool isEnraged = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Auto-find components
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        
        if (anim == null)
            anim = GetComponent<Animator>();

        // Configure NavMeshAgent
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.angularSpeed = 120;
            agent.acceleration = 8;
            agent.stoppingDistance = 2f;
            agent.autoBraking = true;
        }

        // Setup health bar
        UpdateHealthBar();
        if (bossNameText != null) 
            bossNameText.text = bossName;

        // Detach health bar so it doesn't rotate with boss
        if (healthBarCanvas != null)
            healthBarCanvas.transform.SetParent(null);

        // Start inactive unless testing
        if (!activateOnStart)
        {
            battleStarted = false;
            if (agent != null) agent.isStopped = true;
            if (healthBarCanvas != null) healthBarCanvas.gameObject.SetActive(false);
            this.enabled = false; // Disable script entirely
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

        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(true);

        if (roarSound != null) 
            roarSound.Play();

        Debug.Log("🧟 BOSS FIGHT STARTED!");
    }

    void Update()
    {
        if (!battleStarted || currentState == State.Dead || player == null)
            return;

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

    void LateUpdate()
    {
        // Update health bar position (prevents jitter)
        if (healthBarCanvas != null && currentState != State.Dead && battleStarted)
        {
            healthBarCanvas.transform.position = transform.position + Vector3.up * 4f;
            healthBarCanvas.transform.LookAt(Camera.main.transform);
            healthBarCanvas.transform.Rotate(0, 180, 0);
        }
    }

    void ChasePlayer(float distance)
    {
        if (isCharging || agent == null || !agent.isOnNavMesh)
            return;

        currentState = State.Chasing;
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Update animations based on actual movement
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
        else if (distance > meleeRange && distance <= chargeRange && Time.time >= lastChargeTime + chargeCooldown)
        {
            StartCharge();
        }
        else if (distance <= meleeRange && Time.time >= lastMeleeTime + meleeCooldown)
        {
            StartMelee();
        }
    }

    void StartMelee()
    {
        currentState = State.Attacking;
        agent.isStopped = true;

        // Face player
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
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 1.5f, 1.5f, playerLayer);
        
        foreach (Collider hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(attackDamage);
                Debug.Log($"Boss melee: {attackDamage} damage");

                if (CameraShake.Instance != null)
                    CameraShake.Instance.Shake(0.3f, 0.2f);
            }
        }
    }

    void StartCharge()
    {
        currentState = State.Charging;
        agent.isStopped = true;

        if (roarSound != null) roarSound.Play();
        if (anim != null) anim.SetTrigger("ChargeWindup");

        Invoke(nameof(ExecuteCharge), chargeWindupTime);
        lastChargeTime = Time.time;
    }

    void ExecuteCharge()
    {
        isCharging = true;
        
        if (anim != null) anim.SetBool("Charging", true);
        if (chargeEffect != null) chargeEffect.Play();

        Vector3 chargeDir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(chargeDir);

        // CRITICAL FIX: Disable agent during manual movement
        if (agent != null) agent.enabled = false;

        StartCoroutine(ChargeMovement(chargeDir));
    }

    IEnumerator ChargeMovement(Vector3 direction)
    {
        float elapsed = 0f;

        while (elapsed < chargeDuration)
        {
            // Manual movement
            transform.position += direction * chargeSpeed * Time.deltaTime;

            // Check collision with player
            Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f, playerLayer);
            if (hits.Length > 0)
            {
                PlayerHealth ph = hits[0].GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(attackDamage * 1.5f);
                    Debug.Log($"Boss charge: {attackDamage * 1.5f} damage");

                    if (CameraShake.Instance != null)
                        CameraShake.Instance.Shake(0.5f, 0.3f);
                }
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // End charge
        isCharging = false;
        if (anim != null) anim.SetBool("Charging", false);
        if (chargeEffect != null) chargeEffect.Stop();

        // CRITICAL FIX: Re-enable agent
        if (agent != null) agent.enabled = true;

        yield return new WaitForSeconds(0.5f);

        currentState = State.Chasing;
    }

    void StartGroundPound()
    {
        currentState = State.GroundPound;
        agent.isStopped = true;

        if (anim != null) anim.SetTrigger("GroundPound");

        Invoke(nameof(ExecuteGroundPound), 1f);
        lastGroundPoundTime = Time.time;
    }

    public void ExecuteGroundPound()
    {
        if (groundPoundEffect != null)
            Instantiate(groundPoundEffect, transform.position, Quaternion.identity);

        // Fixed: Only hit player layer
        Collider[] hits = Physics.OverlapSphere(transform.position, groundPoundRadius, playerLayer);
        
        foreach (Collider hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(groundPoundDamage);
                
                // Knockback
                Vector3 knockDir = (hit.transform.position - transform.position).normalized;
                CharacterController cc = hit.GetComponent<CharacterController>();
                if (cc != null)
                {
                    StartCoroutine(ApplyKnockback(cc, knockDir));
                }

                Debug.Log($"Ground pound: {groundPoundDamage} damage");
            }
        }

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.8f, 0.5f);

        Invoke(nameof(ResetAttack), 2f);
    }

    IEnumerator ApplyKnockback(CharacterController cc, Vector3 direction)
    {
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            cc.Move(direction * groundPoundKnockback * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
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

        Debug.Log($"💥 Boss took {damage} damage! HP: {currentHealth}/{maxHealth}");

        UpdateHealthBar();

        // Enrage at 50%
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
        Debug.Log("😡 BOSS ENRAGED!");

        Renderer r = GetComponentInChildren<Renderer>();
        if (r != null) r.material.color = Color.red;

        moveSpeed *= 1.5f;
        if (agent != null) agent.speed = moveSpeed;

        lastChargeTime = 0;
        lastGroundPoundTime = 0;

        if (roarSound != null) roarSound.Play();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;

            if (!isEnraged)
            {
                if (currentHealth > maxHealth * 0.6f)
                    healthBarFill.color = Color.green;
                else if (currentHealth > maxHealth * 0.3f)
                    healthBarFill.color = Color.yellow;
                else
                    healthBarFill.color = Color.red;
            }
            else
            {
                healthBarFill.color = Color.red;
            }
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

        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log("💀 BOSS DEFEATED!");

        // Stop zombie spawning
        ZombieSpawner spawner = FindObjectOfType<ZombieSpawner>();
        if (spawner != null) spawner.StopSpawning();

        Destroy(gameObject, 5f);
        this.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargeRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, groundPoundRadius);
    }
}