using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Settings")]
    public string itemID; 
    public int amount = 1;

    [Header("Debug View")]
    public bool isPlayerInRange = false;

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

        SimpleInventory inventory = SimpleInventory.Instance;

        if (inventory != null)
        {
            inventory.AddItem(itemID, amount);
            CheckStoryTriggers();
            wasCollected = true;
            
            if (SelectionManager.instance != null)
            {
                SelectionManager.instance.onTarget = false;
                SelectionManager.instance.interaction_Info_UI.SetActive(false);
            }

            Destroy(gameObject);
        }
    }

    private void CheckStoryTriggers()
    {
        if (ObjectiveManager.Instance == null) return;

        switch (itemID)
        {
            case "Gun":
                ObjectiveManager.Instance.UpdateObjective("Reach the Fire Tower");
                
                // --- NEW CODE TO START SPAWNER ---
                // This searches the scene for your Enemy spawner's script
                ZombieSpawner spawner = Object.FindFirstObjectByType<ZombieSpawner>();
                if (spawner != null)
                {
                    spawner.StartSpawning();
                }
                // ---------------------------------
                break;

            case "Axe":
                ObjectiveManager.Instance.UpdateObjective("Gather Wood (0/10)");
                break;

            case "Logs":
                for (int i = 0; i < amount; i++)
                {
                    ObjectiveManager.Instance.AddWood();
                }
                break;
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
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}