using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

[RequireComponent(typeof(Image))]

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabAreaGroup tabGroup;

    public Image background;

    public bool selectable;

    public bool existObj = false; 

    public void OnPointerClick(PointerEventData eventData)
    {
        if(selectable)
            tabGroup.onTabSelected(this); 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this); 
    }

    private void Start()
    {
        tabGroup = GetComponentInParent<TabAreaGroup>(); 
        background = GetComponent<Image>();
    }



}
