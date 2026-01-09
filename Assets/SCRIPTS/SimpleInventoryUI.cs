using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleInventoryUI : MonoBehaviour
{
    [Header("Weapon Icons")]
    public Image gunIcon;
    public Image axeIcon;
    public Image keyIcon;

    [Header("Resource UI")]
    public Image logIcon;
    public Text logsText;

    [Header("Colors")]
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    public Color unlockedColor = Color.white;

    [Header("Pop Animation")]
    public float popScale = 1.25f;
    public float popDuration = 0.12f;

    // Cached states
    private bool hadGun;
    private bool hadAxe;
    private bool hadKey;
    private int lastLogCount;

    private void Start()
    {
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnInventoryChange.AddListener(RefreshUI);
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        // ---------- GUN ----------
        bool hasGun = SimpleInventory.Instance.HasItem("Gun");
        UpdateIcon(gunIcon, hasGun, ref hadGun);

        // ---------- AXE ----------
        bool hasAxe = SimpleInventory.Instance.HasItem("Axe");
        UpdateIcon(axeIcon, hasAxe, ref hadAxe);

        // ---------- KEY ----------
        bool hasKey = SimpleInventory.Instance.HasItem("Key");
        UpdateIcon(keyIcon, hasKey, ref hadKey);

        // ---------- LOGS ----------
        int logCount = SimpleInventory.Instance.GetCount("Logs");
        UpdateLogs(logCount);
    }

    private void UpdateIcon(Image icon, bool hasItem, ref bool previousState)
    {
        if (icon == null)
            return;

        icon.color = hasItem ? unlockedColor : lockedColor;

        // Pop ONLY when item is newly acquired
        if (!previousState && hasItem)
        {
            StartCoroutine(Pop(icon.rectTransform));
        }

        previousState = hasItem;
    }

    private void UpdateLogs(int count)
    {
        bool hasLogs = count > 0;

        if (logIcon != null)
            logIcon.color = hasLogs ? unlockedColor : lockedColor;

        if (logsText == null)
            return;

        // Hide text if zero
        if (!hasLogs)
        {
            logsText.text = "";
            lastLogCount = 0;
            return;
        }

        // Update number
        logsText.text = count.ToString();
        logsText.color = unlockedColor;

        // Pop EVERY time log count increases
        if (count > lastLogCount)
        {
            StartCoroutine(Pop(logsText.rectTransform));
            StartCoroutine(Pop(logIcon.rectTransform));
        }

        lastLogCount = count;
    }

    private IEnumerator Pop(RectTransform target)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * popScale;

        float t = 0f;

        // Scale up
        while (t < popDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(originalScale, targetScale, t / popDuration);
            yield return null;
        }

        t = 0f;

        // Scale back
        while (t < popDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(targetScale, originalScale, t / popDuration);
            yield return null;
        }

        target.localScale = originalScale;
    }
}
