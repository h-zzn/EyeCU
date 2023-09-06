using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] basicOrbSpawner; 
    public GameObject[] stoneSpawner; 
    public GameObject[] SpecialOrbSpawner;

    public float basicOrbSpeed = 1; 
    public float basicOrbSpawnInterval = 1.735715f; 

    public float stoneSpeed = 1; 
    public float stoneSpawnInterval = 1; 

    public float SpecialOrbSpeed = 1; 
    public float SpecialOrbSpawnInterval = 1; 

    public int totalNumOfBasicOrb = 0;
    

    void Awake()
    {
        SetEnemyComponents();  
    }

    void FixedUpdate()
    {
        SetEnemyComponents();  
    }

    public void SetEnemyComponents()
    {
        int NumOfBasicOrb = 0;
        foreach (GameObject spawner in basicOrbSpawner)
        {
            spawner.GetComponent<Spawner>().OrbSpeed = basicOrbSpeed;
            spawner.GetComponent<Spawner>().Interval = basicOrbSpawnInterval;
            NumOfBasicOrb += spawner.GetComponent<Spawner>().numOfBasicOrb;
        }
        totalNumOfBasicOrb = NumOfBasicOrb;

        foreach (GameObject spawner in stoneSpawner)
        {
            spawner.GetComponent<Spawner>().OrbSpeed = stoneSpeed;
            spawner.GetComponent<Spawner>().Interval = stoneSpawnInterval;
        }

        foreach (GameObject spawner in stoneSpawner) 
        {
            spawner.GetComponent<SpecialOrbSpawner>().SpecialOrbSpeed = SpecialOrbSpeed;
            spawner.GetComponent<SpecialOrbSpawner>().SpecialOrbInterval = SpecialOrbSpawnInterval;
        }
    }

    public void BasicSpawnStop(bool stop)  
    {
        if(basicOrbSpawner.Length != 0)
        {
            foreach (GameObject spawner in basicOrbSpawner)
            {
                spawner.GetComponent<Spawner>().isSpawnStop = stop;
            }
        }
    } 

    public void StoneSpawnStop(bool stop)
    {
        if(stoneSpawner.Length != 0)  
        {
            foreach (GameObject spawner in stoneSpawner)
            {
                spawner.GetComponent<Spawner>().isSpawnStop = stop;
            }
        } 
    } 

    public void SpecialOrbSpawnAllStop(bool stop)
    {
        if(SpecialOrbSpawner.Length != 0)  
        {
            foreach (GameObject spawner in SpecialOrbSpawner)
            {
                spawner.GetComponent<SpecialOrbSpawner>().isSpawnStop = stop;
            }
        } 
    } 
}
