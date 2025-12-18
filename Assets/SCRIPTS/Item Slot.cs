using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            if (transform.childCount > 0 )
            {
                return transform.GetChild(0).gameObject;
            }
 
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");

        if (!Item)
        {
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            
            // Fix size when dragging between slots
            RectTransform itemRect = DragDrop.itemBeingDragged.GetComponent<RectTransform>();
            RectTransform slotRect = GetComponent<RectTransform>();
            
            itemRect.anchoredPosition = Vector2.zero;
            itemRect.localScale = Vector3.one;
            itemRect.sizeDelta = slotRect.sizeDelta;
            
            itemRect.anchorMin = new Vector2(0.5f, 0.5f);
            itemRect.anchorMax = new Vector2(0.5f, 0.5f);
            itemRect.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}