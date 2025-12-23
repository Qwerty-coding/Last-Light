using UnityEngine;

public class GameMusicReceiver : MonoBehaviour
{
    void Start()
    {
        // 1. Get the Audio Source on THIS object
        AudioSource myAudio = GetComponent<AudioSource>();

        if (myAudio != null)
        {
            // 2. Check the "Mailbox" for the volume setting
            float savedVolume = PlayerPrefs.GetFloat("GameBGM", 1f);
            
            // 3. Apply it
            myAudio.volume = savedVolume;
        }
    }
}