using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInventory : MonoBehaviour
{
    public static SimpleInventory Instance;

    // The "Backpack" - Stores Item Name and Quantity
    // Example: "Gun": 1, "Logs": 5, "Key": 1
    private Dictionary<string, int> items = new Dictionary<string, int>();

    [Header("Debug View (Read Only)")]
    [SerializeField] private List<string> debugItemList; // Shows what you have in Inspector

    // ONE event for everything. The UI just listens to this.
    public UnityEvent OnInventoryChange = new UnityEvent();

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- CHECK ITEMS ---
    public bool HasItem(string itemName)
    {
        // Do we have the item AND is the count greater than 0?
        return items.ContainsKey(itemName) && items[itemName] > 0;
    }

    public int GetCount(string itemName)
    {
        if (items.ContainsKey(itemName))
            return items[itemName];
        return 0;
    }

    // --- ADD ITEMS ---
    public void AddItem(string itemName, int amount = 1)
    {
        Debug.Log("🚨 ITEM ADDED: " + itemName + " | TRIGGERED BY: " + new System.Diagnostics.StackTrace());
        // ---------------------
        
        if (!items.ContainsKey(itemName))
        {
            items.Add(itemName, 0);
        }

        items[itemName] += amount;
        
        UpdateDebugList();
        OnInventoryChange.Invoke(); // Tell UI to refresh
    }

    // --- REMOVE ITEMS ---
    public void RemoveItem(string itemName, int amount = 1)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] -= amount;
            if (items[itemName] < 0) items[itemName] = 0;
            
            UpdateDebugList();
            OnInventoryChange.Invoke();
        }
    }

    private void UpdateDebugList()
    {
        debugItemList.Clear();
        foreach (var item in items)
        {
            debugItemList.Add($"{item.Key}: {item.Value}");
        }
    }
}