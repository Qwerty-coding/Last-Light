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

            if (isGun) { inventory.GiveGun(); itemPickedUp = true; }
            else if (isAxe) { inventory.GiveAxe(); itemPickedUp = true; }
            else if (isKey) { inventory.GiveKey(); itemPickedUp = true; }
            else if (woodAmount > 0) { inventory.AddLogs(woodAmount); itemPickedUp = true; }

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