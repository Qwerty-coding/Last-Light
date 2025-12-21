using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for Scene changing

public class GameManagerScript : MonoBehaviour
{
    public GameObject gameOverUI; // Reference to your Game Over Screen object

    void Start()
    {
        // Ensure the game over screen is hidden when the game starts
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
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