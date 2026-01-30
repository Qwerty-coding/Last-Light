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
        if (logPrefab == null) return;

        for (int i = 0; i < logAmount; i++)
        {
            // Start ray slightly above the tree
            Vector3 rayStart = transform.position + Vector3.up * 2f;

            // Small random spread (optional)
            rayStart += new Vector3(
                Random.Range(-0.5f, 0.5f),
                0,
                Random.Range(-0.5f, 0.5f)
            );

            RaycastHit hit;

            // Raycast straight down
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 10f))
            {
                // Place log on the ground
                Vector3 spawnPos = hit.point;

                Instantiate(logPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

}