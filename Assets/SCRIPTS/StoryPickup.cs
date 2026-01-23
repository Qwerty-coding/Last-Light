using UnityEngine;

public class StoryPickup : MonoBehaviour
{
    [Header("Settings")]
    public string itemID = "Axe";
    public int amount = 1;

    [Header("Detection")]
    public bool isPlayerInRange;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Collect();
        }
    }

    private void Collect()
    {
        Debug.Log($"[StoryPickup] Player picked up: {itemID}");

        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.AddItem(itemID, amount);
        }

        if (ObjectiveManager.Instance != null)
        {
            HandleObjective();
        }
        else
        {
            Debug.LogError("[StoryPickup] ObjectiveManager.Instance is NULL!");
        }

        gameObject.SetActive(false);
    }

    private void HandleObjective()
    {
        switch (itemID)
        {
            case "Axe":
                // Axe unlocks the wood objective
                ObjectiveManager.Instance.StartWoodObjective();
                break;

            case "Logs":
                // Each log adds +1 wood
                ObjectiveManager.Instance.AddWood();
                break;

            case "Gun":
                ObjectiveManager.Instance.UpdateObjective("Kill 3 Zombies to get the Key");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
    }
}
