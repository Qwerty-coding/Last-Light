using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject zombiePrefab;    
    public string zombieTag = "Zombie";
    public float spawnInterval = 4f;   
    public int maxZombies = 5;

    [Header("Spawn Radius")]
    public float minDistance = 8f;     
    public float maxDistance = 20f;

    [Header("Boss Arena (Optional)")]
    public Transform bossArenaCenter; // Drag boss arena center here to prevent spawning there
    public float arenaExclusionRadius = 50f; // Don't spawn within this radius of boss

    private bool isSpawning = false;
    private Transform playerTransform;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        CheckForGun();

        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnInventoryChange.AddListener(CheckForGun);
        }
    }

    private void CheckForGun()
    {
        if (isSpawning) return;

        if (SimpleInventory.Instance != null && SimpleInventory.Instance.HasItem("Gun"))
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (isSpawning) return; // Prevent multiple calls

        isSpawning = true;
        Debug.Log("🧟 Zombie spawning STARTED!");
        InvokeRepeating(nameof(SpawnAroundPlayer), 0f, spawnInterval);
    }

    // PUBLIC METHOD: Call this to stop spawning (used by BossTeleporter)
    public void StopSpawning()
    {
        if (!isSpawning) return;

        isSpawning = false;
        CancelInvoke(nameof(SpawnAroundPlayer));
        Debug.Log("🛑 Zombie spawning STOPPED!");
    }

    private void SpawnAroundPlayer()
    {
        if (!isSpawning) return; // Safety check
        if (zombiePrefab == null || playerTransform == null) return;

        // 1. Check zombie count limit
        int currentZombieCount = GameObject.FindGameObjectsWithTag(zombieTag).Length;
        
        if (currentZombieCount >= maxZombies)
        {
            Debug.Log($"Zombie limit reached ({currentZombieCount}/{maxZombies}). Waiting for death...");
            return;
        }

        // 2. Try to find valid spawn position (with boss arena check)
        Vector3? spawnPos = FindValidSpawnPosition();

        if (spawnPos.HasValue)
        {
            GameObject newZombie = Instantiate(zombiePrefab, spawnPos.Value, Quaternion.identity);
            Debug.Log($"✓ Spawned zombie. Total: {currentZombieCount + 1}/{maxZombies}");
        }
        else
        {
            Debug.LogWarning("Could not find valid spawn position (might be too close to boss arena)");
        }
    }

    private Vector3? FindValidSpawnPosition()
    {
        int maxAttempts = 10; // Try 10 times to find a good spot

        for (int i = 0; i < maxAttempts; i++)
        {
            // Generate random position around player
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minDistance, maxDistance);
            
            Vector3 spawnPos = playerTransform.position + 
                               new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;
            spawnPos.y = playerTransform.position.y;

            // CHECK: Is this position too close to boss arena?
            if (bossArenaCenter != null)
            {
                float distanceToArena = Vector3.Distance(spawnPos, bossArenaCenter.position);
                
                if (distanceToArena < arenaExclusionRadius)
                {
                    Debug.Log($"Spawn blocked: Too close to boss arena ({distanceToArena:F1}m < {arenaExclusionRadius}m)");
                    continue; // Try again
                }
            }

            // Position is valid!
            return spawnPos;
        }

        return null; // Failed to find valid position after 10 tries
    }

    // VISUALIZATION: Show spawn area and boss exclusion zone in Scene view
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        if (playerTransform != null)
        {
            // Yellow = Min spawn distance
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, minDistance);

            // Green = Max spawn distance
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, maxDistance);
        }

        // Red = Boss arena exclusion zone
        if (bossArenaCenter != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f); // Transparent red
            Gizmos.DrawSphere(bossArenaCenter.position, arenaExclusionRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(bossArenaCenter.position, arenaExclusionRadius);
        }
    }
}