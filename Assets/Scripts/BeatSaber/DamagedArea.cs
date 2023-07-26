using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedArea : MonoBehaviour
{
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("blueCube") || other.gameObject.CompareTag("redCube"))
        {
            Destroy(other.gameObject);
        }
    }
}
