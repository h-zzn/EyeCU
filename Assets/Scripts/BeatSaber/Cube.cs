using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Cube : MonoBehaviour
{
    public bool IsHovered { get; set; }
    private Vector3 originVector;

    void Awake()
    {
        IsHovered = false; 
        originVector = transform.forward;
    }

    // Update is called once per farame
    void Update()
    {
        transform.position += originVector/8;
        if(!IsHovered)
        {

        }
    }
}
