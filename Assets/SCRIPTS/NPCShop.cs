using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCShop : MonoBehaviour
{
    [Header("Main UI")]
    public GameObject tradeMenuUI;

    [Header("Buttons")]
    public Button tradeButton;
    public Button closeButton;

    [Header("Pop-up Messages")]
    public GameObject notEnoughLogsUI;
    public GameObject successUI;

    [Header("Trade Settings")]
    public string itemCostName = "Logs";
    public int costAmount = 5;
    public string itemRewardName = "Gun";

    [Header("References")]
    public WeaponHandler weaponHandler;

    private bool hasAlreadyTraded = false;
    private MouseMovement playerMouseScript;
    private PlayerMovement playerMoveScript;
    private Coroutine currentMessageRoutine;

    // NEW: Cache the spawner so we only call FindObjectOfType once
    private ZombieSpawner zombieSpawner;

    void Start()
    {
        tradeButton.onClick.AddListener(TryTrade);
        closeButton.onClick.AddListener(CloseShop);

        playerMouseScript = FindObjectOfType<MouseMovement>();
        playerMoveScript = FindObjectOfType<PlayerMovement>();

        // Find spawner once at start
        zombieSpawner = FindObjectOfType<ZombieSpawner>();

        if (weaponHandler == null)
        {
            weaponHandler = FindObjectOfType<WeaponHandler>();
            if (weaponHandler == null)
                Debug.LogError("WeaponHandler not found! Please assign it in Inspector.");
        }

        if (notEnoughLogsUI != null) notEnoughLogsUI.SetActive(false);
        if (successUI != null) successUI.SetActive(false);
    }

    public void OpenShop()
    {
        tradeMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerMouseScript != null) playerMouseScript.enabled = false;
        if (playerMoveScript != null) playerMoveScript.enabled = false;
    }

    public void TryTrade()
    {
        if (currentMessageRoutine != null) StopCoroutine(currentMessageRoutine);

        // Already traded - just show success message again
        if (hasAlreadyTraded)
        {
            currentMessageRoutine = StartCoroutine(AnimateMessage(successUI));
            return;
        }

        if (SimpleInventory.Instance.GetCount(itemCostName) >= costAmount)
        {
            // Successful trade
            SimpleInventory.Instance.RemoveItem(itemCostName, costAmount);
            SimpleInventory.Instance.AddItem(itemRewardName, 1);

            hasAlreadyTraded = true;

            // Equip the gun
            if (weaponHandler != null)
            {
                weaponHandler.ForceEquipGun();
                Debug.Log("Gun equipped after trade!");
            }
            else
            {
                Debug.LogWarning("WeaponHandler not found - gun added to inventory but not equipped");
            }

            // NEW: Start zombie spawning now that gun is equipped
            if (zombieSpawner != null)
            {
                zombieSpawner.StartSpawning();
                Debug.Log("[NPCShop] Zombie spawner activated after gun trade.");
            }
            else
            {
                Debug.LogWarning("[NPCShop] ZombieSpawner not found in scene!");
            }

            // Update objective
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.UpdateObjective(
                    "Kill " + ObjectiveManager.Instance.zombiesRequired + " Zombies to find the Key"
                );
            }

            currentMessageRoutine = StartCoroutine(AnimateMessage(successUI));
        }
        else
        {
            currentMessageRoutine = StartCoroutine(AnimateMessage(notEnoughLogsUI));
        }
    }

    IEnumerator AnimateMessage(GameObject messageObj)
    {
        messageObj.SetActive(true);
        yield return new WaitForSeconds(3f);
        messageObj.SetActive(false);
    }

    public void CloseShop()
    {
        tradeMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMouseScript != null) playerMouseScript.enabled = true;
        if (playerMoveScript != null) playerMoveScript.enabled = true;
    }
}