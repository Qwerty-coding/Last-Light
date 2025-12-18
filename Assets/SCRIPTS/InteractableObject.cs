using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class InteractableObject : MonoBehaviour
{
    public string ItemName;
    
    // NEW: Add sprite for inventory icon
    public Sprite itemIcon;
    
    public bool playerInRange;
    
    public string GetItemName()
    {
        return ItemName;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.instance.onTarget)
        {
            Debug.Log("Interacted with " + ItemName);
            
            // NEW: Add to inventory before destroying
            if (InventorySystem.Instance != null && itemIcon != null)
            {
                InventorySystem.Instance.AddToInventory(ItemName, itemIcon);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Cannot add item: InventorySystem or itemIcon is missing!");
            }
            
            
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}