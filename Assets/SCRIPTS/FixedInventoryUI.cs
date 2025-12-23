using UnityEngine;
using UnityEngine.UI;

public class FixedInventoryUI : MonoBehaviour
{
    [Header("Weapon Icons")]
    public Image gunIcon;
    public Image axeIcon;

    [Header("Resource UI")]
    public Image logIcon;       // NEW: Slot for the Log Picture
    public Text logsCountText;  // Slot for the "0" Text

    [Header("Visual Settings")]
    // Grey and transparent when empty/0
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.4f); 
    // White and fully visible when owned/ >0
    public Color unlockedColor = Color.white; 

    private void Start()
    {
        if (FixedInventory.Instance != null)
        {
            FixedInventory.Instance.OnGunChanged.AddListener(UpdateGunUI);
            FixedInventory.Instance.OnAxeChanged.AddListener(UpdateAxeUI);
            FixedInventory.Instance.OnLogsChanged.AddListener(UpdateLogsUI);

            RefreshAll();
        }
    }

    public void RefreshAll()
    {
        UpdateGunUI();
        UpdateAxeUI();
        // Force update with current count
        UpdateLogsUI(FixedInventory.Instance.LogsCount); 
    }

    private void UpdateGunUI()
    {
        if (gunIcon != null)
        {
            // Logic: If true -> White. If false -> Grey.
            gunIcon.color = FixedInventory.Instance.HasGun ? unlockedColor : lockedColor;
        }
    }

    private void UpdateAxeUI()
    {
        if (axeIcon != null)
        {
            axeIcon.color = FixedInventory.Instance.HasAxe ? unlockedColor : lockedColor;
        }
    }

    private void UpdateLogsUI(int count)
    {
        // 1. Determine the color based on count
        // If count is greater than 0, use White. Else use Grey.
        Color targetColor = (count > 0) ? unlockedColor : lockedColor;

        // 2. Apply color to the Text (the number)
        if (logsCountText != null)
        {
            logsCountText.text = count.ToString();
            logsCountText.color = targetColor;
        }

        // 3. Apply color to the Icon (the log picture)
        if (logIcon != null)
        {
            logIcon.color = targetColor;
        }
    }
}