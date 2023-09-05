using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DDATrainer : Agent
{
    
    private SpawnManager spawnManager;
    private EventManager eventManager; 
    private DamagedArea damagedArea;

    void Awake()
    {
        eventManager = this.transform.GetComponent<EventManager>(); 
        damagedArea = this.transform.GetComponent<DamagedArea>();
        spawnManager = this.GetComponent<SpawnManager>();
    }

    public override void OnEpisodeBegin()
    {
    	
    }

    public override void CollectObservations(VectorSensor sensor)  
    {
        // 정보 수집
        sensor.AddObservation(1);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {    
        spawnManager.basicOrbSpeed = actionBuffers.ContinuousActions[0];
        spawnManager.basicOrbSpawnInterval = actionBuffers.ContinuousActions[1];
        spawnManager.stoneSpeed = actionBuffers.ContinuousActions[2];
        spawnManager.stoneSpawnInterval = actionBuffers.ContinuousActions[3];
        spawnManager.SpecialOrbSpeed = actionBuffers.ContinuousActions[4];
        spawnManager.SpecialOrbSpawnInterval = actionBuffers.ContinuousActions[5];

        if (damagedArea.stageHP > 1000 && eventManager.GameClear == true)
        { 
            SetReward(1.0f);
            EndEpisode();
        }
        else if (damagedArea.stageHP < 0 || eventManager.GameClear == true)
        {
            EndEpisode(); 
        }
        // 플랫폼 밖으로 나가면 Episode 종료

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}