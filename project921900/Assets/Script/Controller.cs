using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    public List<Block> all_blocks;
    public raymarchcamera rm;

    public float precision;
    public float smooth;
    public float max_dist;
    public float max_steps;

    public bool plane = false; 
    //RaymarchingMaster rm; 
    // Start is called before the first frame update
    void Start()
    {
        rm = GetComponent<raymarchcamera>();
    }

    public void UpdateSettings() {
        rm.UpdateSettings(precision, smooth, max_dist, max_steps, plane);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        all_blocks.Clear();

        foreach (Block block in FindObjectsOfType<Block>())
        {
          
            if (!all_blocks.Contains(block) && all_blocks.Count < 10)
                all_blocks.Add(block);               
        }

        rm.UpdateData(all_blocks); 
    }
}
