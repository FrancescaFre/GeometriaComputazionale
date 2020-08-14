﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public void Subscribe(TabButton button) {
        if (tabButton == null) {
            tabButton = new List<TabButton>(); 
        }

        tabButton.Add(button);
    }

    public void OnTabEnter(TabButton button) {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
            button.background.color = new Color(button.background.color.r,
                                                button.background.color.g,
                                                button.background.color.b,
                                                tabHover);
    }

    public void OnTabExit(TabButton button) {
        ResetTabs(); 
    }

    public void onTabSelected(TabButton button) {

        if (selectedTab == button) {
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
                pageToSwap[i].SetActive(true);
            else
                pageToSwap[i].SetActive(false); 
        }
    }

    public void ResetTabs() {
        foreach (TabButton button in tabButton)
        {
            if (selectedTab != null && button == selectedTab)
                continue; 

            button.background.color = new Color(button.background.color.r, 
                                                button.background.color.g, 
                                                button.background.color.b, 
                                                tabIdle); 
        }
    }
}