using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for Scene changing

public class GameManagerScript : MonoBehaviour
{
    public GameObject gameOverUI; // Reference to your Game Over Screen object
    public GameObject ammoUI;     // <--- ADD THIS LINE (New variable for ammo text)
    public CanvasGroup gameOverCanvasGroup; // <--- ADD THIS LINE
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
        // 1. Hide the Ammo
        if (ammoUI != null) ammoUI.SetActive(false);

        // 2. Show the Game Over Screen
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            
            // 3. Pause the game physics immediately
            Time.timeScale = 0f;

            // 4. Unlock the mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 5. Start the fade animation (if assigned)
            if (gameOverCanvasGroup != null)
            {
                // Reset alpha to 0 (invisible) before starting
                gameOverCanvasGroup.alpha = 0f;
                gameOverUI.transform.localScale = Vector3.zero; // <--- NEW LINE: Makes it tiny!
                StartCoroutine(FadeInScreen());
            }
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
    System.Collections.IEnumerator FadeInScreen()
    {
        float duration = 0.5f; // Animation takes 1 second
        float timer = 0f;

        while (timer < duration)
        {
            // We use 'unscaledDeltaTime' so it works even while the game is paused!
            timer += Time.unscaledDeltaTime;
            
          float progress = timer / duration;
gameOverCanvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);
gameOverUI.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress); // <--- NEW LINE: Grows it!
            
            yield return null; // Wait for next frame
        }

        gameOverCanvasGroup.alpha = 1f; // Ensure it's fully visible at the end
        gameOverUI.transform.localScale = Vector3.one; // <--- NEW LINE: Ensures it finishes at full size
    }
    
}