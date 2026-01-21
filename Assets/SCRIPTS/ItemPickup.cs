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
            Debug.Log("[ItemPickup] E pressed");

            if (SelectionManager.instance == null)
            {
                Debug.LogWarning("[ItemPickup] SelectionManager is NULL");
                return;
            }

            Debug.Log("[ItemPickup] Player in range: " + isPlayerInRange);
            Debug.Log("[ItemPickup] On target: " + SelectionManager.instance.onTarget);

            if (isPlayerInRange && SelectionManager.instance.onTarget)
            {
                if (SelectionManager.instance.interaction_Info_UI.activeSelf)
                {
                    Debug.Log("[ItemPickup] Conditions met → Collecting item");
                    CollectItem();
                }
                else
                {
                    Debug.LogWarning("[ItemPickup] Interaction UI not active");
                }
            }
        }
    }

    private void CollectItem()
    {
        if (wasCollected)
        {
            Debug.LogWarning("[ItemPickup] Item already collected");
            return;
        }

        if (SimpleInventory.Instance != null)
        {
            Debug.Log("[ItemPickup] Adding item to inventory: " + itemID);

            SimpleInventory.Instance.AddItem(itemID, amount);
            CheckStoryTriggers();

            wasCollected = true;

            if (SelectionManager.instance != null)
            {
                SelectionManager.instance.onTarget = false;
                SelectionManager.instance.interaction_Info_UI.SetActive(false);
            }

            Debug.Log("[ItemPickup] Destroying pickup object");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("[ItemPickup] SimpleInventory instance is NULL");
        }
    }

    private void CheckStoryTriggers()
    {
        if (ObjectiveManager.Instance == null) return;

        Debug.Log("[ItemPickup] Checking story trigger for: " + itemID);

        switch (itemID)
        {
            case "Gun":
                ObjectiveManager.Instance.UpdateObjective("Reach the Fire Tower");
                break;

            case "Axe":
                ObjectiveManager.Instance.UpdateObjective("Gather Wood (0/10)");
                break;

            case "Logs":
                for (int i = 0; i < amount; i++)
                    ObjectiveManager.Instance.AddWood();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("[ItemPickup] Player entered pickup range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("[ItemPickup] Player left pickup range");
        }
    }
}
