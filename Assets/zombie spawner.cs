using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject zombiePrefab;    // Drag your Zombie Prefab here
    public float spawnInterval = 4f;   // Time between spawns (seconds)
    
    [Header("Spawn Radius")]
    public float minDistance = 8f;     // Keep zombies from spawning on top of player
    public float maxDistance = 20f;    // The furthest they can spawn

    private bool isSpawning = false;
    private Transform playerTransform;

    private void Start()
    {
        // 1. Find the player automatically by Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("ZombieSpawner: Could not find object with tag 'Player'!");
        }

        // 2. Initial check in case player starts with a gun
        CheckForGun();

        // 3. Listen for future inventory changes
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnInventoryChange.AddListener(CheckForGun);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent errors
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnInventoryChange.RemoveListener(CheckForGun);
        }
    }

    // This method is called automatically whenever the Inventory updates
    private void CheckForGun()
    {
        if (isSpawning) return; // Don't start twice

        if (SimpleInventory.Instance != null && SimpleInventory.Instance.HasItem("Gun"))
        {
            StartSpawning();
        }
    }

    private void StartSpawning()
    {
        isSpawning = true;
        Debug.Log("🧟 Gun detected! Starting zombie waves.");
        InvokeRepeating(nameof(SpawnAroundPlayer), 0f, spawnInterval);
    }

    private void SpawnAroundPlayer()
    {
        if (zombiePrefab == null || playerTransform == null) return;

        // --- MATH: Create a random position in a ring around the player ---
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minDistance, maxDistance);

        Vector3 spawnOffset = new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;
        Vector3 spawnPos = playerTransform.position + spawnOffset;

        // Keep the spawn height at ground level
        spawnPos.y = playerTransform.position.y; 

        Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
    }
}