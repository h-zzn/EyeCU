using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.SceneManagement; 

public class DDATrainer : Agent
{
    private bool isRewarding = false; 

    private SpawnManager spawnManager; 
    private EventManager eventManager; 
    private DamagedArea damagedArea; 
    private ControllerManagerDDA controllerManager; 

    private int MissingPoint; 

    private int OriginStageHP; 
    private int OriginEnemyHP; 

    private float OriginBasicOrbSpeed; 
    private float OriginBasicOrbSpawnInterval; 
    private float OriginSpecialOrbSpeed; 
    private float OriginSpecialOrbSpawnInterval;
    private float OriginStoneSpeed;
    private float OriginStoneSpawnInterval;

    private void Awake()
    {
        eventManager = this.transform.GetComponent<EventManager>(); 
        damagedArea = this.transform.GetComponent<DamagedArea>(); 
        spawnManager = this.GetComponent<SpawnManager>(); 

        MissingPoint = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManagerDDA>().MissingPoint;   
    }

    private void Start()
    {
        StartCoroutine(DecreaseOverTime());
        StartCoroutine(CheckMissingPointChange());

        isRewarding = false;

        OriginStageHP = damagedArea.stageHP;
        OriginEnemyHP = eventManager.EnemyHP;

        OriginBasicOrbSpeed = spawnManager.basicOrbSpeed;
        OriginBasicOrbSpawnInterval = spawnManager.basicOrbSpawnInterval;
        OriginSpecialOrbSpeed = spawnManager.SpecialOrbSpeed;
        OriginSpecialOrbSpawnInterval = spawnManager.SpecialOrbSpawnInterval;
        OriginStoneSpeed = spawnManager.stoneSpeed;
        OriginStoneSpawnInterval = spawnManager.stoneSpawnInterval;
    }

    public override void CollectObservations(VectorSensor sensor)  
    {
        sensor.AddObservation(damagedArea.stageHP); 
        sensor.AddObservation(eventManager.EnemyHP); 
        sensor.AddObservation(MissingPoint); 

        sensor.AddObservation(spawnManager.basicOrbSpeed);   
        sensor.AddObservation(spawnManager.basicOrbSpawnInterval);  
        sensor.AddObservation(spawnManager.SpecialOrbSpeed); 
        sensor.AddObservation(spawnManager.SpecialOrbSpawnInterval);  
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)  
    {
        AddReward(Time.deltaTime);

        if (actionBuffers.ContinuousActions[0] >= 0.5f && actionBuffers.ContinuousActions[0] <= 2f)
        {
            spawnManager.basicOrbSpeed = OriginBasicOrbSpeed * actionBuffers.ContinuousActions[0]; 
            spawnManager.SpecialOrbSpeed = OriginSpecialOrbSpeed * actionBuffers.ContinuousActions[0];
            spawnManager.stoneSpeed = OriginStoneSpeed * actionBuffers.ContinuousActions[0];
        }
        else
            AddReward(-1.0f);

        if (actionBuffers.ContinuousActions[1] >= 0.5f && actionBuffers.ContinuousActions[1] <= 2f)
        {
            spawnManager.basicOrbSpawnInterval = OriginBasicOrbSpawnInterval * actionBuffers.ContinuousActions[1]; 
            spawnManager.SpecialOrbSpawnInterval = OriginSpecialOrbSpawnInterval * actionBuffers.ContinuousActions[1];
            spawnManager.stoneSpawnInterval = OriginStoneSpawnInterval * actionBuffers.ContinuousActions[1]; 
        }
        else
            AddReward(-1.0f); 

        if (damagedArea.stageHP < 1500 && damagedArea.stageHP >= 1000) 
        {
            AddReward(1.0f); 
        }

        if (damagedArea.stageHP >= 1900)
        {
            AddReward(-2.0f); 
        }
        else if (damagedArea.stageHP >= 1500)
        {
            AddReward(-1.0f); 
        }

        if (damagedArea.stageHP <= 100)
        {
            AddReward(-2.0f); 
        }
        else if (damagedArea.stageHP <= 500)
        {
            AddReward(-1.0f);
        }

        EndMLAgent();  
    }

    public void EndMLAgent()
    {
        if (eventManager.GameClear == true)
        {
            ReviewEnding();
            EndEpisode();
        }

        if (damagedArea.stageHP < 0)
        {
            SetReward(-10.0f);
            EndEpisode();
        }
    }

    private void ReviewEnding()
    {
        if (isRewarding == false)  
        {
            if (damagedArea.stageHP <= 200)   
            {
                AddReward(-5000.0f);
            }
            else if (damagedArea.stageHP <= 500)   
            {  
                AddReward(-2000.0f);
            }

            if (damagedArea.stageHP >= 1800)     
            {
                AddReward(-5000.0f);
            }
            else if (damagedArea.stageHP >= 1500)      
            {
                AddReward(-2000.0f);
            }

            if (MissingPoint > spawnManager.totalNumOfBasicOrb / 10)      
            {
                AddReward(2000.0f); 
            }

            if (MissingPoint > spawnManager.totalNumOfBasicOrb / 2)      
            {
                AddReward(-2000.0f);
            }

            if (damagedArea.stageHP > OriginStageHP / 2 && damagedArea.stageHP < 1500)       
            {
                AddReward(10000.0f);
            }

            isRewarding = true;
        }
    }

    private IEnumerator DecreaseOverTime() 
    {
        yield return new WaitForSeconds(90f);

        eventManager.EnemyHP -= (OriginEnemyHP + 500);
    }

    private IEnumerator CheckMissingPointChange() 
    {
        while (true)
        {
            // 처음에 현재의 missingPoint 값을 저장합니다.
            int initialMissingPoint = MissingPoint;

            // 10초를 기다립니다.
            yield return new WaitForSeconds(10f);

            // 10초 후에 현재 missingPoint와 처음에 저장한 값을 비교합니다.
            int change = MissingPoint - initialMissingPoint;

            if (change > 3 && change == 0) 
            {
                AddReward(-1000.0f); 
            }
            else
            {
                AddReward(1000.0f); 
            }

            // 현재 missingPoint 값을 다시 저장합니다.
            initialMissingPoint = MissingPoint; 
        } 
    }
}