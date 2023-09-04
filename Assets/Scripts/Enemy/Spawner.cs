using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool isSpawnStop = true;

    public GameObject[] cubes; 
    public Transform[] points; 
    public float beat = (60/130)*2; 
    private float coolTime = 0;  

    public float OrbSpeed = 1;  

    private void Start()
    {
        foreach (GameObject spawnObject in cubes) 
        {
            spawnObject.GetComponent<Cube>().moveSpeed *= OrbSpeed; 
        }   
    }

    void Update()
    {
        if(!isSpawnStop)
        {
            spawnOrb(); 
        }   
    }

    public void spawnOrb()
    {
        if(coolTime > beat) 
        {
            GameObject cube = Instantiate(cubes[Random.Range(0,cubes.Length)],points[Random.Range(0,points.Length)]);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.Rotate(transform.forward);
            coolTime = 0;
        }       
        coolTime += Time.deltaTime;
    }
}
