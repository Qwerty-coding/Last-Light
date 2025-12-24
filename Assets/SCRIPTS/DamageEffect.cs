using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageEffect : MonoBehaviour
{
    [Header("Setup")]
    public Image bloodImage;       // Drag your BloodOverlay here
    
    [Header("Settings")]
    public float fadeSpeed = 3f;   // Higher = fades faster
    public Color flashColor = new Color(1f, 1f, 1f, 0f); // The color when you get hit

// Add this generic Start method
    void Start()
    {
        // This ensures the blood is ALWAYS invisible when you hit Play
        if (bloodImage != null)
        {
            Color resetColor = bloodImage.color;
            resetColor.a = 0f;
            bloodImage.color = resetColor;
        }
    }
    public void ShowDamage()
    {
        // 1. Instantly make the blood visible
        bloodImage.color = flashColor;

        // 2. Start fading it out
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // While the image is still partially visible...
        while (bloodImage.color.a > 0.01f)
        {
            // Slowly fade the alpha (transparency) down to 0
            Color newColor = bloodImage.color;
            newColor.a = Mathf.Lerp(newColor.a, 0f, fadeSpeed * Time.deltaTime);
            bloodImage.color = newColor;
            
            yield return null; // Wait for the next frame
        }

        // Force it to be perfectly invisible at the end
        Color finalColor = bloodImage.color;
        finalColor.a = 0f;
        bloodImage.color = finalColor;
    }
}
