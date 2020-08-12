using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=0yHBDZHLRbQ
public class drag_and_drop : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    Block block; 
    private void Awake()
    {
         block = transform.GetComponent<Block>(); 
    }
    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z; 
        mOffset = gameObject.transform.position - GetMouseWorldPos();
        block.selected = 1;
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;
    }

    private void OnMouseUp()
    {
        block.selected = 0; 
    }

    private Vector3 GetMouseWorldPos() {
        //pixel coordinates(x,y) 
        Vector3 mousePoint = Input.mousePosition;

        //z coordinate of game object on screen 
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint); 
    }
}
