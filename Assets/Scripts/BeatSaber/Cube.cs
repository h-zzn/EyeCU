using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Cube : MonoBehaviour
{
    public bool IsHovered { get; set; }

    void Awake()
    {
        IsHovered = false; 
    }

    // Update is called once per farame
    void Update()
    {
        transform.position += transform.forward/8;

        if(!IsHovered)
        {

        }
    }
}
