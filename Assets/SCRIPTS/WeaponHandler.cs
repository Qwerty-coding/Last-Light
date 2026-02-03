using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("The Parents")]
    public GameObject gunParent; 
    public GameObject axeParent;
    
    [Header("Starting Weapon (Optional)")]
    public bool startWithAxe = true; // Check this if you want axe equipped at start
    public bool startWithGun = false;
    
    private void Start()
    {
        // Hide all weapons first
        HideAllWeapons();
        
        // Then equip starting weapon if specified
        if (startWithAxe)
        {
            EquipAxe();
        }
        else if (startWithGun)
        {
            EquipGun();
        }
        
        // Listen for Inventory Changes (Optional: Auto-equip on pickup)
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnInventoryChange.AddListener(OnInventoryUpdated);
        }
    }
    
    private void Update()
    {
        // Press 1 for Gun, 2 for Axe
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            ToggleWeapon("Gun");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            ToggleWeapon("Axe");
        }
    }
    
    // This runs automatically when you pick something up
    private void OnInventoryUpdated()
    {
        // OPTIONAL: If we picked up a weapon and hands are empty, equip it automatically
        if (SimpleInventory.Instance.HasItem("Gun") && !IsAnyWeaponActive())
        {
            EquipGun();
        }
        else if (SimpleInventory.Instance.HasItem("Axe") && !IsAnyWeaponActive())
        {
            EquipAxe();
        }
    }
    
    // --- MAIN LOGIC ---
    public void ToggleWeapon(string weaponName)
    {
        // Check if SimpleInventory exists
        if (SimpleInventory.Instance == null)
        {
            Debug.LogWarning("SimpleInventory not found! Allowing weapon switch anyway.");
            // If no inventory system, just allow switching
            if (weaponName == "Gun") EquipGun();
            else if (weaponName == "Axe") EquipAxe();
            return;
        }
        
        // Do we own this item in inventory?
        if (!SimpleInventory.Instance.HasItem(weaponName))
        {
            Debug.Log($"You don't have {weaponName} in inventory!");
            return;
        }
        
        // Switch weapons - only ONE active at a time
        if (weaponName == "Gun")
        {
            // If gun is already out, put it away. Otherwise, equip it.
            if (gunParent != null && gunParent.activeSelf)
            {
                HideAllWeapons();
                Debug.Log("Gun holstered");
            }
            else
            {
                EquipGun(); // This automatically hides axe
            }
        }
        else if (weaponName == "Axe")
        {
            // If axe is already out, put it away. Otherwise, equip it.
            if (axeParent != null && axeParent.activeSelf)
            {
                HideAllWeapons();
                Debug.Log("Axe holstered");
            }
            else
            {
                EquipAxe(); // This automatically hides gun
            }
        }
    }
    
    // NEW - Method to check if we have a specific weapon before trying to use it
    public bool HasWeaponInInventory(string weaponName)
    {
        if (SimpleInventory.Instance == null) return false;
        return SimpleInventory.Instance.HasItem(weaponName);
    }
    
    private void EquipGun()
    {
        Debug.Log("Equipping Gun");
        
        // Enable gun
        if (gunParent != null)
        {
            gunParent.SetActive(true);
        }
        else
        {
            Debug.LogError("Gun Parent is not assigned in Inspector!");
        }
        
        // Disable axe (only one weapon at a time)
        if (axeParent != null)
        {
            axeParent.SetActive(false);
        }
    }
    
    private void EquipAxe()
    {
        Debug.Log("Equipping Axe");
        
        // Enable axe
        if (axeParent != null)
        {
            axeParent.SetActive(true);
        }
        // else
        // {
        //     Debug.LogError("Axe Parent is not assigned in Inspector!");
        // }
        
        // Disable gun (only one weapon at a time)
        if (gunParent != null)
        {
            gunParent.SetActive(false);
        }
    }
    
    private void HideAllWeapons()
    {
        // Debug.Log("Hiding all weapons");
        
        if (gunParent != null)
        {
            gunParent.SetActive(false);
        }
        
        if (axeParent != null)
        {
            axeParent.SetActive(false);
        }
    }
    
    private bool IsAnyWeaponActive()
    {
        bool gunActive = gunParent != null && gunParent.activeSelf;
        bool axeActive = axeParent != null && axeParent.activeSelf;
        
        return gunActive || axeActive;
    }
    
    // PUBLIC METHODS - Call these from other scripts if needed
    
    public void ForceEquipGun()
    {
        EquipGun();
    }
    
    public void ForceEquipAxe()
    {
        EquipAxe();
    }
    
    public void ForceHideAll()
    {
        HideAllWeapons();
    }
    
    public string GetCurrentWeapon()
    {
        if (gunParent != null && gunParent.activeSelf) return "Gun";
        if (axeParent != null && axeParent.activeSelf) return "Axe";
        return "None";
    }
}