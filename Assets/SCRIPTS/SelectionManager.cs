using UnityEngine;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance { get; set; }
    public bool onTarget;

    [Header("UI References")]
    public GameObject interaction_Info_UI;
    TextMeshProUGUI interaction_text;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void Start()
    {
        if (interaction_Info_UI != null)
            interaction_text = interaction_Info_UI.GetComponent<TextMeshProUGUI>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Cast a ray from the center of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Limit the ray distance (e.g., 5 meters)
        if (Physics.Raycast(ray, out hit, 5f))
        {
            // --- THE FIX IS HERE ---
            // We changed 'StoryPickup' to 'ItemPickup' to match the script on your Gun/Axe
            ItemPickup itemScript = hit.transform.GetComponentInParent<ItemPickup>();

            // Check for Doors
            KeyDoorMech keyDoorScript = hit.transform.GetComponentInParent<KeyDoorMech>();

            if (itemScript != null)
            {
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
                onTarget = true;

                if (interaction_text != null)
                {
                    if (keyDoorScript.isLocked)
                        interaction_text.text = "Locked";
                    else
                        interaction_text.text = "Open";
                }
                
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