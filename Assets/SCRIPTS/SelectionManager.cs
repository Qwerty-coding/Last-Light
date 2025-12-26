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

        // Limit the ray distance (e.g., 5 meters) so you can't select items from across the room
        if (Physics.Raycast(ray, out hit, 5f))
        {
            // 1. Check for Items (Using the NEW script)
            UniversalPickup itemScript = hit.transform.GetComponentInParent<UniversalPickup>();

            // 2. Check for Doors
            KeyDoorMech keyDoorScript = hit.transform.GetComponentInParent<KeyDoorMech>();

            if (itemScript != null)
            {
                // REQUIRES: public bool isPlayerInRange in UniversalPickup.cs
                if (itemScript.isPlayerInRange)
                {
                    onTarget = true;
                    
                    // Display the Item ID (e.g., "Gun", "Key")
                    if (interaction_text != null) interaction_text.text = itemScript.itemID;
                    
                    interaction_Info_UI.SetActive(true);
                }
                else
                {
                    onTarget = false;
                    interaction_Info_UI.SetActive(false);
                }
            }
            // 3. Check if we hit the KeyDoor
            else if (keyDoorScript != null)
            {
                // Optional: Check distance or range here if needed
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
                onTarget = false;
                interaction_Info_UI.SetActive(false);
            }
        }
        else
        {
            onTarget = false;
            interaction_Info_UI.SetActive(false);
        }
    }
}