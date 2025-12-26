using UnityEngine;
using UnityEngine.UI; // Required for Image and Text

public class SimpleInventoryUI : MonoBehaviour
{
    [Header("Weapon Icons")]
    public Image gunIcon;
    public Image axeIcon;
    public Image keyIcon;
    
    [Header("Resource UI")]
    public Image logIcon;  // <--- NEW: Slot for the Log Image
    public Text logsText;  // <--- Slot for the Counter (e.g., "0")

    [Header("Settings")]
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // Dark
    public Color unlockedColor = Color.white; // Bright

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
        // 1. Check Weapons
        if(gunIcon != null) 
            gunIcon.color = SimpleInventory.Instance.HasItem("Gun") ? unlockedColor : lockedColor;

        if(axeIcon != null) 
            axeIcon.color = SimpleInventory.Instance.HasItem("Axe") ? unlockedColor : lockedColor;

        if(keyIcon != null) 
            keyIcon.color = SimpleInventory.Instance.HasItem("Key") ? unlockedColor : lockedColor;

        // 2. Check Logs (Icon AND Text)
        int count = SimpleInventory.Instance.GetCount("Logs");

        // Logic: If we have logs, turn icon white. If 0, turn dark.
        if (logIcon != null)
        {
            logIcon.color = count > 0 ? unlockedColor : lockedColor;
        }

        // Logic: Update the number text
        if (logsText != null)
        {
            logsText.text = count.ToString(); // Just shows the number "0", "1", "5"
        }
    }
}