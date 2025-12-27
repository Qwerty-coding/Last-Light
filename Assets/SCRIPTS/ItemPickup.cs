using UnityEngine;

public class ItemPickup : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("UI Info")]
    public string itemName; // Name to show on screen (e.g. "Rusty Axe")

    [Header("Inventory Settings")]
    public bool isGun;
    public bool isAxe;
    public int woodAmount = 0;

    // Public so SelectionManager can read it
    [HideInInspector]
=======
    [Header("Settings")]
    // TYPE EXACTLY: "Gun", "Axe", "Key", or "Logs"
    // This must match the spelling in SimpleInventoryUI checks
    public string itemID; 
    public int amount = 1;

    [Header("Debug View")]
>>>>>>> Stashed changes
    public bool isPlayerInRange = false;

    private void Update()
    {
<<<<<<< Updated upstream
        // LOGIC: Press E + Player is Close + Looking at Object (SelectionManager)
        // We use 'SelectionManager.instance.onTarget' to ensure we are actually looking at THIS object
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange && SelectionManager.instance.onTarget)
        {
            CollectItem();
=======
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
>>>>>>> Stashed changes
        }
    }

    private void CollectItem()
    {
<<<<<<< Updated upstream
        FixedInventory inventory = FixedInventory.Instance;
=======
        if (wasCollected) return;

        // 1. GET INVENTORY REFERENCE
        SimpleInventory inventory = SimpleInventory.Instance;
>>>>>>> Stashed changes

        if (inventory != null)
        {
            // 2. ADD ITEM TO DATA (The Backpack)
            inventory.AddItem(itemID, amount);

<<<<<<< Updated upstream
            if (isGun)
            {
                inventory.GiveGun(); //
                Debug.Log("Picked up Gun");
                itemPickedUp = true;
            }
            else if (isAxe)
            {
                inventory.GiveAxe(); //
                Debug.Log("Picked up Axe");
                itemPickedUp = true;
            }
            else if (woodAmount > 0)
            {
                inventory.AddLogs(woodAmount); //
                Debug.Log("Picked up Wood");
                itemPickedUp = true;
            }
=======
            // 3. UPDATE OBJECTIVES (The Story)
            CheckStoryTriggers();
>>>>>>> Stashed changes

            // 4. CLEANUP
            wasCollected = true;
            
            // Turn off the UI immediately so it doesn't get stuck on screen
            if (SelectionManager.instance != null)
            {
<<<<<<< Updated upstream
                // Clear the UI text immediately so it doesn't get stuck
                SelectionManager.instance.onTarget = false;
                SelectionManager.instance.interaction_Info_UI.SetActive(false);
                
                Destroy(gameObject);
=======
                SelectionManager.instance.onTarget = false;
                SelectionManager.instance.interaction_Info_UI.SetActive(false);
>>>>>>> Stashed changes
            }

            // Destroy the object
            Destroy(gameObject);
        }
    }

<<<<<<< Updated upstream
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
=======
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
>>>>>>> Stashed changes
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}