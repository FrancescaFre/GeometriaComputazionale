using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    raymarchcamera rm;

    Controller controller;


    // Start is called before the first frame update
    void Start()
    {
        rm = FindObjectOfType<raymarchcamera>();
        controller = FindObjectOfType<Controller>();
    }

    #region World Settings
    public void Set_precision(float value)
    {
        controller.precision = value;
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

    #region block Settings
    public void Set_shape(int value) {
        int x = value + 1; 
    }

    public void Set_operator(int value)
    {
        int x = value; 
    }

    public void Set_selector()
    { 
        //selection = !selection; 
    }

    public void Set_size(float z)
    { }

    public void Set_zPos(float z)
    { 
        //pos = Vector3(pos.xy, z); 
    }

    public void Set_rotationAngle(float a)
    { 
    
    }

    public void Set_rotationAxis(int value)
    {


    }

    #endregion

}

