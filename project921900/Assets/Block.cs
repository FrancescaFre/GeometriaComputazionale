using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    enum Shape {SPHERE, CUBE, ROUNDEDCUBE, TORUS}
    enum Operator {INTERSECTION, UNION, SUBTRACTION}
    
    public int shape;
    public int op;
    public int selected;

    public float size;
    public int auto;
    public int morph; //morph with value for the new shape


    public Vector3 position;
    public Vector4 rotation; //bool sugli assi e il quarto valore è il grado di rotazione
    public Color color;

}
