using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{   
    public GameObject[] basicOrbSpawner;
    public GameObject[] stoneSpawner;       
    public GameObject[] SpecialOrbSpawner;

    public float basicOrbSpeed = 1; 
    public float basicOrbSpawnInterval = 1.735715f; 

    public float stoneSpeed = 1; 
    public float stoneSpawnInterval = 3; 

    public float SpecialOrbSpeed = 1; 
    public float SpecialOrbSpawnInterval = 7.5f;  

    public int totalNumOfBasicOrb = 0;

    public bool activeBasicOrb = false; 
    public bool activeStone = false;
    public bool activeSpecialOrb = false;
    
    public bool activeSkill = false;

    private float OriginBasicOrbSpeed;
    private float OriginSpecialOrbSpeed;


    void Awake()
    {
        SetEnemyComponents();
        OriginBasicOrbSpeed = basicOrbSpeed;
        OriginSpecialOrbSpeed = SpecialOrbSpeed;
    }

    void Update()
    {
        SetEnemyComponents();   
        
        realtimeControllAllSpawner();   
    }

    public void SetEnemyComponents()
    {
        if (activeBasicOrb == true)
        {
            AdjustCubeSpeed("redCube");
            AdjustCubeSpeed("blueCube");
        }

        if (activeStone == true)
        {
            AdjustCubeSpeed("IceStone");
            AdjustCubeSpeed("LavaStone");
        }

        if (activeSpecialOrb == true)
        {
            AdjustMovingOrbSpeed("MovingOrb");
        }

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

        foreach (GameObject spawner in SpecialOrbSpawner) 
        {
            spawner.GetComponent<SpecialOrbSpawner>().SpecialOrbSpeed = SpecialOrbSpeed;
            spawner.GetComponent<SpecialOrbSpawner>().SpecialOrbInterval = SpecialOrbSpawnInterval;
        }
    }

    void AdjustCubeSpeed(string cubeTag)
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag(cubeTag);

        foreach (GameObject cubeObject in cubes)
        {
            Cube cubeComponent = cubeObject.GetComponent<Cube>();

            if (cubeComponent != null)
            {
                cubeComponent.moveSpeed = 5 * basicOrbSpeed; 
            }
        }
    }

    void AdjustMovingOrbSpeed(string orbTag)
    {
        GameObject[] orbs = GameObject.FindGameObjectsWithTag(orbTag);

        foreach (GameObject orbObject in orbs)
        {
            Tracing tracingComponent = orbObject.GetComponent<Tracing>();

            if (tracingComponent != null)
            {
                tracingComponent.movingTime = 3 / SpecialOrbSpeed; 
            }
        }
    }

    GameObject FindCubeWithTag(string cubeTag)
    {
        return GameObject.FindWithTag(cubeTag);
    }

    private void realtimeControllAllSpawner()
    {
        if(activeSkill == false)
        {
            if(activeBasicOrb == true)
            {
                BasicSpawnStop(false); 
            }
            else
            {
                BasicSpawnStop(true); 
            }

            if(activeStone == true)
            {
                StoneSpawnStop(false); 
            }
            else
            {
                StoneSpawnStop(true); 
            }

            if(activeSpecialOrb == true)
            {
                SpecialOrbSpawnAllStop(false); 
            }
            else
            {
                SpecialOrbSpawnAllStop(true); 
            }
        }
        else
        {
            BasicSpawnStop(true); 
            StoneSpawnStop(true); 
            SpecialOrbSpawnAllStop(true); 
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
