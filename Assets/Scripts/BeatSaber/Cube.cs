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
    [SerializeField] private int moveSpeed = 8; 

    void Awake()
    {
        IsHovered = false; 
        originVector = transform.forward;
    }

    // Update is called once per farame
    void Update()
    {
        transform.position += originVector*moveSpeed*Time.deltaTime; 
    }
}
