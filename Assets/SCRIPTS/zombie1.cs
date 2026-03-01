using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class Zombie1 : MonoBehaviour
{
    [Header("Zombie Health and Damage")]
    public float giveDamage = 10f;
    public float health = 100f;

    [Header("Zombie Components")]
    public NavMeshAgent zombieAgent;
    public Transform playerTransform;
    public Camera AttackingRaycastArea;
    public LayerMask PlayerLayer;
    public Animator anim;

    [Header("Zombie Guarding")]
    public GameObject[] walkPoints;
    int currentZombiePosition = 0;
    public float walkSpeed = 1.5f;
    float walkingpointRadius = 2;

    [Header("Zombie Attacking")]
    public float timeBetweenAttacks = 1.5f;
    bool alreadyAttacked;

    [Header("Zombie States")]
    public float runSpeed = 4f;
    public float visionRadius = 15f;
    public float attackingRadius = 2f;
    public float stopChasingRadius = 25f;

    [Header("Zombie Sounds")]
    public AudioSource audioSource;
    public AudioClip idleSound;
    public AudioClip runSound;
    public AudioClip deathSound;

    [Range(0, 1)] public float idleVolume = 0.5f;
    [Range(0, 1)] public float runVolume = 1.0f;

    // State tracking
    private bool isChasing = false;
    private bool countedAsDead = false;

    // FIX: Only run Update after NavMesh initialization succeeded
    private bool isInitialized = false;

    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (zombieAgent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                zombieAgent.Warp(hit.position);
                isInitialized = true;
            }
            else
            {
                Debug.LogWarning($"[Zombie1] '{gameObject.name}' spawned too far from NavMesh at {transform.position}. Destroying.");
                // FIX: Delay destruction by one frame so the spawner's
                // Instantiate() call fully returns before this object is gone.
                // Also disable Animator immediately to stop the ghost animation.
                StartCoroutine(DestroyNextFrame());
                return;
            }
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 30f;
        }
    }

    // FIX: Disables animator first to prevent the floating animation ghost,
    // then destroys on the next frame so the spawner doesn't get a null mid-Instantiate.
    private IEnumerator DestroyNextFrame()
    {
        if (zombieAgent != null) zombieAgent.enabled = false;
        if (anim != null) anim.enabled = false;
        yield return null;
        Destroy(gameObject);
    }

    private void Update()
    {
        // FIX: Don't run any logic if NavMesh init failed or zombie is dead
        if (!isInitialized || health <= 0 || countedAsDead) return;

        bool playerInAttackingRadius = Physics.CheckSphere(transform.position, attackingRadius, PlayerLayer);
        float currentDetectionRange = isChasing ? stopChasingRadius : visionRadius;
        bool playerInDetectionRange = Physics.CheckSphere(transform.position, currentDetectionRange, PlayerLayer);

        if (playerInAttackingRadius)
        {
            Attack();
        }
        else if (playerInDetectionRange)
        {
            isChasing = true;
            Chase();
        }
        else
        {
            isChasing = false;
            Guard();
        }

        UpdateZombieAudio();
    }

    private void UpdateZombieAudio()
    {
        if (audioSource == null) return;

        AudioClip targetClip = null;
        float targetVolume = 1f;

        if (isChasing)
        {
            targetClip = runSound;
            targetVolume = runVolume;
        }
        else
        {
            targetClip = idleSound;
            targetVolume = idleVolume;
        }

        if (audioSource.clip != targetClip)
        {
            audioSource.clip = targetClip;
            audioSource.volume = targetVolume;
            audioSource.loop = true;

            if (targetClip != null)
                audioSource.Play();
            else
                audioSource.Stop();
        }
    }

    private void Guard()
    {
        // FIX: Safety check before any NavMeshAgent calls
        if (!zombieAgent.isOnNavMesh) return;

        zombieAgent.isStopped = false;
        zombieAgent.speed = walkSpeed;

        if (walkPoints.Length > 0)
        {
            zombieAgent.SetDestination(walkPoints[currentZombiePosition].transform.position);
            if (Vector3.Distance(transform.position, walkPoints[currentZombiePosition].transform.position) <= walkingpointRadius)
            {
                currentZombiePosition = Random.Range(0, walkPoints.Length);
            }
        }

        anim.SetBool("Walking", true);
        anim.SetBool("Running", false);
        anim.SetBool("Attacking", false);
    }

    private void Chase()
    {
        // FIX: Safety check before any NavMeshAgent calls
        if (!zombieAgent.isOnNavMesh) return;

        zombieAgent.isStopped = false;
        zombieAgent.speed = runSpeed;
        if (playerTransform != null)
            zombieAgent.SetDestination(playerTransform.position);

        anim.SetBool("Walking", false);
        anim.SetBool("Running", true);
        anim.SetBool("Attacking", false);
    }

    private void Attack()
    {
        // FIX: Safety check before any NavMeshAgent calls
        if (!zombieAgent.isOnNavMesh) return;

        zombieAgent.isStopped = true;
        zombieAgent.velocity = Vector3.zero;

        if (playerTransform != null)
        {
            Vector3 targetPosition = new Vector3(
                playerTransform.position.x,
                transform.position.y,
                playerTransform.position.z
            );
            transform.LookAt(targetPosition);
        }

        if (!alreadyAttacked)
        {
            anim.SetBool("Attacking", true);
            anim.SetBool("Walking", false);
            anim.SetBool("Running", false);

            RaycastHit hit;
            Vector3 origin = AttackingRaycastArea != null
                ? AttackingRaycastArea.transform.position
                : transform.position + Vector3.up;

            if (Physics.Raycast(origin, transform.forward, out hit, attackingRadius, PlayerLayer))
            {
                var pHealth = hit.collider.GetComponent<PlayerHealth>();
                if (pHealth != null) pHealth.TakeDamage(giveDamage);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0) return;
        health -= damage;
        if (health <= 0) Die();
    }

    private void Die()
    {
        if (countedAsDead) return;
        countedAsDead = true;

        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.OnZombieKilled();

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.volume = 1.0f;

            if (deathSound != null)
            {
                audioSource.clip = deathSound;
                audioSource.Play();
            }
        }

        anim.SetBool("Died", true);
        anim.SetBool("Walking", false);
        anim.SetBool("Running", false);
        anim.SetBool("Attacking", false);

        zombieAgent.isStopped = true;
        zombieAgent.enabled = false;

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule != null) capsule.enabled = false;

        this.enabled = false;
        Destroy(gameObject, 5f);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (health > 0) anim.SetBool("Attacking", false);
    }
}