using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("UI Info")]
    public string itemName; // Name to show on screen (e.g. "Rusty Axe")

    [Header("Inventory Settings")]
    public bool isGun;
    public bool isAxe;
    public int woodAmount = 0;

    // Public so SelectionManager can read it
    [HideInInspector]
    public bool isPlayerInRange = false;

    private void Update()
    {
        // LOGIC: Press E + Player is Close + Looking at Object (SelectionManager)
        // We use 'SelectionManager.instance.onTarget' to ensure we are actually looking at THIS object
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange && SelectionManager.instance.onTarget)
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        FixedInventory inventory = FixedInventory.Instance;

        if (inventory != null)
        {
            bool itemPickedUp = false;

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

            if (itemPickedUp)
            {
                // Clear the UI text immediately so it doesn't get stuck
                SelectionManager.instance.onTarget = false;
                SelectionManager.instance.interaction_Info_UI.SetActive(false);
                
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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