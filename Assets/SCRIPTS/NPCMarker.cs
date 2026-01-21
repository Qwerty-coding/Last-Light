using UnityEngine;

public class NPCMarker : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatSpeed = 3f;     // How fast it bobs up and down
    public float floatHeight = 0.2f;  // How far it moves

    private Vector3 startPos;
    private Camera mainCam;

    void Start()
    {
        // Store the local starting position relative to the NPC
        startPos = transform.localPosition;
        mainCam = Camera.main;
    }

    // LateUpdate is better for camera-related movement to prevent "jittering"
    void LateUpdate()
    {
        // 1. Face the Camera (Billboard Effect)
        // We match the camera's rotation so the UI is always flat to the player's view
        if (mainCam != null)
        {
            transform.rotation = mainCam.transform.rotation;
        }

        // 2. Floating Animation
        // Uses a Sine wave to move the Y position smoothly over time
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}