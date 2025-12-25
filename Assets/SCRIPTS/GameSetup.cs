using UnityEngine;

public class GameSetup : MonoBehaviour
{
    void Start()
    {
        // This locks the cursor to the center of the screen
        // so you don't need to click to focus.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}