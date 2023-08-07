using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedArea : MonoBehaviour
{
    public int stageHP = 1000; 


    void Awake()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("blueCube") || other.gameObject.CompareTag("redCube") || other.gameObject.CompareTag("LavaStone") || other.gameObject.CompareTag("IceStone"))
        {
            stageHP -= 100;
            Destroy(other.gameObject); 
        }
        else if(other.gameObject.CompareTag("MovingOrb"))
        {
            if (other.transform.parent != null)
            {
                stageHP -= 500;
                Destroy(other.transform.parent.gameObject); 
            }
        }
    }
}
