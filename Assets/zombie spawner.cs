using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject zombiePrefab;    
    public string zombieTag = "Zombie"; // Matches your tag from the screenshot
    public float spawnInterval = 4f;   
    public int maxZombies = 5;         // The cap you requested

    [Header("Spawn Radius")]
    public float minDistance = 8f;     
    public float maxDistance = 20f;    

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

    private void StartSpawning()
    {
        isSpawning = true;
        // This starts a loop that runs every 4 seconds
        InvokeRepeating(nameof(SpawnAroundPlayer), 0f, spawnInterval);
    }

    private void SpawnAroundPlayer()
    {
        if (zombiePrefab == null || playerTransform == null) return;

        // 1. Check how many zombies currently exist
        int currentZombieCount = GameObject.FindGameObjectsWithTag(zombieTag).Length;

        // 2. STOP if we are at or above the limit
        if (currentZombieCount >= maxZombies)
        {
            Debug.Log("Limit reached. No spawn this time.");
            return; // This line prevents spawning until a zombie is destroyed
        }

        // 3. Only runs if count is less than 5
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minDistance, maxDistance);
        Vector3 spawnPos = playerTransform.position + new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;
        spawnPos.y = playerTransform.position.y; 

        Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
    }
}