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

    /*
     * 
    public void UpdateSceneData(List<Block> blocks)
    {
       shape_and_op_and_select = new Vector4 [10];
        size_auto_morph = new Vector4[10];
        position = new Vector4[10];
        rotation = new Vector4[10];
        color = new Vector4[10];

        for (int i = 0; i < blocks.Count; i++)
        {
            shape_and_op_and_select[i] = new Vector4(blocks[i].shape, blocks[i].op, blocks[i].selected, 0);
            size_auto_morph[i] = new Vector4(blocks[i].size, blocks[i].auto, blocks[i].morph, 0);

            position[i]= blocks[i].position;
            rotation[i] = blocks[i].rotation;
            color[i] = blocks[i].color;
        }
    }
         private Vector4[] shape_and_op_and_select;
    private Vector4[] size_auto_morph;
    private Vector4[] position;
    private Vector4[] rotation;
    private Vector4[] color;

     
     */

}
