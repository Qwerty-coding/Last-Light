using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("The Parents")]
    // Drag 'WeaponSpawn' here. 
    // Since Ammo/Sound/UI are children of it, they will vanish too!
    public GameObject gunParent; 
    
    // Drag 'Axe_Parent' here
    public GameObject axeParent; 

    private void Start()
    {
        // 1. Start with everything hidden
        if (gunParent != null) gunParent.SetActive(false);
        if (axeParent != null) axeParent.SetActive(false);
        
        // 2. Listen for Inventory
        if (FixedInventory.Instance != null)
        {
            FixedInventory.Instance.OnGunChanged.AddListener(OnGunPickedUp);
            FixedInventory.Instance.OnAxeChanged.AddListener(OnAxePickedUp);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleGun();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleAxe();
    }

    private void OnGunPickedUp()
    {
        if (FixedInventory.Instance.HasGun)
        {
            if (gunParent != null) gunParent.SetActive(true);
            if (axeParent != null) axeParent.SetActive(false);
        }
    }

    private void OnAxePickedUp()
    {
        if (FixedInventory.Instance.HasAxe)
        {
            if (axeParent != null) axeParent.SetActive(true);
            if (gunParent != null) gunParent.SetActive(false);
        }
    }

    private void ToggleGun()
    {
        if (FixedInventory.Instance.HasGun)
        {
            bool newState = !gunParent.activeSelf;
            gunParent.SetActive(newState);

            if (newState && axeParent != null) axeParent.SetActive(false);
        }
    }

    private void ToggleAxe()
    {
        if (FixedInventory.Instance.HasAxe)
        {
            bool newState = !axeParent.activeSelf;
            axeParent.SetActive(newState);

            if (newState && gunParent != null) gunParent.SetActive(false);
        }
    }
}