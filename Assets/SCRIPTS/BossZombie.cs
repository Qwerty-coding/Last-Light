using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class BossZombie : MonoBehaviour
{
    [Header("Boss Stats")]
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

    private float lastMeleeTime;
    private float lastChargeTime;
    private float lastGroundPoundTime;

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
    public GameObject healthBarUI;
    public Image healthBarFill;
    public bool battleStarted = false;
    private bool isEnraged = false;

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;
    public LayerMask playerLayer; // Make sure this is set to "Player" in Inspector!
    public AudioSource roarSound;

    private Camera mainCam;

    private enum BossState { Idle, Chasing, Melee, Charge, GroundPound, Dead }
    private BossState currentState = BossState.Idle;
    private bool canAct = true;

    void Start()
    {
        currentHealth = maxHealth;

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (!player)
        {
            Debug.LogError("BossZombie: Player not found! Tag your player as 'Player'.");
            enabled = false;
            return;
        }

        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponent<Animator>();

        agent.speed = moveSpeed;
        mainCam = Camera.main;

        if (healthBarUI) healthBarUI.SetActive(false);
    }

    void LateUpdate()
    {
        // Smoothly rotate UI to face camera
        if (!healthBarUI || !battleStarted || currentState == BossState.Dead) return;

        healthBarUI.transform.position = transform.position + Vector3.up * 3.5f;
        healthBarUI.transform.LookAt(mainCam.transform);
        healthBarUI.transform.Rotate(0, 180, 0);
    }

    void Update()
    {
        if (!battleStarted || currentState == BossState.Dead || !canAct || !player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= groundPoundRange && Time.time >= lastGroundPoundTime + groundPoundCooldown)
            StartGroundPound();
        else if (distance > meleeRange && distance <= chargeRange && Time.time >= lastChargeTime + chargeCooldown)
            StartCharge();
        else if (distance <= meleeRange && Time.time >= lastMeleeTime + meleeCooldown)
            StartMelee();
        else
            Chase();
    }

    // -------- ACTIVATION --------
    public void StartBossFight()
    {
        battleStarted = true;
        currentState = BossState.Chasing;

        if (healthBarUI) healthBarUI.SetActive(true);
        if (roarSound) roarSound.Play();
        anim.SetTrigger("Roar");
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
        agent.isStopped = true;

        anim.SetBool("Attacking", true);
        lastMeleeTime = Time.time;

        // SAFETY FALLBACK: Even if you forget Animation Events, this deals damage at 0.6s
        Invoke(nameof(DealMeleeDamage), 0.6f); 
        
        Invoke(nameof(ResetState), 1.5f);
    }

    public void DealMeleeDamage()
    {
        if (currentState != BossState.Melee) return;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1.5f;

        // Uses playerLayer to only hit the player
        if (Physics.Raycast(origin, transform.forward, out hit, meleeRange, playerLayer))
            hit.collider.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
    }

    // -------- CHARGE --------
    void StartCharge()
    {
        CancelInvoke();
        currentState = BossState.Charge;
        canAct = false;
        agent.isStopped = true;

        anim.SetTrigger("ChargeWindup");
        lastChargeTime = Time.time;

        Invoke(nameof(ExecuteCharge), chargeWindupTime);
    }

    void ExecuteCharge()
    {
        agent.enabled = false; // Critical: Disable NavMeshAgent so we can move manually

        Vector3 dir = (player.position - transform.position).normalized;
        StartCoroutine(ChargeRoutine(dir));
    }

    IEnumerator ChargeRoutine(Vector3 direction)
    {
        float t = 0f;
        if (chargeEffect) chargeEffect.Play();

        // LayerMask to hit Walls/Ground but IGNORE Player
        // This prevents the boss from stopping when he sees the player
        int wallMask = ~playerLayer; 

        while (t < chargeDuration)
        {
            // Stop if we hit a wall (but not the player)
            if (Physics.Raycast(transform.position + Vector3.up, direction, 1.5f, wallMask))
                break;

            transform.position += direction * chargeSpeed * Time.deltaTime;

            // Check if we hit the player to deal damage
            Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f, playerLayer);
            if (hits.Length > 0)
            {
                hits[0].GetComponent<PlayerHealth>()?.TakeDamage(attackDamage * 1.5f);
                break; // Stop charge on impact
            }

            t += Time.deltaTime;
            yield return null;
        }

        // Snap agent back to navmesh to prevent errors
        agent.enabled = true; 
        NavMeshHit navHit;
        if(NavMesh.SamplePosition(transform.position, out navHit, 2.0f, NavMesh.AllAreas))
        {
             agent.Warp(navHit.position);
        }
        
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

        Invoke(nameof(ExecuteGroundPound), 1f);
    }

    void ExecuteGroundPound()
    {
        if (groundPoundEffect)
            Instantiate(groundPoundEffect, transform.position, Quaternion.identity);

        // Hits only objects on 'Player' layer
        Collider[] hits = Physics.OverlapSphere(transform.position, groundPoundRadius, playerLayer);
        foreach (Collider hit in hits)
        {
            hit.GetComponent<PlayerHealth>()?.TakeDamage(groundPoundDamage);

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(dir * groundPoundKnockback, ForceMode.Impulse);
            }
        }

        Invoke(nameof(ResetState), 2f);
    }

    // -------- DAMAGE --------
    public void TakeDamage(float amount)
    {
        if (currentState == BossState.Dead) return;

        currentHealth -= amount;
        if (healthBarFill)
            healthBarFill.fillAmount = currentHealth / maxHealth;

        // ENRAGE PHASE
        if (!isEnraged && currentHealth <= maxHealth * 0.5f)
        {
            isEnraged = true;

            // Make him Red
            Renderer r = GetComponentInChildren<Renderer>();
            if(r != null) {
                r.material.color = Color.red; 
            }

            moveSpeed *= 1.5f;
            agent.speed = moveSpeed;
        }

        if (currentHealth <= 0)
            Die();
        else
            anim.SetTrigger("Hit");
    }

    void Die()
    {
        CancelInvoke();
        StopAllCoroutines(); // Stop any charging immediately
        currentState = BossState.Dead;
        agent.isStopped = true;

        anim.SetTrigger("Died");
        if (healthBarUI) healthBarUI.SetActive(false);

        Destroy(gameObject, 5f);
    }

    // -------- RESET --------
    void ResetState()
    {
        CancelInvoke();
        currentState = BossState.Chasing;
        canAct = true;
        anim.SetBool("Attacking", false);
    }
}