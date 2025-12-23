using UnityEngine;
using UnityEngine.UI;

public class MusicSliderControl : MonoBehaviour
{
    [Header("Assign these in the Inspector")]
    public Slider volumeSlider;
    public AudioSource musicSource;

    void Start()
    {
        // Set the slider handle to match the current music volume
        if (musicSource != null && volumeSlider != null)
        {
            volumeSlider.value = musicSource.volume;
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }
    }

    void ChangeVolume(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
        }
    }
}