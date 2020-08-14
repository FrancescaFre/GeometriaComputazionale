using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    public List<Block> all_blocks;

    public Block block_toInstantiate; 

    public raymarchcamera rm;

    public float precision;
    public float smooth;
    public float max_dist;
    public float max_steps;

    public bool plane = false; 

    void Start()
    {
        rm = GetComponent<raymarchcamera>();
    }

    public void UpdateSettings() {
        rm.UpdateSettings(precision, smooth, max_dist, max_steps, plane);
    }

    public void UpdateScene() {

        rm.UpdateData(all_blocks);
    }



    public void remove_block(int i)
    {
        Block b = all_blocks[i];
        all_blocks.RemoveAt(i); 

        Destroy(b.gameObject);
        UpdateScene();
    }
    public void add_block()
    {
        if (all_blocks.Count <= 9)
        { 
            Block new_block = Instantiate(block_toInstantiate);
            all_blocks.Add(new_block);
            UpdateScene();
        }
    }
}
