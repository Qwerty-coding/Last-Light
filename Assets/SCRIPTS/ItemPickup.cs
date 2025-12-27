using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Settings")]
    // TYPE EXACTLY: "Gun", "Axe", "Key", or "Logs"
    // This must match the spelling in SimpleInventoryUI checks
    public string itemID; 
    public int amount = 1;

    [Header("Debug View")]
    public bool isPlayerInRange = false;

    // Double-pickup prevention
    private bool wasCollected = false; 

    private void Update()
    {
        // Check if player presses "E"
        if (Input.GetKeyDown(KeyCode.E))
        {
            // We check TWO things:
            // 1. Are we standing in the trigger? (isPlayerInRange)
            // 2. Are we looking at it? (SelectionManager.instance.onTarget)
            if (isPlayerInRange && SelectionManager.instance.onTarget) 
            {
                // Safety check to ensure the UI is actually active
                if(SelectionManager.instance.interaction_Info_UI.activeSelf) 
                {
                    CollectItem();
                }
            }
        }
    }

    private void CollectItem()
    {
        if (wasCollected) return;

        // 1. GET INVENTORY REFERENCE
        SimpleInventory inventory = SimpleInventory.Instance;

        if (inventory != null)
        {
            // 2. ADD ITEM TO DATA (The Backpack)
            inventory.AddItem(itemID, amount);

            // 3. UPDATE OBJECTIVES (The Story)
            CheckStoryTriggers();

            // 4. CLEANUP
            wasCollected = true;
            
            // Turn off the UI immediately so it doesn't get stuck on screen
            if (SelectionManager.instance != null)
            {
                SelectionManager.instance.onTarget = false;
                SelectionManager.instance.interaction_Info_UI.SetActive(false);
            }

            // Destroy the object
            Destroy(gameObject);
        }
    }

    // This function acts as the "Bridge" to your ObjectiveManager
    private void CheckStoryTriggers()
    {
        if (ObjectiveManager.Instance == null) return;

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
                // Loop in case you picked up a stack of 3 logs
                for (int i = 0; i < amount; i++)
                {
                    ObjectiveManager.Instance.AddWood();
                }
                break;
                
            case "Key":
                // Keys don't usually trigger a new objective text, but you can add it here if you want!
                break;
        }
    }

    // --- PHYSICS TRIGGERS ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}