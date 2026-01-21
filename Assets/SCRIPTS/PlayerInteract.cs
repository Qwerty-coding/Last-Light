using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 3f; // Distance required to talk
    public KeyCode interactKey = KeyCode.E; // The key to press

    void Update()
    {
        // Change: Check for KeyCode.E instead of MouseButton
        if (Input.GetKeyDown(interactKey)) 
        {
            Interact();
        }
    }

    void Interact()
    {
        // 1. Create a Ray from the center of the camera
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // 2. Shoot the ray
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // 3. Check if we hit the NPC (Does it have the shop script?)
            NPCShop shop = hit.collider.GetComponent<NPCShop>();
            
            if (shop != null)
            {
                // Open the menu!
                shop.OpenShop();
            }
        }
    }
}