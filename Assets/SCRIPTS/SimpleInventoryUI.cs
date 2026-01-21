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
        if (icon == null) return;

        // 1. SET ALPHA: 1 if we have it, 0 if we don't
        Color c = icon.color;
        c.a = hasItem ? 1f : 0f;
        icon.color = c;

        // 2. POP ANIMATION
        if (!previousState && hasItem)
        {
            icon.rectTransform.localScale = Vector3.one; 
            StartCoroutine(Pop(icon.rectTransform));
        }

        previousState = hasItem;
    }

    private void UpdateLogs(int count)
    {
        bool hasLogs = count > 0;

        // 1. LOG ICON ALPHA
        if (logIcon != null)
        {
            Color c = logIcon.color;
            c.a = hasLogs ? 1f : 0f;
            logIcon.color = c;
        }

        if (logsText == null) return;

        // 2. TEXT ALPHA & VALUE
        if (!hasLogs)
        {
            // Just make it invisible, no need to clear text string
            Color tColor = logsText.color;
            tColor.a = 0f;
            logsText.color = tColor;
            
            lastLogCount = 0;
            return;
        }

        // Show text
        logsText.text = count.ToString();
        Color visibleColor = logsText.color;
        visibleColor.a = 1f;
        logsText.color = visibleColor;

        // 3. POP ANIMATION
        if (count > lastLogCount)
        {
            StartCoroutine(Pop(logsText.rectTransform));
            if (logIcon != null) StartCoroutine(Pop(logIcon.rectTransform));
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