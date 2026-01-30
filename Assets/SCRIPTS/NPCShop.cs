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
    public WeaponHandler weaponHandler; // NEW - Assign in Inspector
    
    private bool hasAlreadyTraded = false;
    private MouseMovement playerMouseScript;
    private PlayerMovement playerMoveScript; 
    private Coroutine currentMessageRoutine;
    
    void Start()
    {
        tradeButton.onClick.AddListener(TryTrade);
        closeButton.onClick.AddListener(CloseShop);
        
        playerMouseScript = FindObjectOfType<MouseMovement>();
        playerMoveScript = FindObjectOfType<PlayerMovement>();
        
        // NEW - Auto-find WeaponHandler if not assigned
        if (weaponHandler == null)
        {
            weaponHandler = FindObjectOfType<WeaponHandler>();
            if (weaponHandler == null)
            {
                Debug.LogError("WeaponHandler not found! Please assign it in Inspector.");
            }
        }
        
        if(notEnoughLogsUI != null) notEnoughLogsUI.SetActive(false);
        if(successUI != null) successUI.SetActive(false);
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
        
        if (hasAlreadyTraded)
        {
            currentMessageRoutine = StartCoroutine(AnimateMessage(successUI));
            return;
        }
        
        if (SimpleInventory.Instance.GetCount(itemCostName) >= costAmount)
        {
            // Successful Trade
            SimpleInventory.Instance.RemoveItem(itemCostName, costAmount);
            SimpleInventory.Instance.AddItem(itemRewardName, 1);
            
            hasAlreadyTraded = true;
            
            // NEW - Equip the gun after giving it
            if (weaponHandler != null)
            {
                // Hide current weapon and equip the gun
                weaponHandler.ForceEquipGun();
                Debug.Log("Gun equipped after trade!");
            }
            else
            {
                Debug.LogWarning("WeaponHandler not found - gun added to inventory but not equipped");
            }
            
            // Tell ObjectiveManager to start the Zombie Hunt phase
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.UpdateObjective("Kill 3 Zombies to get the Key");
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
        CanvasGroup group = messageObj.GetComponent<CanvasGroup>();
        
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