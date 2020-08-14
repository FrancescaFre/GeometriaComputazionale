using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    enum Shape {SPHERE, CUBE, ROUNDEDCUBE, TORUS}
    enum Operator {INTERSECTION, UNION, SUBTRACTION}
    
    public int shape;
    public int op;
    public bool selected;

    public float size;
    public int auto;
    public int morph; //morph with value for the new shape


    public Vector3 position;
    public Vector4 rotation; //bool sugli assi e il quarto valore è il grado di rotazione
    public Color color;

    public SphereCollider collider; 
    
    public void Awake() {
        shape = Random.Range(1, 4);
        op = 0;
        selected = false;
        size = Random.Range(0.5f, 2);
        transform.position = new Vector3(0, 0, 0); 
        collider = GetComponent<SphereCollider>();
    }

    public void Update()
    {
        collider.radius = size+1; 
        position = transform.position;
    }

}
