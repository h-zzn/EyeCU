using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.SceneManagement; 

public class DDATrainer : Agent
{
    
    private SpawnManager spawnManager;
    private EventManager eventManager; 
    private DamagedArea damagedArea; 

    private int MissingPoint;

    private int OriginStageHP;
    private int OriginEnemyHP;


    private void Awake()
    {
        eventManager = this.transform.GetComponent<EventManager>(); 
        damagedArea = this.transform.GetComponent<DamagedArea>();
        spawnManager = this.GetComponent<SpawnManager>();

        MissingPoint = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>().MissingPoint;

        OriginStageHP = damagedArea.stageHP;
        OriginEnemyHP = eventManager.EnemyHP;
    }

    public override void CollectObservations(VectorSensor sensor)  
    {
        // 정보 수집
        sensor.AddObservation(damagedArea.stageHP);
        sensor.AddObservation(eventManager.EnemyHP);

        sensor.AddObservation(spawnManager.basicOrbSpeed);
        sensor.AddObservation(spawnManager.basicOrbSpawnInterval);
        sensor.AddObservation(spawnManager.stoneSpeed);
        sensor.AddObservation(spawnManager.stoneSpawnInterval);
        sensor.AddObservation(spawnManager.SpecialOrbSpeed);
        sensor.AddObservation(spawnManager.SpecialOrbSpawnInterval);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {    
        spawnManager.basicOrbSpeed = actionBuffers.ContinuousActions[0];
        spawnManager.basicOrbSpawnInterval = actionBuffers.ContinuousActions[1];
        spawnManager.stoneSpeed = actionBuffers.ContinuousActions[2]; 
        spawnManager.stoneSpawnInterval = actionBuffers.ContinuousActions[3];
        spawnManager.SpecialOrbSpeed = actionBuffers.ContinuousActions[4];
        spawnManager.SpecialOrbSpawnInterval = actionBuffers.ContinuousActions[5];
    }

    public void EndMLAgent()
    {
        if (eventManager.GameClear == true)
        {
            ReviewEnding();
            EndEpisode();
        }

        if(damagedArea.stageHP < 0)
        {
            SetReward(-10.0f); 
            EndEpisode(); 
        }
    }

    private void ReviewEnding()
    {
        if (damagedArea.stageHP <= 200)
        {
            AddReward(-5.0f);
        }
        else if(damagedArea.stageHP <= 500)
        {
            AddReward(-2.0f);
        }

        if (damagedArea.stageHP >= 1800)
        {
            AddReward(-5.0f);
        }
        else if(damagedArea.stageHP >= 1500)
        {
            AddReward(-2.0f);
        }

        if(MissingPoint > spawnManager.totalNumOfBasicOrb/10)
        {
            AddReward(2.0f);
        }

        if(MissingPoint > spawnManager.totalNumOfBasicOrb/2)
        {
            AddReward(-2.0f); 
        }

        if (damagedArea.stageHP > OriginStageHP/2 && damagedArea.stageHP < 1500)
        {
            AddReward(10.0f);
        }
    }
}