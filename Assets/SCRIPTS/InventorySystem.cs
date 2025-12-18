// ========================================
// FILE 1: InventorySystem.cs (UPDATED)
// ========================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;
    public bool isOpen;
    
    // NEW: Reference to item slots
    public List<GameObject> slotList = new List<GameObject>();
    
    // NEW: Item prefab for inventory (UI element)
    public GameObject itemPrefab; // Assign in Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        isOpen = false;
        
        // NEW: Automatically find all slots
        PopulateSlotList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            Debug.Log("i is pressed");
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            isOpen = false;
        }
    }
    
    // NEW: Find all inventory slots
    void PopulateSlotList()
    {
        slotList.Clear();

        if (inventoryScreenUI != null)
        {
            ItemSlot[] slots = inventoryScreenUI.GetComponentsInChildren<ItemSlot>(true);
            foreach (ItemSlot slot in slots)
            {
                if (slot != null)
                    slotList.Add(slot.gameObject);
            }
        }

        // Fallback to tag-based search if none found under the UI root
        if (slotList.Count == 0)
        {
            foreach (GameObject slot in GameObject.FindGameObjectsWithTag("Slot"))
            {
                if (slot != null)
                    slotList.Add(slot);
            }
        }

        Debug.Log($"InventorySystem: Found {slotList.Count} slot(s)");
    }
    
    // NEW: Add item to inventory with proper sizing
    public void AddToInventory(string itemName, Sprite itemSprite)
    {
        // Find first empty slot
        GameObject emptySlot = FindEmptySlot();
        
        if (emptySlot != null)
        {
            if (itemPrefab == null)
            {
                Debug.LogWarning("InventorySystem: itemPrefab is not assigned");
                return;
            }

            // Create the item UI element (preserve RectTransform layout)
            GameObject newItem = Instantiate(itemPrefab, emptySlot.transform);
            newItem.name = itemName;
            newItem.transform.SetParent(emptySlot.transform, false);
            
            // Set the item's image
            Image itemImage = newItem.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = itemSprite;
            }
            
            // Set proper size and position
            RectTransform itemRect = newItem.GetComponent<RectTransform>();
            if (itemRect != null)
            {
                // Center in slot
                itemRect.anchoredPosition = Vector2.zero;
                
                // Match slot size
                RectTransform slotRect = emptySlot.GetComponent<RectTransform>();
                itemRect.sizeDelta = slotRect.sizeDelta;
                
                // Reset scale
                itemRect.localScale = Vector3.one;
                
                // Set anchors to center
                itemRect.anchorMin = new Vector2(0.5f, 0.5f);
                itemRect.anchorMax = new Vector2(0.5f, 0.5f);
                itemRect.pivot = new Vector2(0.5f, 0.5f);
            }
            
            // Store item name for later reference
            InventoryItem invItem = newItem.GetComponent<InventoryItem>();
            if (invItem != null)
            {
                invItem.itemName = itemName;
            }
            
            Debug.Log($"InventorySystem: Added {itemName} to slot {emptySlot.name}");
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }
    
    // NEW: Find first empty slot
    GameObject FindEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            if (itemSlot != null && itemSlot.Item == null)
            {
                return slot;
            }
        }
        return null; // No empty slots
    }
}