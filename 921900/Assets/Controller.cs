using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    private List<Block> all_blocks; 

    //RaymarchingMaster rm; 
    // Start is called before the first frame update
    void Start()
    {
        
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

        //rm.UpdateSceneData(all_blocks); 
    }
}
