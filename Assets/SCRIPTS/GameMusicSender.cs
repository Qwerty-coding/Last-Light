using UnityEngine;
using UnityEngine.UI;

public class GameMusicSender : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("GameBGM", 1f);
        
        if (slider != null)
        {
            slider.value = savedVolume;
            slider.onValueChanged.AddListener(SaveVolume);
        }
    }

    public void SaveVolume(float value)
    {
        PlayerPrefs.SetFloat("GameBGM", value);
        PlayerPrefs.Save();
    }
}
