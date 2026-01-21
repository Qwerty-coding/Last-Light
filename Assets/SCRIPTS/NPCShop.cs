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

    // We store the actively running animation so we can stop it if the player clicks fast
    private Coroutine currentMessageRoutine;

    void Start()
    {
        tradeButton.onClick.AddListener(TryTrade);
        closeButton.onClick.AddListener(CloseShop);
        
        playerMouseScript = FindObjectOfType<MouseMovement>();
        
        if(notEnoughLogsUI != null) notEnoughLogsUI.SetActive(false);
        if(successUI != null) successUI.SetActive(false);
    }

    public void OpenShop()
    {
        tradeMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (playerMouseScript != null) playerMouseScript.enabled = false;
    }

    public void TryTrade()
    {
        // Stop any message currently on screen so they don't overlap
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

    // --- THE ANIMATION LOGIC ---
    IEnumerator AnimateMessage(GameObject messageObj)
    {
        // 1. Setup
        messageObj.SetActive(true); // Turn it on
        CanvasGroup group = messageObj.GetComponent<CanvasGroup>();
        RectTransform rect = messageObj.GetComponent<RectTransform>();

        // Remember where the button is supposed to be (The center)
        Vector2 finalPosition = rect.anchoredPosition;
        
        // Move it 100 pixels to the LEFT for the start
        Vector2 startPosition = finalPosition + new Vector2(-100f, 0);

        float elapsedTime = 0f;
        float animationDuration = 0.5f; // Animation takes 0.5 seconds

        // 2. The Animation Loop (Fade In + Slide Right)
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / animationDuration; // 0 to 1

            // Smoothly move from Start to Final
            rect.anchoredPosition = Vector2.Lerp(startPosition, finalPosition, percentage);
            
            // Smoothly fade Alpha from 0 to 1
            if(group != null) group.alpha = Mathf.Lerp(0f, 1f, percentage);

            yield return null; // Wait for next frame
        }

        // Ensure it ends exactly at the right spot/opacity
        rect.anchoredPosition = finalPosition;
        if(group != null) group.alpha = 1f;

        // 3. Wait for 5 Seconds (as you requested)
        yield return new WaitForSeconds(5f);

        // 4. Turn it off
        messageObj.SetActive(false);
    }

    public void CloseShop()
    {
        tradeMenuUI.SetActive(false);
        
        if(successUI != null) successUI.SetActive(false);
        if(notEnoughLogsUI != null) notEnoughLogsUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerMouseScript != null) playerMouseScript.enabled = true;
    }
}