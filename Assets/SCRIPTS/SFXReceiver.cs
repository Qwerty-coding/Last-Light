using UnityEngine;

public class SFXReceiver : MonoBehaviour
{
    void Start()
    {
        // 1. Get the Audio Source on this object
        AudioSource myAudio = GetComponent<AudioSource>();

        if (myAudio != null)
        {
            // 2. Read the "SFXVolume" from the mailbox
            float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            
            // 3. Apply it
            myAudio.volume = savedVolume;
        }
    }
}
