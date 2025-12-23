using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for UI elements

public class GameManagerScript : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverUI;           // Your Game Over Screen
    public GameObject ammoUI;               // Your "0/0" text
    public CanvasGroup gameOverCanvasGroup; // The Canvas Group on the Game Over Screen
    public CanvasGroup transitionOverlay;   // <--- NEW: The Black Panel (TransitionPanel)

    void Start()
    {
        // 1. Setup Game Over Screen (Hide it)
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
            gameOverUI.transform.localScale = Vector3.zero; // Start tiny
        }
        
        // 2. Setup Ammo (Show it)
        if (ammoUI != null) ammoUI.SetActive(true);

        // 3. Setup Transition Panel (Make it invisible and non-blocking)
        if (transitionOverlay != null)
        {
            transitionOverlay.alpha = 0f;
            transitionOverlay.blocksRaycasts = false; // Allow clicks
        }
    }

    public void TriggerGameOver()
    {
        // Hide the ammo counter
        if (ammoUI != null) ammoUI.SetActive(false);

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            
            // Pause physics immediately
            Time.timeScale = 0f;

            // Unlock cursor so you can click buttons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Start the Pop-Up Animation
            if (gameOverCanvasGroup != null)
            {
                gameOverCanvasGroup.alpha = 0f;
                gameOverUI.transform.localScale = Vector3.zero;
                StartCoroutine(FadeInScreen());
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (FixedInventory.Instance != null)
        {
            FixedInventory.Instance.ResetInventory();
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Must unpause to let animation play
        StartCoroutine(FadeOutAndLoad()); // <--- NEW: Calls the black fade routine
    }

    // ANIMATION 1: Game Over Pop-Up (Zoom + Fade In)
    System.Collections.IEnumerator FadeInScreen()
    {
        float duration = 0.3f; // Fast and snappy
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / duration;

            // Fade Alpha from 0 to 1
            gameOverCanvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);
            
            // Zoom Scale from 0 to 1
            gameOverUI.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);

            yield return null;
        }

        // Ensure final state
        gameOverCanvasGroup.alpha = 1f;
        gameOverUI.transform.localScale = Vector3.one;
    }

    // ANIMATION 2: Transition to Main Menu (Fade to Black)
    System.Collections.IEnumerator FadeOutAndLoad()
    {
        // Block clicks so user doesn't double-click
        if (transitionOverlay != null) 
        {
            transitionOverlay.blocksRaycasts = true; 
        }

        float duration = 0.5f; // Duration of fade to black
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            
            if (transitionOverlay != null)
            {
                // Fade Alpha from 0 (Transparent) to 1 (Black)
                transitionOverlay.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            }
            yield return null;
        }

        // Wait a tiny moment at full black (optional polish)
        yield return new WaitForSecondsRealtime(0.1f);

        // Finally Load the Scene
        SceneManager.LoadScene("MainMenu");
    }
}