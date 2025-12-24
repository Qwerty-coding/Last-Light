using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IntEvent : UnityEvent<int> { }

public class FixedInventory : MonoBehaviour
{
    private static FixedInventory _instance;
    public static FixedInventory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FixedInventory>();
                if (_instance == null)
                {
                    var go = new GameObject("FixedInventory");
                    _instance = go.AddComponent<FixedInventory>();
                }
            }
            return _instance;
        }
    }

    [Header("Items")]
    [SerializeField] private bool hasGun = false;
    [SerializeField] private bool hasAxe = false;
    [SerializeField] private int logsCount = 0;

    [Header("Events")]
    public UnityEvent OnInventoryChanged = new UnityEvent();
    public UnityEvent OnGunChanged = new UnityEvent();
    public UnityEvent OnAxeChanged = new UnityEvent();
    public IntEvent OnLogsChanged = new IntEvent();

    private const string PrefKey_Gun = "FixedInv_hasGun";
    private const string PrefKey_Axe = "FixedInv_hasAxe";
    private const string PrefKey_Logs = "FixedInv_logs";

   private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // --- ADD THIS LINE BELOW ---
            // This deletes all saved data immediately when the game starts
            ResetInventory(); 
            // ---------------------------

            // You can remove or comment out 'Load();' since we just reset it
            // Load(); 
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    #region Public API

    public bool HasGun => hasGun;
    public bool HasAxe => hasAxe;
    public int LogsCount => logsCount;

    public void GiveGun()
    {
        if (!hasGun)
        {
            hasGun = true;
            OnGunChanged?.Invoke();
            OnInventoryChanged?.Invoke();
            Save();
        }
    }

    public void RemoveGun()
    {
        if (hasGun)
        {
            hasGun = false;
            OnGunChanged?.Invoke();
            OnInventoryChanged?.Invoke();
            Save();
        }
    }

    public void GiveAxe()
    {
        if (!hasAxe)
        {
            hasAxe = true;
            OnAxeChanged?.Invoke();
            OnInventoryChanged?.Invoke();
            Save();
        }
    }

    public void RemoveAxe()
    {
        if (hasAxe)
        {
            hasAxe = false;
            OnAxeChanged?.Invoke();
            OnInventoryChanged?.Invoke();
            Save();
        }
    }

    public void AddLogs(int amount)
    {
        if (amount <= 0) return;
        logsCount += amount;
        OnLogsChanged?.Invoke(logsCount);
        OnInventoryChanged?.Invoke();
        Save();
    }

    public bool RemoveLogs(int amount)
    {
        if (amount <= 0) return false;
        if (logsCount < amount) return false;
        logsCount -= amount;
        OnLogsChanged?.Invoke(logsCount);
        OnInventoryChanged?.Invoke();
        Save();
        return true;
    }

    public void SetLogs(int amount)
    {
        logsCount = Math.Max(0, amount);
        OnLogsChanged?.Invoke(logsCount);
        OnInventoryChanged?.Invoke();
        Save();
    }

    

    public void ClearInventory()
    {
        hasGun = false;
        hasAxe = false;
        logsCount = 0;
        OnGunChanged?.Invoke();
        OnAxeChanged?.Invoke();
        OnLogsChanged?.Invoke(logsCount);
        OnInventoryChanged?.Invoke();
        Save();
    }

    #endregion

    #region Persistence

    public void Save()
    {
        PlayerPrefs.SetInt(PrefKey_Gun, hasGun ? 1 : 0);
        PlayerPrefs.SetInt(PrefKey_Axe, hasAxe ? 1 : 0);
        PlayerPrefs.SetInt(PrefKey_Logs, logsCount);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        hasGun = PlayerPrefs.GetInt(PrefKey_Gun, hasGun ? 1 : 0) == 1;
        hasAxe = PlayerPrefs.GetInt(PrefKey_Axe, hasAxe ? 1 : 0) == 1;
        logsCount = PlayerPrefs.GetInt(PrefKey_Logs, logsCount);
    }

    #endregion

    #region Debug helpers

    [ContextMenu("Reset Inventory")]
    public void ResetInventory()
    {
        PlayerPrefs.DeleteKey(PrefKey_Gun);
        PlayerPrefs.DeleteKey(PrefKey_Axe);
        PlayerPrefs.DeleteKey(PrefKey_Logs);
        ClearInventory();
    }

    // --- NEW ADDITION BELOW THIS LINE ---
    
#if UNITY_EDITOR
    // This runs automatically whenever you change a value in the Inspector
    private void OnValidate()
    {
        // We only want to fire events if the game is actually running
        if (Application.isPlaying)
        {
            OnGunChanged?.Invoke();
            OnAxeChanged?.Invoke();
            OnLogsChanged?.Invoke(logsCount);
            OnInventoryChanged?.Invoke();
        }
    }
#endif

    #endregion
}