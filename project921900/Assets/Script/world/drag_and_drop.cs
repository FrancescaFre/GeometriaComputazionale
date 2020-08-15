using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=0yHBDZHLRbQ
public class drag_and_drop : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    Block block;
    Controller controller; 
    private void Awake()
    {
        block = transform.GetComponent<Block>();
        controller = FindObjectOfType<Controller>(); 
    }

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z; 
        mOffset = gameObject.transform.position - GetMouseWorldPos();
        block.selected = true;
        controller.UpdateScene(); 
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;
        controller.UpdateScene(); 
    }

    private void OnMouseUp()
    {
        block.selected = false;
        controller.UpdateScene(); 
    }

    private Vector3 GetMouseWorldPos() {
        //pixel coordinates(x,y) 
        Vector3 mousePoint = Input.mousePosition;

        //z coordinate of game object on screen 
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint); 
    }
}
