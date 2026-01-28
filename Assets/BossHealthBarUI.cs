using UnityEngine;
using System.Collections;

public class BossUIAnimator : MonoBehaviour
{
    public RectTransform bossPanel;
    public float slideInDuration = 0.5f;

    private Vector2 hiddenPosition = new Vector2(0, 100); // Above screen
    private Vector2 shownPosition = new Vector2(0, -60);  // Visible position

    void Start()
    {
        if (bossPanel != null)
            bossPanel.anchoredPosition = hiddenPosition;
    }

    public void SlideIn()
    {
        StartCoroutine(SlideInCoroutine());
    }

    IEnumerator SlideInCoroutine()
    {
        float elapsed = 0f;
        
        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideInDuration;
            
            bossPanel.anchoredPosition = Vector2.Lerp(hiddenPosition, shownPosition, t);
            
            yield return null;
        }
        
        bossPanel.anchoredPosition = shownPosition;
    }
}