using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialCanvas;

    public void OpenTutorial()
    {
        tutorialCanvas.SetActive(true);
    }

    public void CloseTutorial()
    {
        tutorialCanvas.SetActive(false);
    }
}