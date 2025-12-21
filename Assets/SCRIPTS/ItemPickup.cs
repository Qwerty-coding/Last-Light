using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("What is this item?")]
    public bool isGun;
    public bool isAxe;
    public int woodAmount = 0;

    // Internal flag to track if player is close enough
    private bool isPlayerInRange = false;

    private void Update()
    {
        // 1. Check if Player is nearby AND presses 'E'
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        // 2. Access the Inventory
        FixedInventory inventory = FixedInventory.Instance;

        if (inventory != null)
        {
            bool itemPickedUp = false;

            if (isGun)
            {
                inventory.GiveGun(); //
                Debug.Log("Picked up Gun via E!");
                itemPickedUp = true;
            }
            else if (isAxe)
            {
                inventory.GiveAxe(); //
                Debug.Log("Picked up Axe via E!");
                itemPickedUp = true;
            }
            else if (woodAmount > 0)
            {
                inventory.AddLogs(woodAmount); //
                Debug.Log("Picked up Wood via E!");
                itemPickedUp = true;
            }

            // 3. Destroy object only after successful pickup
            if (itemPickedUp)
            {
                Destroy(gameObject);
            }
        }
    }

    // 4. Detect when player walks INTO the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Press E to pickup"); // Optional debug message
        }
    }

    // 5. Detect when player walks OUT of the trigger zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}