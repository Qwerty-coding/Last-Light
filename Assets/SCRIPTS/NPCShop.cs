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