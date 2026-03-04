using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

public class VictoryManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryPanel; 

    // This is the specific method your Boss script is looking for
    public void ShowVictory()
    {
        Debug.Log("🏆 Victory Triggered!");
        if (victoryPanel != null)
        
        {
            // 1. Pause the game immediately
            Time.timeScale = 0f; 

            // 2. Start the Pop Up Animation
            StartCoroutine(AnimatePopUp());
        }

        // 3. Unlock mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator AnimatePopUp()
    {
        victoryPanel.SetActive(true);

        // Get the rect transform to modify scale
        RectTransform rect = victoryPanel.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero; // Start invisible (size 0)

        float timer = 0f;
        float duration = 0.5f; // Animation speed (0.5 seconds)

        // Animation Loop (Using Unscaled time so it runs while game is paused)
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / duration;
            
            // "SmootherStep" math for a nice pop effect
            float scale = Mathf.SmoothStep(0f, 1f, progress); 
            rect.localScale = Vector3.one * scale;
            
            yield return null;
        }

        // Ensure it ends at exactly 100% size
        rect.localScale = Vector3.one;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}