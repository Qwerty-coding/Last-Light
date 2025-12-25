using UnityEngine;
using UnityEngine.UI;

public class FixedInventoryUI : MonoBehaviour
{
    [Header("Weapon Icons")]
    public Image gunIcon;
    public Image axeIcon;
    public Image keyIcon; // NEW

    [Header("Resource UI")]
    public Image logIcon;      
    public Text logsCountText; 

    [Header("Visual Settings")]
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.4f); 
    public Color unlockedColor = Color.white; 

    private void Start()
    {
        if (FixedInventory.Instance != null)
        {
            FixedInventory.Instance.OnGunChanged.AddListener(UpdateGunUI);
            FixedInventory.Instance.OnAxeChanged.AddListener(UpdateAxeUI);
            FixedInventory.Instance.OnKeyChanged.AddListener(UpdateKeyUI); // NEW
            FixedInventory.Instance.OnLogsChanged.AddListener(UpdateLogsUI);

            RefreshAll();
        }
    }

    private void OnDestroy()
    {
        if (FixedInventory.Instance != null)
        {
            FixedInventory.Instance.OnGunChanged.RemoveListener(UpdateGunUI);
            FixedInventory.Instance.OnAxeChanged.RemoveListener(UpdateAxeUI);
            FixedInventory.Instance.OnKeyChanged.RemoveListener(UpdateKeyUI); // NEW
            FixedInventory.Instance.OnLogsChanged.RemoveListener(UpdateLogsUI);
        }
    }

    public void RefreshAll()
    {
        UpdateGunUI();
        UpdateAxeUI();
        UpdateKeyUI(); // NEW
        UpdateLogsUI(FixedInventory.Instance.LogsCount); 
    }

    private void UpdateGunUI()
    {
        if (gunIcon != null)
            gunIcon.color = FixedInventory.Instance.HasGun ? unlockedColor : lockedColor;
    }

    private void UpdateAxeUI()
    {
        if (axeIcon != null)
            axeIcon.color = FixedInventory.Instance.HasAxe ? unlockedColor : lockedColor;
    }

    private void UpdateKeyUI() // NEW
    {
        if (keyIcon != null)
            keyIcon.color = FixedInventory.Instance.HasKey ? unlockedColor : lockedColor;
    }

    private void UpdateLogsUI(int count)
    {
        Color targetColor = (count > 0) ? unlockedColor : lockedColor;

        if (logsCountText != null)
        {
            logsCountText.text = count.ToString();
            logsCountText.color = targetColor;
        }

        if (logIcon != null)
        {
            logIcon.color = targetColor;
        }
    }
}