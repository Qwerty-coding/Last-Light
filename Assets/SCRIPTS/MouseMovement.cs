using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    
    // NEW: We need a reference to the camera specifically
    public Transform playerCamera; 

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        // Auto-find the camera if you forget to drag it in
        if (playerCamera == null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 1. LOOK UP/DOWN (Rotate Camera Only)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        // We apply this ONLY to the camera, not the body
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 2. LOOK LEFT/RIGHT (Rotate Player Body)
        // We rotate the entire player object around the Y axis
        transform.Rotate(Vector3.up * mouseX);
    }
}