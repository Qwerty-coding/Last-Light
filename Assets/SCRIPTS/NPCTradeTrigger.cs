using UnityEngine;

public class NPCTradeTrigger : MonoBehaviour
{
    [Header("Trade Settings")]
    public int logsRequired = 5;

    private bool playerInRange = false;
    private bool tradeCompleted = false;

    private void Start()
    {
        Debug.Log("🟢 NPCTradeTrigger active on " + gameObject.name);
    }

    private void Update()
    {
        if (!playerInRange || tradeCompleted)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("🟡 E pressed near NPC");
            TryTrade();
        }
    }

    private void TryTrade()
    {
        if (SimpleInventory.Instance == null)
        {
            Debug.LogError("❌ SimpleInventory.Instance is NULL");
            return;
        }

        int logs = SimpleInventory.Instance.GetCount("Logs");
        Debug.Log("📦 Logs in inventory: " + logs);

        if (logs >= logsRequired)
        {
            SimpleInventory.Instance.RemoveItem("Logs", logsRequired);
            SimpleInventory.Instance.AddItem("Gun", 1);

            tradeCompleted = true;

            Debug.Log("🎉 NPC TRADE SUCCESSFUL");
        }
        else
        {
            Debug.LogWarning("❌ Not enough Logs to trade");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("🟢 Player entered NPC range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("🔴 Player left NPC range");
        }
    }
}
