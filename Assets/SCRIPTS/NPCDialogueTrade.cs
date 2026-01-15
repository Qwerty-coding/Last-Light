using UnityEngine;
using TMPro;
using System.Collections;

public class NPCDialogueTrade : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject exchangeButton;

    [Header("Animation Settings")]
    public float fadeDuration = 0.3f;
    public float dropDistance = 150f;

    [Header("Player Control")]
    public MouseMovement mouseMovement;

    private bool playerInRange;
    private bool dialogueOpen;
    private bool hasTraded = false;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
        rectTransform = dialoguePanel.GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        dialoguePanel.SetActive(false);
        exchangeButton.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E) && !dialogueOpen)
        {
            OpenDialogue();
        }
    }

    private void OpenDialogue()
    {
        dialogueOpen = true;

        dialoguePanel.SetActive(true);
        exchangeButton.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(FadeDropIn());

        LockPlayer();
    }

    private IEnumerator FadeDropIn()
    {
        float elapsed = 0f;

        Vector2 startPos = originalPosition + Vector2.up * dropDistance;
        rectTransform.anchoredPosition = startPos;
        canvasGroup.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            rectTransform.anchoredPosition =
                Vector2.Lerp(startPos, originalPosition, t);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalPosition;
    }

    // 🔘 Exchange Button
    public void ExecuteTrade()
    {
        if (hasTraded)
        {
            dialogueText.text = "You have already traded.";
            return;
        }

        // ❗ Inventory checks using YOUR API
        if (!SimpleInventory.Instance.HasItem("Logs"))
        {
            dialogueText.text = "You need 5 logs to trade.";
            return;
        }

        int logCount = SimpleInventory.Instance.GetCount("Logs");

        if (logCount < 5)
        {
            dialogueText.text = "You need 5 logs to trade.";
            return;
        }

        // ✅ TRADE
        SimpleInventory.Instance.RemoveItem("Logs", 5);
        SimpleInventory.Instance.AddItem("Gun", 1);

        hasTraded = true;
        dialogueText.text = "Trade successful! You received a gun.";
    }

    public void CloseDialogueButton()
    {
        CloseDialogue();
    }

    private void CloseDialogue()
    {
        dialogueOpen = false;

        dialoguePanel.SetActive(false);
        exchangeButton.SetActive(false);

        UnlockPlayer();
    }

    private void LockPlayer()
    {
        if (mouseMovement != null)
            mouseMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UnlockPlayer()
    {
        if (mouseMovement != null)
            mouseMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        CloseDialogue();
    }
}
