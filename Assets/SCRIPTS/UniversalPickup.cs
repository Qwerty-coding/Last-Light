using UnityEngine;

public class UniversalPickup : MonoBehaviour
{
    [Header("Settings")]
    public string itemID = "Gun"; // Type EXACTLY: "Gun", "Key", "Axe", or "Logs"
    public int amount = 1;

    public bool isPlayerInRange;

    private void Update()
    {
        // If player is close and presses E
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange)
        {
            Collect();
        }
    }

    private void Collect()
    {
        Debug.Log("🚨 I AM THE GHOST! My name is: " + gameObject.name, gameObject);
        
        if (SimpleInventory.Instance != null)
        {
            // Add to inventory by NAME
            SimpleInventory.Instance.AddItem(itemID, amount);
            
            // Optional: Hide/Destroy the object
            gameObject.SetActive(false); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}