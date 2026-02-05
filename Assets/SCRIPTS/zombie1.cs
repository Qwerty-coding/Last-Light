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
    public AudioClip idleSound;   // Drag your Zombie_Idle audio here
    public AudioClip runSound;    // Drag your Zombie_Run/Scream audio here
    public AudioClip deathSound;  // Drag your Zombie_Death audio here
    
    [Range(0, 1)] public float idleVolume = 0.5f;
    [Range(0, 1)] public float runVolume = 1.0f;

    // State tracking
    private bool isChasing = false;
    private bool countedAsDead = false;

    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
        if (playerTransform == null) playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (zombieAgent != null)
    {
        NavMeshHit hit;
        // Search for a NavMesh within 5 units of the spawn point
        if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
        {
            zombieAgent.Warp(hit.position); // Snap to the valid ground position found
        }
        else
        {
            // Fallback: If no ground found, disable agent to let physics gravity work (if you have a Rigidbody)
            // or just destroy the glitchy zombie so it doesn't break the game.
            Debug.LogWarning("Zombie spawned too far from NavMesh! Destroying to prevent glitches.");
            Destroy(gameObject);
            return;
        }
    }
    
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        
        // --- AUDIO SETUP ---
        // Ensure 3D settings are correct for horror atmosphere
        if (audioSource != null)
        {
            audioSource.spatialBlend = 1f; // Make it fully 3D
            audioSource.playOnAwake = false;
            audioSource.loop = true; // Default to looping for movement
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 30f;
        }
    }

    private void Update()
    {
        if (health <= 0 || countedAsDead) return;

        bool playerInAttackingRadius = Physics.CheckSphere(transform.position, attackingRadius, PlayerLayer);
        float currentDetectionRange = isChasing ? stopChasingRadius : visionRadius;
        bool playerInDetectionRange = Physics.CheckSphere(transform.position, currentDetectionRange, PlayerLayer);

        if (playerInAttackingRadius)
        {
            Attack();
        }
        else if (playerInDetectionRange)
        {
            // Player is seen -> Switch to Chase State
            isChasing = true;
            Chase();
        }
        else
        {
            // Player lost -> Switch to Guard State
            isChasing = false;
            Guard();
        }

        // Update the audio based on the decisions made above
        UpdateZombieAudio();
    }

    private void UpdateZombieAudio()
    {
        if (audioSource == null) return;

        AudioClip targetClip = null;
        float targetVolume = 1f;

        // DECISION LOGIC: Determine which sound *should* be playing
        if (isChasing)
        {
            // If chasing, play running sound
            targetClip = runSound;
            targetVolume = runVolume;
        }
        else
        {
            // If patrolling or standing still, play idle sound
            targetClip = idleSound;
            targetVolume = idleVolume;
        }

        // APPLY LOGIC: Only switch tracks if we aren't already playing the correct one
        if (audioSource.clip != targetClip)
        {
            audioSource.clip = targetClip;
            audioSource.volume = targetVolume;
            
            // Ensure we are looping for idle/run
            audioSource.loop = true; 
            
            if (targetClip != null) 
                audioSource.Play();
            else 
                audioSource.Stop();
        }
    }

    private void Guard()
    {
        zombieAgent.isStopped = false;
        zombieAgent.speed = walkSpeed; 

        if(walkPoints.Length > 0)
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
        zombieAgent.isStopped = false;
        zombieAgent.speed = runSpeed; 
        if(playerTransform != null) zombieAgent.SetDestination(playerTransform.position);

        anim.SetBool("Walking", false);
        anim.SetBool("Running", true);
        anim.SetBool("Attacking", false);
    }

    private void Attack()
    {
        zombieAgent.isStopped = true; 
        zombieAgent.velocity = Vector3.zero;

        if (playerTransform != null)
        {
            Vector3 targetPostition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(targetPostition);
        }

        if (!alreadyAttacked)
        {
            anim.SetBool("Attacking", true);
            anim.SetBool("Walking", false);
            anim.SetBool("Running", false);

            RaycastHit hit;
            Vector3 origin = AttackingRaycastArea != null ? AttackingRaycastArea.transform.position : transform.position + Vector3.up;

            if (Physics.Raycast(origin, transform.forward, out hit, attackingRadius, PlayerLayer))
            {
                // Assuming you have a PlayerHealth script
                var pHealth = hit.collider.GetComponent<PlayerHealth>(); // Ensure this class exists in your project
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

        // If you have an objective manager
        if (ObjectiveManager.Instance != null) ObjectiveManager.Instance.OnZombieKilled();

        // --- DEATH AUDIO LOGIC ---
        if (audioSource != null)
        {
            audioSource.Stop();           // Stop the looping Run/Idle sound immediately
            audioSource.loop = false;     // Death sound should only play once
            audioSource.volume = 1.0f;    // Max volume for death scream
            
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

        this.enabled = false; // Stops the Update loop
        Destroy(gameObject, 5f);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (health > 0) anim.SetBool("Attacking", false);
    }
}