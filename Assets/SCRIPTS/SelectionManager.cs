using System;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance{get;set;}
    public bool onTarget;
    public GameObject interaction_Info_UI;
    TextMeshProUGUI interaction_text;
 
    private void Start()
    {
        onTarget = false;
        interaction_text = interaction_Info_UI.GetComponent<TextMeshProUGUI>();
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            
        }
        else 
        {
            instance = this;
        }
    }
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
 
            if (selectionTransform.GetComponent<InteractableObject>() && selectionTransform.GetComponent<InteractableObject>().playerInRange)
            {
                onTarget = true;
                interaction_text.text = selectionTransform.GetComponent<InteractableObject>().GetItemName();
                interaction_Info_UI.SetActive(true);
            }
            else 
            { 
                onTarget = false;
                interaction_Info_UI.SetActive(false);
            }}
        else 
            { 
                onTarget = false;
                interaction_Info_UI.SetActive(false);
            }

        
    }

}
