using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedArea : MonoBehaviour
{
    void Update()
    {
        
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("blueCube") || other.gameObject.CompareTag("redCube") || other.gameObject.CompareTag("LavaStone") || other.gameObject.CompareTag("IceStone"))
        {
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("MovingOrb"))
        {
            if (other.transform.parent != null)
            {
                Destroy(other.transform.parent.gameObject);
            }
        }
    }
}
