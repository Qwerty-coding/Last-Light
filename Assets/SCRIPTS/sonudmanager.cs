using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get;set; }

    public AudioSource shootingSound1911;
    public AudioSource reloadingSound1911;
    public AudioSource emptyManagizeSound1911;
    private void Awake()
    {
        // If another instance exists, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
       
        }

        // Set this as the main instance
        Instance = this;

        // Optional: keep sound manager across scenes
    }
}
