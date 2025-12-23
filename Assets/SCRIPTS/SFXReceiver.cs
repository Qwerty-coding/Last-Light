using UnityEngine;

public class SFXReceiver : MonoBehaviour
{
    void Start()
    {
        // 1. Find ALL Audio Sources on this object AND any child objects
        AudioSource[] allAudioSources = GetComponentsInChildren<AudioSource>();

        // 2. Read the "SFXVolume" from the mailbox
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 3. Loop through every single Audio Source found and set the volume
        foreach (AudioSource source in allAudioSources)
        {
            source.volume = savedVolume;
        }
    }
}
