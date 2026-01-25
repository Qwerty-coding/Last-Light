using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Settings")]
    public string itemID;   
    public int amount = 1;

    [Header("Axe Visual Reference")]
    [Tooltip("Drag the Axe child object from your Player Camera here")]
    public GameObject axeInHand; // This represents the 'check box' you want ticked

    [Header("Debug View")]
    public bool isPlayerInRange = false;

    private bool wasCollected = false;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(itemID))
        {
            itemID = itemID.Trim(); 
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
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

        // ✅ ACTIVATE THE CHECKBOX
        // This line ticks the 'active' box for the axe model in your hand
        if (itemID == ItemNames.AXE && axeInHand != null)
        {
            axeInHand.SetActive(true);
        }

        SimpleInventory.Instance.AddItem(itemID, amount);
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