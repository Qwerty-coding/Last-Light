using UnityEngine;

public class StoryPickup : MonoBehaviour
{
    [Header("Settings")]
    // IMPORANT: Case Sensitive! Must match SimpleInventoryUI checks ("Gun", "Axe", "Key", "Logs")
    public string itemID = "Gun"; 
    public int amount = 1;

    [Header("Debug")]
    public bool isPlayerInRange;

    private void Update()
    {
        // 1. Check if player is close and presses E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Collect();
        }
    }

    private void Collect()
    {
        // --- 1. ADD TO INVENTORY (The Data) ---
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.AddItem(itemID, amount);
        }
        else
        {
            Debug.LogError("SimpleInventory is missing from the scene!");
            return;
        }

        // --- 2. UPDATE OBJECTIVES (The Story) ---
        if (ObjectiveManager.Instance != null)
        {
            CheckStoryTriggers();
        }

        // --- 3. CLEANUP ---
        // Hide the item so it looks picked up
        gameObject.SetActive(false);
        // Or use Destroy(gameObject); if you prefer
    }

    private void CheckStoryTriggers()
    {
        // This is the specific logic connecting Items to Objectives
        
        switch (itemID)
        {
            case "Gun":
                // Found Gun -> Go to Tower
                ObjectiveManager.Instance.UpdateObjective("Reach the Fire Tower");
                break;

            case "Axe":
                // Found Axe -> Go chop wood
                ObjectiveManager.Instance.UpdateObjective("Gather Wood (0/10)");
                break;

            case "Logs":
                // Found Wood -> Update the specific wood counter
                // We loop this just in case you pick up a stack of 3 logs at once
                for (int i = 0; i < amount; i++)
                {
                    ObjectiveManager.Instance.AddWood();
                }
                break;
                
            case "Key":
                // Keys might not update an objective, but you can add logic here if they do!
                Debug.Log("Key collected - no objective update needed.");
                break;
        }
    }

    // --- TRIGGER LOGIC ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}