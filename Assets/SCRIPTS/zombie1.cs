using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie1 : MonoBehaviour
{
    [Header("Zombie Health and Damage")]
    public float giveDamage = 10f;
    
    [Header("Zombie Things")]
    public NavMeshAgent zombieAgent;
    public Transform LookPoint;
    public Camera AttackingRaycastArea;
    public LayerMask PlayerLayer;

    [Header("Zombie Guarding Var")]
    public GameObject[] walkPoints;
    int currentZombiePosition = 0;
    public float zombieSpeed = 2f; // Added default value
    float walkingpointRadius = 2;

    // Cached references
    private Transform playerTransform;

    [Header("Zombie Attacking Var")]
    public float timeBetweenAttacks = 1.5f; // Added default value
    bool alreadyAttacked;   

    [Header("Zombie mood/states")]
    public float visionRadius = 15f;
    public float attackingRadius = 2f;
    public bool playerInvisionRadius;
    public bool playerInattackingRadius;
    
    public float stopChasingRadius = 25f; // Distance to STOP chasing (Must be bigger!)

    // Add this private variable to remember what we were doing
    private bool isChasing = false;
    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

      private void Update()
    {
        // 1. Check if we are close enough to attack
        bool playerInAttackingRadius = Physics.CheckSphere(transform.position, attackingRadius, PlayerLayer);

        // 2. Decide which detection radius to use
        // If we are ALREADY chasing, use the larger 'stopChasingRadius' (harder to escape)
        // If we are NOT chasing, use the smaller 'visionRadius' (harder to get noticed)
        float currentDetectionRange = isChasing ? stopChasingRadius : visionRadius;
        
        // 3. Check for player presence using that dynamic range
        bool playerInDetectionRange = Physics.CheckSphere(transform.position, currentDetectionRange, PlayerLayer);

        if (playerInAttackingRadius)
        {
            Attack();
        }
        else if (playerInDetectionRange)
        {
            // Player is found (or still being chased)
            isChasing = true;
            Chase();
        }
        else
        {
            // Player has escaped the larger radius completely
            isChasing = false;
            Guard();
        }
    }


    private void Guard()
    {
        if (walkPoints == null || walkPoints.Length == 0) return;

        // MOVEMENT
        if (zombieAgent != null && zombieAgent.isOnNavMesh)
        {
            zombieAgent.isStopped = false;
            zombieAgent.speed = zombieSpeed;
            zombieAgent.SetDestination(walkPoints[currentZombiePosition].transform.position);
        }
        else
        {
            // Fallback if no NavMesh
            transform.position = Vector3.MoveTowards(transform.position, walkPoints[currentZombiePosition].transform.position, Time.deltaTime * zombieSpeed);
        }

        // CHECK DISTANCE
        // Use Vector3.Distance to ignore height differences (optional, but safer)
        if (Vector3.Distance(transform.position, walkPoints[currentZombiePosition].transform.position) <= walkingpointRadius)
        {
            // RANDOM PATROL (Matches your image snippet)
            int previousPos = currentZombiePosition;
            // Loop until we get a new position so he doesn't go to the same spot twice
            while(currentZombiePosition == previousPos && walkPoints.Length > 1) {
                currentZombiePosition = Random.Range(0, walkPoints.Length);
            }
        }
        
        // NOTE: I removed transform.LookAt() here to prevent jitter. 
        // NavMeshAgent handles rotation automatically while moving.
    }

    private void Chase()
    {
        if (playerTransform == null) return;

        if (zombieAgent != null && zombieAgent.isOnNavMesh)
        {
            zombieAgent.isStopped = false;
            zombieAgent.speed = zombieSpeed * 2; // Optional: Run faster when chasing?
            zombieAgent.SetDestination(playerTransform.position);
        }
    }

    private void Attack()
    {
        if (zombieAgent != null) zombieAgent.isStopped = true; // Stop moving

        // Look at player (keep this, it's good for attacking)
        if (playerTransform != null)
        {
            // Lock rotation to Y axis only so zombie doesn't tilt up/down
            Vector3 targetPostition = new Vector3(playerTransform.position.x, this.transform.position.y, playerTransform.position.z);
            this.transform.LookAt(targetPostition);
        }

        if (!alreadyAttacked)
        {
            RaycastHit hit;
            // Aim at Player's Chest (Position + Up Vector) rather than feet
            Vector3 targetPoint = playerTransform != null ? playerTransform.position + Vector3.up * 1.5f : transform.forward;
            
            Vector3 origin = AttackingRaycastArea != null ? AttackingRaycastArea.transform.position : transform.position + Vector3.up;
            Vector3 dir = (targetPoint - origin).normalized;

            // Draw ray in editor to debug
            Debug.DrawRay(origin, dir * attackingRadius, Color.red, 1f);

            if (Physics.Raycast(origin, dir, out hit, attackingRadius, PlayerLayer))
            {
                Debug.Log("Zombie Hit: " + hit.collider.name);
                // Try-Catch or Null check for component
                var playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if(playerHealth != null)
                {
                    playerHealth.TakeDamage(giveDamage);
                }
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}