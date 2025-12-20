using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerBehaviour : MonoBehaviour
{   
    [SerializeField]
    GameObject pauseMenu;

    public static bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused=false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
            {
            PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused=true;
        Time.timeScale=0f;
        Cursor.lockState = CursorLockMode.None; // Unlocks the cursor so it can move
    Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        isPaused=false;
        Time.timeScale=1f;
        Cursor.lockState = CursorLockMode.Locked; // Locks cursor to center of screen
    Cursor.visible = false;
    }

    public void MainMenu()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
