using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInventory : MonoBehaviour
{
    public static SimpleInventory Instance;

    private Dictionary<string, int> items = new Dictionary<string, int>();

    [SerializeField] private List<string> debugItemList;

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

    public bool HasItem(string itemName)
    {
        return items.ContainsKey(itemName) && items[itemName] > 0;
    }

    public int GetCount(string itemName)
    {
        return items.ContainsKey(itemName) ? items[itemName] : 0;
    }

    public void AddItem(string itemName, int amount = 1)
    {
        itemName = itemName.Trim(); // SAFETY

        if (!items.ContainsKey(itemName))
            items[itemName] = 0;

        items[itemName] += amount;

        UpdateDebugList();
        OnInventoryChange.Invoke();
    }

    public void RemoveItem(string itemName, int amount = 1)
    {
        if (!items.ContainsKey(itemName)) return;

        items[itemName] -= amount;
        if (items[itemName] < 0) items[itemName] = 0;

        UpdateDebugList();
        OnInventoryChange.Invoke();
    }

    private void UpdateDebugList()
    {
        debugItemList.Clear();
        foreach (var item in items)
            debugItemList.Add($"{item.Key}: {item.Value}");
    }
}
