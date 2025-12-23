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

        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            // CHANGED: Now looks for 'ItemPickup' instead of 'InteractableObject'
            ItemPickup itemScript = selectionTransform.GetComponent<ItemPickup>();

            // Check if we hit an item AND the player is close enough to it
            if (itemScript != null && itemScript.isPlayerInRange)
            {
                onTarget = true;
                interaction_text.text = itemScript.itemName; // Get name from ItemPickup
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