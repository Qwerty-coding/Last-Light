using UnityEngine;
using UnityEngine.UI;

public class SimpleInventoryUI : MonoBehaviour
{
    [Header("Weapon Icons")]
    public Image gunIcon;
    public Image axeIcon;
    public Image keyIcon;
    
    [Header("Resource UI")]
    public Image logIcon;
    public Text logsText;

    [Header("Settings")]
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.4f); // Faded
    public Color unlockedColor = Color.white;                     // Bright

    private void Start()
    {
        // Listen for inventory changes
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnInventoryChange.AddListener(RefreshUI);
            RefreshUI(); // Run once at start to set initial state
        }
    }

    public void RefreshUI()
    {
        // 1. Check Gun
        if(gunIcon != null) 
        {
            bool hasGun = SimpleInventory.Instance.HasItem("Gun");
            gunIcon.color = hasGun ? unlockedColor : lockedColor;
        }

        // 2. Check Axe
        if(axeIcon != null) 
        {
            bool hasAxe = SimpleInventory.Instance.HasItem("Axe");
            axeIcon.color = hasAxe ? unlockedColor : lockedColor;
        }

        // 3. Check Key
        if(keyIcon != null) 
        {
            bool hasKey = SimpleInventory.Instance.HasItem("Key");
            keyIcon.color = hasKey ? unlockedColor : lockedColor;
        }

        // 4. Check Logs
        int logCount = SimpleInventory.Instance.GetCount("Logs");
        
        // Determine color based on count
        Color targetColor = logCount > 0 ? unlockedColor : lockedColor;

        if (logIcon != null)
        {
            logIcon.color = targetColor;
        }

        if (logsText != null)
        {
            logsText.text = logCount.ToString();
            logsText.color = targetColor;
        }
    }
}