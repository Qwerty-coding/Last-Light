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
    [SerializeField] private bool hasKey = false; 
    [SerializeField] private int logsCount = 0;

    [Header("Events")]
    // These are standard UnityEvents (void) so they match your other scripts
    public UnityEvent OnInventoryChanged = new UnityEvent();
    public UnityEvent OnGunChanged = new UnityEvent();
    public UnityEvent OnAxeChanged = new UnityEvent();
    public UnityEvent OnKeyChanged = new UnityEvent(); 
    public IntEvent OnLogsChanged = new IntEvent();

    private const string PrefKey_Gun = "FixedInv_hasGun";
    private const string PrefKey_Axe = "FixedInv_hasAxe";
    private const string PrefKey_Key = "FixedInv_hasKey";
    private const string PrefKey_Logs = "FixedInv_logs";

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // We do NOT call Load(). 
            // We call ResetInventory() to force a clean slate every time the game starts.
            ResetInventory(); 
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // --- ADDED THIS SECTION ---
    private void Start()
    {
        // 1. Lock the cursor to the center of the screen
        // This ensures your Raycast (which uses screen center) works immediately
        Cursor.lockState = CursorLockMode.Locked;
        
        // 2. Hide the cursor so you only see your crosshair
        Cursor.visible = false;

        // 3. Fire events to ensure UI is ready
        if (Application.isPlaying)
        {
            OnGunChanged?.Invoke();
            OnAxeChanged?.Invoke();
            OnKeyChanged?.Invoke();
            OnLogsChanged?.Invoke(logsCount);
            OnInventoryChanged?.Invoke();
        }
    }
    // --------------------------

    #region Public API

    public bool HasGun => hasGun;
    public bool HasAxe => hasAxe;
    public bool HasKey => hasKey;
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

    public void GiveKey()
    {
        if (!hasKey)
        {
            hasKey = true;
            OnKeyChanged?.Invoke();
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

    public void ClearInventory()
    {
        hasGun = false;
        hasAxe = false;
        hasKey = false; 
        logsCount = 0;
        
        OnGunChanged?.Invoke();
        OnAxeChanged?.Invoke();
        OnKeyChanged?.Invoke(); 
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
        PlayerPrefs.SetInt(PrefKey_Key, hasKey ? 1 : 0);
        PlayerPrefs.SetInt(PrefKey_Logs, logsCount);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        hasGun = PlayerPrefs.GetInt(PrefKey_Gun, hasGun ? 1 : 0) == 1;
        hasAxe = PlayerPrefs.GetInt(PrefKey_Axe, hasAxe ? 1 : 0) == 1;
        hasKey = PlayerPrefs.GetInt(PrefKey_Key, hasKey ? 1 : 0) == 1; 
        logsCount = PlayerPrefs.GetInt(PrefKey_Logs, logsCount);
    }

    [ContextMenu("Reset Inventory")]
    public void ResetInventory()
    {
        PlayerPrefs.DeleteAll();
        ClearInventory();
    }

    #endregion
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            OnGunChanged?.Invoke();
            OnAxeChanged?.Invoke();
            OnKeyChanged?.Invoke();
            OnLogsChanged?.Invoke(logsCount);
            OnInventoryChanged?.Invoke();
        }
    }
#endif
}