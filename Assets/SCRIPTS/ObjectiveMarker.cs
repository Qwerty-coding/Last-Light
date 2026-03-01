using UnityEngine;

// Attach this script to each marker GameObject you have dropped in the scene.
// In the Inspector, set the "objectiveKey" to match exactly the objective text
// that ObjectiveManager sets when this marker should be visible.
//
// Example:
//   Marker near the axe      -> objectiveKey = "Find the Axe in the outside storeroom"
//   Marker near the NPC      -> objectiveKey = "Trade logs with the NPC for a gun"
//   Marker near fire tower   -> objectiveKey = "Key Found! Find the Fire Tower and reach the top"
//
// ObjectiveManager will call ObjectiveMarker.RefreshAllMarkers() every time
// the objective changes, and only the matching marker will be visible.

public class ObjectiveMarker : MonoBehaviour
{
    [Header("Marker Settings")]
    [Tooltip("Must match exactly the objective string set in ObjectiveManager")]
    public string objectiveKey;

    [Header("Floating Animation")]
    public float floatSpeed = 3f;
    public float floatHeight = 0.2f;

    private Vector3 startPos;
    private Camera mainCam;

    private void Start()
    {
        startPos = transform.localPosition;
        mainCam = Camera.main;

        // Hide by default on start - ObjectiveManager will show the right one
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        // Billboard: always face the camera
        if (mainCam != null)
            transform.rotation = mainCam.transform.rotation;

        // Floating bob animation
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }

    // Called by ObjectiveManager when objective changes.
    // Shows this marker only if its key matches the current objective.
    public void OnObjectiveChanged(string currentObjective)
    {
        bool shouldBeVisible = (currentObjective == objectiveKey);
        gameObject.SetActive(shouldBeVisible);
    }

    // Static helper - refreshes ALL markers in the scene at once
    public static void RefreshAllMarkers(string currentObjective)
    {
        ObjectiveMarker[] allMarkers = FindObjectsOfType<ObjectiveMarker>(true); // true = include inactive
        foreach (ObjectiveMarker marker in allMarkers)
        {
            marker.OnObjectiveChanged(currentObjective);
        }
    }
}