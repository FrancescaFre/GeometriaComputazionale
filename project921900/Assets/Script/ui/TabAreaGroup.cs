using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TabAreaGroup : MonoBehaviour
{
    public List<TabButton> tabButton;
    public float tabIdle;
    public float tabHover;
    public float tabActive;

    public TabButton selectedTab;

    public List<GameObject> pageToSwap;

    public Controller controller;
    public UI_Controller ui; 

    private void Awake()
    {
        controller = FindObjectOfType<Controller>();
        ui = FindObjectOfType<UI_Controller>();

        foreach (GameObject go in pageToSwap)
            go.SetActive(false); 
     
    }

    public void OnTabEnter(TabButton button) { //hover
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
            button.background.color = new Color(button.background.color.r,
                                                button.background.color.g,
                                                button.background.color.b,
                                                tabHover);
    }

    public void OnTabExit(TabButton button) { //via dal pulsante
        ResetTabs(); 
    }

    public void onTabSelected(TabButton button) { //cliccare sul tab

        if (selectedTab == button) { //se premo sulla stessa tab per chiuderla
            selectedTab = null;
            ResetTabs();
            int i = button.transform.GetSiblingIndex();
            pageToSwap[i].SetActive(false);

            return; 
        }

        selectedTab = button; 

        ResetTabs();

        button.background.color = new Color(button.background.color.r,
                                            button.background.color.g,
                                            button.background.color.b,
                                            tabActive);

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < pageToSwap.Count; i++)
        {
            if (i == index)
            {
                pageToSwap[i].SetActive(true);
                if (i > 0)
                    ui.Set_ActiveTab(pageToSwap[i], i-1);
            }

            else
                pageToSwap[i].SetActive(false); 
        }
    }

    public void ResetTabs() {

        for (int i = 0; i < 11; i++)
        {
            Color color = Color.white;

            if (i > 0 && i-1< controller.all_blocks.Count)
                color = controller.all_blocks[i - 1].color;
        

            color.a = (selectedTab != null && tabButton[i] == selectedTab) ? tabActive : tabIdle;
            tabButton[i].background.color = color;
        }
       
    }

}
