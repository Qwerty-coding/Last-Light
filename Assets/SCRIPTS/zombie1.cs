using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie1 : MonoBehaviour
{
    [Header("Zombie Health and Damage")]
    public float giveDamage = 10f;
    public float health = 100f;
    
    [Header("Zombie Things")]
    public NavMeshAgent zombieAgent;
    public Transform playerTransform; 
    public Camera AttackingRaycastArea;
    public LayerMask PlayerLayer;

    [Header("Zombie Guarding Var")]
    public GameObject[] walkPoints;
    int currentZombiePosition = 0;
    public float walkSpeed = 1.5f; 
    float walkingpointRadius = 2;

    [Header("Zombie Attacking Var")]
    public float timeBetweenAttacks = 1.5f;
    bool alreadyAttacked;   

    [Header("Zombie Animation")]
    public Animator anim;

    [Header("Zombie mood/states")]
    public float runSpeed = 4f; 
    public float visionRadius = 15f;
    public float attackingRadius = 2f;
    public float stopChasingRadius = 25f; 

    private bool isChasing = false;

    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        // 1. THE KILL-SWITCH: Check death at the VERY TOP of Update
        if (health <= 0 || anim.GetBool("Died")) 
        {
            return; // Exit immediately so no other logic (rotation/attack) runs
        }

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
    }

    private void Guard()
    {
        zombieAgent.isStopped = false;
        zombieAgent.speed = walkSpeed; 
        zombieAgent.SetDestination(walkPoints[currentZombiePosition].transform.position);

        if (Vector3.Distance(transform.position, walkPoints[currentZombiePosition].transform.position) <= walkingpointRadius)
        {
            currentZombiePosition = Random.Range(0, walkPoints.Length);
        }

        anim.SetBool("Walking", true);
        anim.SetBool("Running", false);
        anim.SetBool("Attacking", false);
    }

    private void Chase()
    {
        zombieAgent.isStopped = false;
        zombieAgent.speed = runSpeed; 
        zombieAgent.SetDestination(playerTransform.position);

        anim.SetBool("Walking", false);
        anim.SetBool("Running", true);
        anim.SetBool("Attacking", false);
    }

    private void Attack()
    {
        zombieAgent.isStopped = true; 
        zombieAgent.speed = 0;

        // Only rotate if alive
        if (playerTransform != null)
        {
            Vector3 targetPostition = new Vector3(playerTransform.position.x, this.transform.position.y, playerTransform.position.z);
            this.transform.LookAt(targetPostition);
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
                // Ensure PlayerHealth script exists on your player
                var pHealth = hit.collider.GetComponent<PlayerHealth>();
                if(pHealth != null) pHealth.TakeDamage(giveDamage);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0) return;

        health -= damage;
        
        if (health <= 0)
        {
            Die(); // Use a centralized Die function
        }
    }

    public void Die()
    {
        // Set animation triggers
        anim.SetBool("Died", true);
        anim.SetBool("Walking", false);
        anim.SetBool("Running", false);
        anim.SetBool("Attacking", false);

        // Stop movement and disable AI
        zombieAgent.isStopped = true; 
        zombieAgent.enabled = false; 

        // Stop all pending attacks
        CancelInvoke(nameof(ResetAttack));

        // Disable the collider so the player can walk over the body
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule != null) capsule.enabled = false;

        // Cleanup body
        Destroy(gameObject, 5f);

        // Disable this script so Update() stops running forever
        this.enabled = false; 
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        anim.SetBool("Attacking", false);
    }
}