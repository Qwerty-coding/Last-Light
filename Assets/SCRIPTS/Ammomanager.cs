using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ammomanager : MonoBehaviour
{
     
     public TextMeshProUGUI ammoDisplay;

    public static Ammomanager Instance { get;set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
