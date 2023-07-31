using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollisionScript : MonoBehaviour
{
    StageManagerScript stageManager;  

    private void Start()
    {
        stageManager = FindObjectOfType<StageManagerScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("collision!");
            stageManager.GoStage(1);
        }
    }
}
