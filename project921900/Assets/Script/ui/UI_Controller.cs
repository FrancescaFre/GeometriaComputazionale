﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{

    Controller controller;
    TabAreaGroup tabs;
    cameraController cam; 
    
    GameObject activePage;
    int activeObj; 

    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<Controller>();
        tabs = FindObjectOfType<TabAreaGroup>();
        cam = FindObjectOfType<cameraController>(); 
    }

    #region World Settings
    public void Set_camDistance(float value) {
        cam.distanceToTarget = value; 
    }
    public void Set_precision(float value)
    {
        controller.precision = value;
        Debug.Log(value); 

        controller.UpdateSettings(); 
    }

    public void Set_maxDist(float value)
    {
        controller.max_dist = value;

        controller.UpdateSettings();
    }

    public void Set_maxSteps(float value)
    {
        controller.max_steps = value;

        controller.UpdateSettings();
    }

    public void Set_smoothness(float value)
    {
        controller.smooth = value;

        controller.UpdateSettings();
    }

    public void Set_plane() {
        controller.plane = !controller.plane;
  
        controller.UpdateSettings();
    }
    #endregion


    #region Block Settings

    public void ADD_REMOVE()
    { 

        Text text = GameObject.Find("AddRemove").GetComponentInChildren<Text>();

        if (text.text == "ADD")
        {
            controller.add_block();
            tabs.ResetTabs();
         
        }
        else
        {
            Debug.Log("del");
            controller.remove_block(activeObj);
            tabs.ResetTabs();
          
        }
        checkButton();
    }

    public void checkButton()
    {
        Text text = GameObject.Find("AddRemove").GetComponentInChildren<Text>();
       
        if (text.text == "ADD" && controller.all_blocks.Count == activeObj+1)
        {
            text.text = "REMOVE";
            activePage.GetComponent<Image>().color = (controller.all_blocks[activeObj].color + (new Color(0.5f, 0.5f, 0.5f, 0.0f))) * 0.6f;
        }
        else if (text.text == "REMOVE" && controller.all_blocks.Count == activeObj)
        {
            text.text = "ADD";
            activePage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f);
        }
    }

    public void Set_shape(int value) {
        int x = value + 1;
        controller.all_blocks[activeObj].shape = x;

        controller.UpdateScene();
    }

    public void Set_operator(int value)
    {
        int x = value;
        controller.all_blocks[activeObj].op = x;

        controller.UpdateScene();
    }

    public void Set_selector()
    {
        controller.all_blocks[activeObj].selected = !controller.all_blocks[activeObj].selected;

        controller.UpdateScene();
    }

    public void Set_size(float z)
    {
        controller.all_blocks[activeObj].size = z;

        controller.UpdateScene();
    }

    public void Set_zPos(float z)
    {
        float actualz = controller.all_blocks[activeObj].transform.position.z;
        z = Mathf.Clamp(actualz - z, -10, +10);
        controller.all_blocks[activeObj].transform.position = new Vector3(controller.all_blocks[activeObj].transform.position.x, controller.all_blocks[activeObj].transform.position.y, z); 

        controller.UpdateScene();
    }

    public void Set_rotationAngle(float value)
    {
        Vector4 new_rot = controller.all_blocks[activeObj].rotation;
        new_rot.w = value;
        controller.all_blocks[activeObj].rotation = new_rot;

        controller.UpdateScene();
    }

    public void Set_rotationAxis(int a)
    {
        float w = controller.all_blocks[activeObj].rotation.w;
        
        switch (a)
        {
           case 0:
                controller.all_blocks[activeObj].rotation = new Vector4(0f, 1f, 1f, w);
                break;
            case 1:
                controller.all_blocks[activeObj].rotation = new Vector4(1f, 0f, 1f, w);
                break;
            case 2:
                controller.all_blocks[activeObj].rotation = new Vector4(1f, 1f, 0f, w);
                break;
        }
        controller.UpdateScene();
    }

    public void Set_Color(Color c)
    {

        controller.UpdateScene();
        tabs.ResetTabs();
        if (activeObj+1 <= controller.all_blocks.Count)
        {
            controller.all_blocks[activeObj].color = c;
            activePage.GetComponent<Image>().color = (controller.all_blocks[activeObj].color + (new Color(0.5f, 0.5f, 0.5f, 0f))) * 0.6f ;
        }
    }
    #endregion

    #region Active tab
    public void Set_ActiveTab(GameObject active, int i) {
        activePage = active;
        activeObj = i;
        if (i < controller.all_blocks.Count)
            activePage.GetComponent<Image>().color = (controller.all_blocks[activeObj].color + (new Color(0.5f, 0.5f, 0.5f, 0f))) * 0.6f;



        checkButton(); 
    }
    #endregion
}

