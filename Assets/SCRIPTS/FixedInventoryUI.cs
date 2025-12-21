using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class FixedInventoryUI : MonoBehaviour
{
    // --- NEW: Add this slot so you can drag a font in ---
    [Header("Settings")]
    public Font defaultFont; 
    // ----------------------------------------------------

    [Header("Text UI (Optional)")]
    public Text gunText;
    public Text axeText;
    public Text logsText;

    // ... (Keep your TMP variables here) ...
    [Header("TextMeshPro UI (Optional)")]
    public TMP_Text gunTMP;
    public TMP_Text axeTMP;
    public TMP_Text logsTMP;

    [Header("Auto Create")]
    public bool autoCreateDefaultUI = true;

    private void Start()
    {
        if (FixedInventory.Instance == null)
        {
            Debug.LogWarning("FixedInventory instance not found; creating one.");
        }

        if (autoCreateDefaultUI && !HasAnyUIAssigned())
        {
            CreateDefaultHUD();
        }

        // Subscribe to events
        var inv = FixedInventory.Instance;
        inv.OnGunChanged.AddListener(UpdateUI);
        inv.OnAxeChanged.AddListener(UpdateUI);
        inv.OnInventoryChanged.AddListener(UpdateUI);
        inv.OnLogsChanged.AddListener(OnLogsChanged);

        UpdateUI();
    }

    // ... (Keep HasAnyUIAssigned, OnDestroy, OnLogsChanged, UpdateUI exactly as they were) ...
    private bool HasAnyUIAssigned()
    {
        return gunText || axeText || logsText || gunTMP || axeTMP || logsTMP;
    }

    private void OnDestroy()
    {
        if (FixedInventory.Instance != null)
        {
            FixedInventory.Instance.OnGunChanged.RemoveListener(UpdateUI);
            FixedInventory.Instance.OnAxeChanged.RemoveListener(UpdateUI);
            FixedInventory.Instance.OnInventoryChanged.RemoveListener(UpdateUI);
            FixedInventory.Instance.OnLogsChanged.RemoveListener(OnLogsChanged);
        }
    }

    private void OnLogsChanged(int count)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        var inv = FixedInventory.Instance;
        if (inv == null) return;

        string gun = inv.HasGun ? "Owned" : "---";
        string axe = inv.HasAxe ? "Owned" : "---";
        string logs = inv.LogsCount.ToString();

        if (gunText) gunText.text = $"Gun: {gun}";
        if (axeText) axeText.text = $"Axe: {axe}";
        if (logsText) logsText.text = $"Logs: {logs}";

        if (gunTMP) gunTMP.text = $"Gun: {gun}";
        if (axeTMP) axeTMP.text = $"Axe: {axe}";
        if (logsTMP) logsTMP.text = $"Logs: {logs}";
    }

    private void CreateDefaultHUD()
    {
        // Check if font is missing
        if (defaultFont == null)
        {
            Debug.LogError("NO FONT ASSIGNED! Drag a font into the 'Default Font' slot on the FixedInventoryUI script.");
            return;
        }

        var canvasGO = new GameObject("FixedInventoryHUD_Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasGO);

        var panelGO = new GameObject("HUD_Panel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        var image = panelGO.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.35f);

        var rect = panelGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = new Vector2(180f, 80f);
        rect.anchoredPosition = new Vector2(10f, -10f);

        // USE THE PUBLIC FONT VARIABLE HERE instead of Resources.GetBuiltinResource
        gunText = CreateHUDText("GunText", panelGO.transform, new Vector2(8f, -12f), defaultFont);
        axeText = CreateHUDText("AxeText", panelGO.transform, new Vector2(8f, -32f), defaultFont);
        logsText = CreateHUDText("LogsText", panelGO.transform, new Vector2(8f, -52f), defaultFont);
    }

    private Text CreateHUDText(string name, Transform parent, Vector2 anchoredPos, Font font)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<Text>();
        t.font = font; // Assign the font passed in
        t.fontSize = 14;
        t.color = Color.white;
        t.alignment = TextAnchor.UpperLeft;

        var r = go.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0f, 1f);
        r.anchorMax = new Vector2(0f, 1f);
        r.pivot = new Vector2(0f, 1f);
        r.anchoredPosition = anchoredPos;
        r.sizeDelta = new Vector2(160f, 18f);

    

        return t;
    }
}