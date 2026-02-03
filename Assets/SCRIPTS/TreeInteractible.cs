using UnityEngine;

public class TreeInteractable : MonoBehaviour
{
    // Health set to 1 for instant chopping
    public int maxHealth = 1; 
    private int currentHealth;

    [Header("Log Settings")]
    public GameObject logPrefab; 
    public int logAmount = 1;

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

        // STEP 1: Turn off the Tree's collider specifically for this moment.
        // This ensures the Raycast goes THROUGH the tree and hits the Ground.
        Collider treeCollider = GetComponent<Collider>();
        if (treeCollider != null)
        {
            treeCollider.enabled = false;
        }

        for (int i = 0; i < logAmount; i++)
        {
            // STEP 2: Start the ray slightly up from the tree root
            Vector3 rayStart = transform.position + Vector3.up * 1.5f;

            // Optional: Add random spread if you drop more than 1 log
            if (logAmount > 1)
            {
                rayStart += new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            }

            RaycastHit hit;

            // STEP 3: Shoot Ray down to find the exact floor position
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 10f))
            {
                // Found the ground!
                Vector3 spawnPos = hit.point;
                
                // STEP 4: Add a tiny upward offset (0.2f) so the log isn't buried half-way in the dirt
                spawnPos.y += 0.2f;

                // Instantiate the log on the ground
                // Quaternion.Euler(0, Random.Range(0, 360), 0) rotates it randomly on the Y axis
                Instantiate(logPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
            else
            {
                // Fallback: If ray misses (rare), spawn at tree position
                Instantiate(logPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}