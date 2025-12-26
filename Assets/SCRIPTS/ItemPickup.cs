using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("UI Info")]
    public string itemName;

    [Header("Inventory Settings")]
    public bool isGun;
    public bool isAxe;
    public bool isKey; 
    public int woodAmount = 0;

    // Public so we can see it in Inspector while playing
    public bool isPlayerInRange = false;

    // Double-pickup prevention
    private bool wasCollected = false; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerInRange && SelectionManager.instance.onTarget) 
            {
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

        FixedInventory inventory = FixedInventory.Instance;

        if (inventory != null)
        {
            bool itemPickedUp = false;

            if (isGun) 
            { 
                inventory.GiveGun(); 
                itemPickedUp = true; 
                
                // UPDATE: Found Gun -> Go to Tower
                ObjectiveManager.Instance.UpdateObjective("Reach the Fire Tower");
            }
            else if (isAxe) 
            { 
                inventory.GiveAxe(); 
                itemPickedUp = true; 
                
                // UPDATE: Found Axe -> Chop Trees
                ObjectiveManager.Instance.UpdateObjective("Gather Wood (0/10)");
            }
            else if (isKey) 
            { 
                inventory.GiveKey(); 
                itemPickedUp = true; 
            }
            else if (woodAmount > 0) 
            { 
                inventory.AddLogs(woodAmount); 
                itemPickedUp = true; 
                
                // UPDATE: Add to wood counter
                // Loop in case pickup is a stack of wood
                for(int i=0; i < woodAmount; i++) 
                    ObjectiveManager.Instance.AddWood();
            }

            if (itemPickedUp)
            {
                wasCollected = true;
                if (SelectionManager.instance != null)
                {
                    SelectionManager.instance.onTarget = false;
                    SelectionManager.instance.interaction_Info_UI.SetActive(false);
                }
                Destroy(gameObject);
            }
        }
    }

    // --- DEBUG TRIGGERS ---
    private void OnTriggerEnter(Collider other)
    {
        // Print EXACTLY what touched the trigger
        Debug.Log("Something touched the trigger: " + other.name + " | Tag: " + other.tag);

        if (other.CompareTag("Player")) 
        {
            Debug.Log("SUCCESS: Player Detected!");
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}