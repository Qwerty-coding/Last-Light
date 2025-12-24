using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    private Camera mainCamera;
    
    // Optional: Offset Z to ensure it renders in front of/behind UI as needed
    public float depthFromCamera = 10f; 

    void Start()
    {
        mainCamera = Camera.main;
        
        // Hide the system cursor if you want to replace it entirely
        // Cursor.visible = false; 
    }

    void Update()
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        // Get mouse position in screen coordinates (pixels)
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Set the distance from the camera so it doesn't clip or disappear
        mouseScreenPosition.z = depthFromCamera;

        // Convert screen position to world position
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // Apply position to this object
        transform.position = mouseWorldPosition;
    }
}