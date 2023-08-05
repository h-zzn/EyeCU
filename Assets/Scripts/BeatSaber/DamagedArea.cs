using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedArea : MonoBehaviour
{
    [SerializeField] private GameObject EventManager; 
    

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("blueCube") || other.gameObject.CompareTag("redCube") || other.gameObject.CompareTag("LavaStone") || other.gameObject.CompareTag("IceStone"))
        {
            EventManager.GetComponent<stageManager>().stageHP -= 100;
            Destroy(other.gameObject); 
        }
        else if(other.gameObject.CompareTag("MovingOrb"))
        {
            if (other.transform.parent != null)
            {
                EventManager.GetComponent<stageManager>().stageHP -= 500;
                Destroy(other.transform.parent.gameObject); 
            }
        }
    }
}
