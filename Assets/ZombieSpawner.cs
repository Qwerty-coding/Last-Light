using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject zombiePrefab;
    public Transform playerTransform; // Drag your 'PLAYER' object here

    [Header("Settings")]
    public float timeBetweenSpawns = 5f;
    public float spawnRadius = 20f;   // How far from the player they spawn
    public float minDistance = 10f;  // Don't spawn too close to the player
    public bool isSpawningActive = false; 

    private float nextSpawnTime;

    void Update()
    {
        if (isSpawningActive)
        {
            if (Time.time >= nextSpawnTime)
            {
                SpawnZombieNearPlayer();
                nextSpawnTime = Time.time + timeBetweenSpawns;
            }
        }
    }

    public void StartSpawning()
    {
        isSpawningActive = true;
        nextSpawnTime = Time.time;
    }

    void SpawnZombieNearPlayer()
    {
        if (zombiePrefab == null || playerTransform == null) return;

        // 1. Pick a random direction
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minDistance, spawnRadius);
        Vector3 spawnPos = new Vector3(playerTransform.position.x + randomCircle.x, playerTransform.position.y, playerTransform.position.z + randomCircle.y);

        // 2. Snap the position to the NavMesh (so they don't spawn inside trees/walls)
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 5f, NavMesh.AllAreas))
        {
            Instantiate(zombiePrefab, hit.position, Quaternion.identity);
        }
    }
}