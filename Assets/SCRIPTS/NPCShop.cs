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

    // --- REFERENCES TO PLAYER SCRIPTS ---
    private MouseMovement playerMouseScript;
    private PlayerMovement playerMoveScript; // <--- NEW: Reference to movement

    private Coroutine currentMessageRoutine;

    void Start()
    {
        tradeButton.onClick.AddListener(TryTrade);
        closeButton.onClick.AddListener(CloseShop);
        
        // Find both scripts automatically
        playerMouseScript = FindObjectOfType<MouseMovement>();
        playerMoveScript = FindObjectOfType<PlayerMovement>(); // <--- NEW: Find the script
        
        if(notEnoughLogsUI != null) notEnoughLogsUI.SetActive(false);
        if(successUI != null) successUI.SetActive(false);
    }

    public void OpenShop()
    {
        tradeMenuUI.SetActive(true);
        
        // 1. UNLOCK CURSOR
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. STOP CAMERA ROTATION
        if (playerMouseScript != null) playerMouseScript.enabled = false;

        // 3. STOP WASD MOVEMENT (Fixes your bug)
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
            // SUCCESS
            SimpleInventory.Instance.RemoveItem(itemCostName, costAmount);
            SimpleInventory.Instance.AddItem(itemRewardName, 1);
            
            hasAlreadyTraded = true; 
            currentMessageRoutine = StartCoroutine(AnimateMessage(successUI));
        }
        else
        {
            // FAIL
            currentMessageRoutine = StartCoroutine(AnimateMessage(notEnoughLogsUI));
        }
    }

    IEnumerator AnimateMessage(GameObject messageObj)
    {
        messageObj.SetActive(true); 
        CanvasGroup group = messageObj.GetComponent<CanvasGroup>();
        RectTransform rect = messageObj.GetComponent<RectTransform>();

        Vector2 finalPosition = rect.anchoredPosition;
        Vector2 startPosition = finalPosition + new Vector2(-100f, 0);

        float elapsedTime = 0f;
        float animationDuration = 0.5f; 

        // Animation Loop
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / animationDuration; 

            rect.anchoredPosition = Vector2.Lerp(startPosition, finalPosition, percentage);
            if(group != null) group.alpha = Mathf.Lerp(0f, 1f, percentage);

            yield return null; 
        }

        rect.anchoredPosition = finalPosition;
        if(group != null) group.alpha = 1f;

        yield return new WaitForSeconds(5f); // 5 Second Timer

        messageObj.SetActive(false);
    }

    public void CloseShop()
    {
        tradeMenuUI.SetActive(false);
        
        if(successUI != null) successUI.SetActive(false);
        if(notEnoughLogsUI != null) notEnoughLogsUI.SetActive(false);

        // 1. LOCK CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 2. RESUME CAMERA ROTATION
        if (playerMouseScript != null) playerMouseScript.enabled = true;

        // 3. RESUME WASD MOVEMENT (Fixes your bug)
        if (playerMoveScript != null) playerMoveScript.enabled = true;
    }
}