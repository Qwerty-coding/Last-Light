using UnityEngine;
using UnityEngine.UI;

public class SFXSender : MonoBehaviour
{
    public Slider sfxSlider;

    void Start()
    {
        // 1. Load the saved SFX volume (default to 1)
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 2. Set the slider to match
        if (sfxSlider != null)
        {
            sfxSlider.value = savedVolume;
            // 3. Listen for changes
            sfxSlider.onValueChanged.AddListener(SaveSFXVolume);
        }
    }

    public void SaveSFXVolume(float value)
    {
        // 4. Save to the "SFXVolume" mailbox
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
}
