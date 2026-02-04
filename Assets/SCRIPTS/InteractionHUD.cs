using UnityEngine;
using TMPro;

public class InteractionHUD : MonoBehaviour
{
    [Header("UI Setup")]
    public TextMeshProUGUI nameText; 
    public float rayDistance = 10f;

    void Update()
    {
        // Create the ray from center of screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // VISUAL DEBUG: This draws a line in your SCENE view (Red = hitting nothing, Green = hitting something)
        bool isHit = Physics.Raycast(ray, out hit, rayDistance);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, isHit ? Color.green : Color.red);

        if (isHit)
        {
            // LOG: Tell us what the Raycast hit
            // Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            // Try finding the script on the hit object OR its parents (in case you hit leaves)
            TreeInteractable tree = hit.collider.GetComponentInParent<TreeInteractable>();

            if (tree != null)
            {
                if (nameText != null)
                {
                    nameText.text = tree.treeName;
                    nameText.gameObject.SetActive(true);
                    // Debug.Log("Found Tree! Showing name: " + tree.treeName);
                }
                else
                {
                    Debug.LogError("HUD ERROR: nameText is not assigned in the Inspector!");
                }
            }
            else
            {
                HideText();
            }
        }
        else
        {
            HideText();
        }
    }

    void HideText()
    {
        if (nameText != null && nameText.gameObject.activeSelf)
        {
            nameText.gameObject.SetActive(false);
        }
    }
}