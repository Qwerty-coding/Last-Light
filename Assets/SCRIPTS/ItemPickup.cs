using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Settings")]
    public string itemID;   // Make sure this matches ItemNames.LOGS exactly in Inspector!
    public int amount = 1;

    [Header("Debug View")]
    public bool isPlayerInRange = false;

    private bool wasCollected = false;

    // FIX 1: Clean the ID as soon as the object loads
    private void Awake()
    {
        if (!string.IsNullOrEmpty(itemID))
        {
            itemID = itemID.Trim(); // Removes accidental spaces like "Logs "
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Consolidate checks for cleaner reading
            if (isPlayerInRange && 
                SelectionManager.instance != null && 
                SelectionManager.instance.onTarget && 
                SelectionManager.instance.interaction_Info_UI.activeSelf)
            {
                CollectItem();
            }
        }
    }

    private void CollectItem()
    {
        if (wasCollected) return;
        if (SimpleInventory.Instance == null) return;

        Debug.Log($"[ItemPickup] Collecting: '{itemID}' Amount: {amount}");

        // ✅ ADD ITEM
        SimpleInventory.Instance.AddItem(itemID, amount);

        // ✅ CHECK TRIGGER MATCHING
        // This ensures the logic fires even if there is a tiny casing mismatch
        // assuming your ItemNames constants are standard
        HandleObjectiveTrigger();

        wasCollected = true;

        if (SelectionManager.instance != null)
        {
            SelectionManager.instance.onTarget = false;
            SelectionManager.instance.interaction_Info_UI.SetActive(false);
        }

        Destroy(gameObject);
    }

    private void HandleObjectiveTrigger()
    {
        if (ObjectiveManager.Instance == null) return;

        // Using simple string comparison for safety
        if (itemID == ItemNames.GUN)
        {
            ObjectiveManager.Instance.UpdateObjective("Reach the Fire Tower");
        }
        else if (itemID == ItemNames.AXE)
        {
            ObjectiveManager.Instance.StartWoodObjective();
        }
        else if (itemID == ItemNames.LOGS)
        {
            for (int i = 0; i < amount; i++)
            {
                ObjectiveManager.Instance.AddWood();
            }
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