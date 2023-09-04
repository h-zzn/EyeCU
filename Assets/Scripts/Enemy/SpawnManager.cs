using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] basicOrbSpawner; 
    public GameObject[] stoneSpawner; 
    public GameObject[] SpecialOrbSpawner;

    public float basicOrbSpeed = 1; 
    public float basicOrbSpawnBeat = 1; 

    public float stoneSpeed = 1; 
    public float stoneSpawnBeat = 1; 

    public float SpecialOrbSpeed = 1; 
    public float SpecialOrbSpawnInterval = 1; 

    void Awake()
    { 
        foreach (GameObject spawner in basicOrbSpawner) 
        {
            spawner.GetComponent<Spawner>().OrbSpeed = basicOrbSpeed; 
            spawner.GetComponent<Spawner>().beat *= basicOrbSpawnBeat; 
        }

        foreach (GameObject spawner in stoneSpawner) 
        {
            spawner.GetComponent<Spawner>().OrbSpeed = stoneSpeed; 
            spawner.GetComponent<Spawner>().beat *= stoneSpawnBeat; 
        }

        foreach (GameObject spawner in stoneSpawner) 
        {
            spawner.GetComponent<SpecialOrbSpawner>().SpecialOrbSpeed /= SpecialOrbSpeed; 
            spawner.GetComponent<SpecialOrbSpawner>().SpecialOrbInterval = SpecialOrbSpawnInterval; 
        }
    }

    void Update()
    {
        
    }

}
