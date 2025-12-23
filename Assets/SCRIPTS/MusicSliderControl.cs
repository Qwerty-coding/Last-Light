using UnityEngine;
using UnityEngine.UI;

public class MusicSliderControl : MonoBehaviour
{
    [Header("Assign these in the Inspector")]
    public Slider volumeSlider;
    public AudioSource musicSource;

    void Start()
    {
        // 1. Make sure we have the connections
        if (musicSource != null && volumeSlider != null)
        {
            // Set the slider handle to match the current music volume (0 to 1)
            volumeSlider.value = musicSource.volume;

            // Tell the slider to run "ChangeVolume" whenever it is moved
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }
    }

    // This runs every time you drag the handle
    void ChangeVolume(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
        }
    }
}