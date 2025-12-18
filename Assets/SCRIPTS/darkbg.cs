

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class darkbg : MonoBehaviour
{
    private bool isOpen;

    public GameObject darkbgUI;
    void Start()
    {
        isOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            darkbgUI.SetActive(true);
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            darkbgUI.SetActive(false);
            isOpen = false;
        }
    }
}