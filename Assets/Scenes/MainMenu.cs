using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Exit button clicked!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}