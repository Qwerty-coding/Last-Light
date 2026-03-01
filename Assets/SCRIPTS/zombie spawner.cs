using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject zombiePrefab;
    public Transform player;

    [Header("Spawn Settings")]
    [Tooltip("Radius around player where zombies spawn")]
    public float spawnRadius = 5f;

    [Tooltip("Max zombies allowed within spawnRadius at any time")]
    public int maxZombiesInRadius = 5;

    [Tooltip("How often spawner checks if new zombies are needed")]
    public float checkInterval = 2f;

    private bool isActive = false;
    private List<GameObject> activeZombies = new List<GameObject>();
    private Coroutine spawnCoroutine;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void StartSpawning()
    {
        if (isActive) return;
        isActive = true;
        spawnCoroutine = StartCoroutine(SpawnLoop());
        Debug.Log("[ZombieSpawner] Spawning started.");
    }

    public void StopSpawning()
    {
        isActive = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        Debug.Log("[ZombieSpawner] Spawning stopped.");
    }

    private IEnumerator SpawnLoop()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(checkInterval);

            // Remove killed/destroyed zombies
            activeZombies.RemoveAll(z => z == null);

            // Count zombies currently within spawn radius of player
            int zombiesInRadius = CountZombiesInRadius();

            // Spawn one if below max
            if (zombiesInRadius < maxZombiesInRadius)
            {
                yield return StartCoroutine(TrySpawnZombie());
            }
        }
    }

    private int CountZombiesInRadius()
    {
        int count = 0;
        foreach (GameObject z in activeZombies)
        {
            if (z == null) continue;
            if (Vector3.Distance(z.transform.position, player.position) <= spawnRadius)
                count++;
        }
        return count;
    }

    private IEnumerator TrySpawnZombie()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();

        NavMeshHit navHit;
        if (!NavMesh.SamplePosition(spawnPos, out navHit, 3f, NavMesh.AllAreas))
        {
            Debug.LogWarning("[ZombieSpawner] Could not find NavMesh at spawn position, skipping.");
            yield break;
        }

        GameObject zombie = Instantiate(zombiePrefab, navHit.position, Quaternion.identity);

        // Wait one frame for NavMeshAgent.Warp() in Zombie1.Awake() to settle
        yield return null;

        // Zombie may have self-destructed if off NavMesh
        if (zombie == null)
        {
            Debug.LogWarning("[ZombieSpawner] Zombie destroyed itself in Awake, skipping.");
            yield break;
        }

        NavMeshAgent agent = zombie.GetComponent<NavMeshAgent>();
        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning("[ZombieSpawner] Zombie not on NavMesh after spawn, destroying.");
            Destroy(zombie);
            yield break;
        }

        activeZombies.Add(zombie);
        Debug.Log($"[ZombieSpawner] Zombie spawned. In radius: {CountZombiesInRadius()}/{maxZombiesInRadius}");
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Spawn on edge of radius so zombies walk toward player
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        return new Vector3(
            player.position.x + randomCircle.x,
            player.position.y,
            player.position.z + randomCircle.y
        );
    }

    public void ClearAllZombies()
    {
        foreach (GameObject z in activeZombies)
            if (z != null) Destroy(z);
        activeZombies.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, spawnRadius);
    }
}