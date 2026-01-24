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

    private bool hadGun;
    private bool hadAxe;
    private bool hadKey;
    private int lastLogCount;

    private void Start()
    {
        SimpleInventory.Instance.OnInventoryChange.AddListener(RefreshUI);
        RefreshUI();
    }

    public void RefreshUI()
    {
        UpdateIcon(gunIcon, SimpleInventory.Instance.HasItem(ItemNames.GUN), ref hadGun);
        UpdateIcon(axeIcon, SimpleInventory.Instance.HasItem(ItemNames.AXE), ref hadAxe);
        UpdateIcon(keyIcon, SimpleInventory.Instance.HasItem(ItemNames.KEY), ref hadKey);

        UpdateLogs(SimpleInventory.Instance.GetCount(ItemNames.LOGS));
    }

    private void UpdateIcon(Image icon, bool hasItem, ref bool previousState)
    {
        if (icon == null) return;

        Color c = icon.color;
        c.a = hasItem ? 1f : 0f;
        icon.color = c;

        if (!previousState && hasItem)
            StartCoroutine(Pop(icon.rectTransform));

        previousState = hasItem;
    }

    private void UpdateLogs(int count)
    {
        if (count <= 0)
        {
            logIcon.color = new Color(1,1,1,0);
            logsText.color = new Color(1,1,1,0);
            lastLogCount = 0;
            return;
        }

        logIcon.color = new Color(1,1,1,1);
        logsText.color = new Color(1,1,1,1);
        logsText.text = count.ToString();

        if (count > lastLogCount)
        {
            StartCoroutine(Pop(logIcon.rectTransform));
            StartCoroutine(Pop(logsText.rectTransform));
        }

        lastLogCount = count;
    }

    private IEnumerator Pop(RectTransform target)
    {
        Vector3 a = Vector3.one;
        Vector3 b = Vector3.one * popScale;

        float t = 0;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(a, b, t / popDuration);
            yield return null;
        }

        t = 0;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(b, a, t / popDuration);
            yield return null;
        }

        target.localScale = a;
    }
}
