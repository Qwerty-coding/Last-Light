using UnityEngine;
using TMPro; // Use UnityEngine.UI if using Legacy Text

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance { get; set; }
    public bool onTarget;
    
    [Header("UI References")]
    public GameObject interaction_Info_UI;
    TextMeshProUGUI interaction_text; // Change to 'Text' if using Legacy UI

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        onTarget = false;
        // Auto-get the text component from the UI object you dragged in
        interaction_text = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

<<<<<<< Updated upstream
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            // CHANGED: Now looks for 'ItemPickup' instead of 'InteractableObject'
            ItemPickup itemScript = selectionTransform.GetComponent<ItemPickup>();
=======
        // Limit the ray distance (e.g., 5 meters)
        if (Physics.Raycast(ray, out hit, 5f))
        {
            // --- THE FIX IS HERE ---
            // We changed 'StoryPickup' to 'ItemPickup' to match the script on your Gun/Axe
            ItemPickup itemScript = hit.transform.GetComponentInParent<ItemPickup>();

            // Check for Doors
            KeyDoorMech keyDoorScript = hit.transform.GetComponentInParent<KeyDoorMech>();
>>>>>>> Stashed changes

            // Check if we hit an item AND the player is close enough to it
            if (itemScript != null && itemScript.isPlayerInRange)
            {
<<<<<<< Updated upstream
=======
                // Logic: Must be looking at it (Raycast) AND standing close (Trigger)
                if (itemScript.isPlayerInRange)
                {
                    onTarget = true;
                    
                    // Display the Item ID (e.g., "Gun")
                    if (interaction_text != null) interaction_text.text = itemScript.itemID;
                    
                    interaction_Info_UI.SetActive(true);
                }
                else
                {
                    // Looking at it, but too far away
                    onTarget = false;
                    interaction_Info_UI.SetActive(false);
                }
            }
            // Logic for Doors
            else if (keyDoorScript != null)
            {
>>>>>>> Stashed changes
                onTarget = true;
                interaction_text.text = itemScript.itemName; // Get name from ItemPickup
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                // Looking at a wall or something else
                onTarget = false;
                interaction_Info_UI.SetActive(false);
            }
        }
        else
        {
            // Raycast hit nothing (looking at sky)
            onTarget = false;
            interaction_Info_UI.SetActive(false);
        }
    }
}