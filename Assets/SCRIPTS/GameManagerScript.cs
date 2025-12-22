using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for Scene changing

public class GameManagerScript : MonoBehaviour
{
    public GameObject gameOverUI; // Reference to your Game Over Screen object
    public GameObject ammoUI;     // <--- ADD THIS LINE (New variable for ammo text)
    void Start()
    {
        // Ensure the game over screen is hidden when the game starts
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        if (ammoUI != null) ammoUI.SetActive(true); // <--- ADD THIS LINE (Ensures ammo is visible at start)
    }

    public void TriggerGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // Show the screen
            Time.timeScale = 0f;        // Pause the game (physics & time stop)
            
            // If you use a First Person Controller, you need to unlock the mouse:
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (ammoUI != null) ammoUI.SetActive(false); // <--- ADD THIS LINE (Hides ammo when you die)
    }

    // Call this from your "Restart" button
    public void RestartGame()
    {
        Time.timeScale = 1f; // IMPORTANT: Unpause time before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Call this from your "Main Menu" button
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Make sure this matches your menu scene name exactly
    }
}