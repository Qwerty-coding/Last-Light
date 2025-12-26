using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("The Parents")]
    public GameObject gunParent; 
    public GameObject axeParent; 

    private void Start()
    {
        // 1. Start with everything hidden
        HideAllWeapons();

        // 2. Listen for Inventory Changes (Optional: Auto-equip on pickup)
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnInventoryChange.AddListener(OnInventoryUpdated);
        }
    }

    private void Update()
    {
        // Press 1 for Gun, 2 for Axe
        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleWeapon("Gun");
        if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleWeapon("Axe");
    }

    // This runs automatically when you pick something up
    private void OnInventoryUpdated()
    {
        // OPTIONAL: If we picked up a gun and hands are empty, equip it automatically
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
        // 1. Do we even own this item?
        if (!SimpleInventory.Instance.HasItem(weaponName)) return;

        // 2. Logic to Switch
        if (weaponName == "Gun")
        {
            // If gun is already out, put it away. Otherwise, equip it.
            if (gunParent.activeSelf) HideAllWeapons();
            else EquipGun();
        }
        else if (weaponName == "Axe")
        {
            // If axe is already out, put it away. Otherwise, equip it.
            if (axeParent.activeSelf) HideAllWeapons();
            else EquipAxe();
        }
    }

    private void EquipGun()
    {
        if (gunParent != null) gunParent.SetActive(true);
        if (axeParent != null) axeParent.SetActive(false);
    }

    private void EquipAxe()
    {
        if (axeParent != null) axeParent.SetActive(true);
        if (gunParent != null) gunParent.SetActive(false);
    }

    private void HideAllWeapons()
    {
        if (gunParent != null) gunParent.SetActive(false);
        if (axeParent != null) axeParent.SetActive(false);
    }

    private bool IsAnyWeaponActive()
    {
        return (gunParent != null && gunParent.activeSelf) || 
               (axeParent != null && axeParent.activeSelf);
    }
}