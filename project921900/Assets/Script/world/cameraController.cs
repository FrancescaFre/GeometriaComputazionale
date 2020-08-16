using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://emmaprats.com/p/how-to-rotate-the-camera-around-an-object-in-unity3d/
public class cameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Vector3 target;
    public float distanceToTarget = 10;

    private Vector3 previousPosition;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        target = new Vector3(0,0,0);
    }
    void Update()
    {
        cam.transform.position = target;
        if (Input.GetMouseButtonDown(1))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        
        else if (Input.GetMouseButton(1))
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically

            

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

            

            previousPosition = newPosition;
        }
        cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));
    }
}
