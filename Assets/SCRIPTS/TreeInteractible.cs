using UnityEngine;

public class TreeInteractable : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Log Settings")]
    public GameObject logPrefab; 
    public int logAmount = 1;    // CHANGED: Set to 1 so only one log drops

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Chop()
    {
        currentHealth--;
        Debug.Log("Tree chopped! Health left: " + currentHealth);

        if (currentHealth <= 0)
        {
            DropLogs();
            Destroy(gameObject);
        }
    }

    private void DropLogs()
    {
        if (logPrefab != null)
        {
            for (int i = 0; i < logAmount; i++)
            {
                // Spawns log slightly above the ground
                Vector3 spawnPos = transform.position + new Vector3(0, 1f, 0);
                
                // Keep the random spread in case you decide to increase it later
                spawnPos += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));

                Instantiate(logPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}