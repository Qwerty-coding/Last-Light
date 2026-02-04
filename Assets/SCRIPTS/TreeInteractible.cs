using UnityEngine;

public class TreeInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string treeName = "Birch Tree"; 

    [Header("Tree Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Log Settings")]
    public GameObject logPrefab; 
    public int logAmount = 1;    

    void Start()
    {
        currentHealth = maxHealth;
        // Verify we have a collider
        if (GetComponent<Collider>() == null && GetComponentInChildren<Collider>() == null)
        {
            Debug.LogWarning(gameObject.name + " has no Collider! Raycast will not work.");
        }
    }

    public void Chop()
    {
        currentHealth--;
        Debug.Log(treeName + " chopped! Health: " + currentHealth);

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
            Vector3 rayStart = transform.position + Vector3.up * 2f;
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 10f))
            {
                Instantiate(logPrefab, hit.point, Quaternion.identity);
            }
        }
    }
}