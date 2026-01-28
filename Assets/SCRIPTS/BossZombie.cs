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

    [Header("Seismic Roar Attack")]
    public float roarDamage = 35f;
    public float roarWindupTime = 1.2f;
    public ParticleSystem roarEffect;

    [Header("Ground Pound")]
    public float groundPoundRadius = 8f;
    public float groundPoundDamage = 40f;
    public float groundPoundKnockback = 10f;
    public GameObject groundPoundEffect;

    [Header("UI - Screen Space (NEW)")]
    public GameObject bossHealthUI;           // The entire UI panel
    public Image healthBarFill;               // The fill image
    public TMP_Text bossNameText;                 // Boss name text
    public TMP_Text healthText;                   // Optional: "500/500" text
    public bool activateOnStart = false;

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

        // Setup UI
        if (bossNameText != null) 
            bossNameText.text = bossName;
        
        UpdateHealthBar();

        // Hide UI at start
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

        BossUIAnimator uiAnim = bossHealthUI.GetComponent<BossUIAnimator>();
    if (uiAnim != null)
        uiAnim.SlideIn();

        if (agent != null) 
            agent.isStopped = false;

        // Show UI at top of screen
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

    // REMOVED: LateUpdate (no longer needed - UI doesn't follow boss)

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
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 1.5f, 1.5f, playerLayer);
        
        foreach (Collider hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(attackDamage);
                Debug.Log($"💥 Boss melee: {attackDamage} damage");
            }
        }
    }

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

        Debug.Log("📢 BOSS ROARING! Run away!");

        Invoke(nameof(ExecuteSeismicRoar), roarWindupTime);

        lastRoarTime = Time.time;
    }

    void ExecuteSeismicRoar()
    {
        if (roarEffect != null)
            roarEffect.Play();

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= seismicRoarRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(roarDamage);
                Debug.Log($"🔊 Seismic Roar hit for {roarDamage} damage!");
            }
        }
        else
        {
            Debug.Log("Player escaped the roar!");
        }

        Invoke(nameof(ResetAttack), 1f);
    }

    void StartGroundPound()
    {
        currentState = State.GroundPound;
        agent.isStopped = true;

        if (anim != null) 
            anim.SetTrigger("GroundPound");

        Invoke(nameof(ExecuteGroundPound), 1f);
        lastGroundPoundTime = Time.time;
    }

    public void ExecuteGroundPound()
    {
        if (groundPoundEffect != null)
            Instantiate(groundPoundEffect, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, groundPoundRadius, playerLayer);
        
        foreach (Collider hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(groundPoundDamage);
                
                Vector3 knockDir = (hit.transform.position - transform.position).normalized;
                CharacterController cc = hit.GetComponent<CharacterController>();
                if (cc != null)
                {
                    StartCoroutine(ApplyKnockback(cc, knockDir));
                }

                Debug.Log($"💥 Ground pound: {groundPoundDamage} damage");
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

        lastMeleeTime = 0;
        lastRoarTime = 0;
        lastGroundPoundTime = 0;

        if (roarSound != null) roarSound.Play();
        
        // Optional: Change boss name text
        if (bossNameText != null)
            bossNameText.text = bossName + " - ENRAGED";
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;

            // Color changes based on health
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

        // Update health text (optional)
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

        // Hide UI when boss dies
        if (bossHealthUI != null)
        {
            StartCoroutine(FadeOutUI());
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log("💀 BOSS DEFEATED!");

        ZombieSpawner spawner = FindObjectOfType<ZombieSpawner>();
        if (spawner != null) spawner.StopSpawning();

        Destroy(gameObject, 5f);
        this.enabled = false;
    }

    // Optional: Smooth fade out when boss dies
    IEnumerator FadeOutUI()
    {
        CanvasGroup cg = bossHealthUI.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = bossHealthUI.AddComponent<CanvasGroup>();

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - (elapsed / duration);
            yield return null;
        }

        bossHealthUI.SetActive(false);
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