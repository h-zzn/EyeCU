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


    private void Start()
    {
        StartCoroutine(DecreaseOverTime());

        isRewarding = false;
    }

    private void Awake()
    {
        eventManager = this.transform.GetComponent<EventManager>(); 
        damagedArea = this.transform.GetComponent<DamagedArea>();
        spawnManager = this.GetComponent<SpawnManager>();

        MissingPoint = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManagerDDA>().MissingPoint;

        OriginStageHP = damagedArea.stageHP;
        OriginEnemyHP = eventManager.EnemyHP;

        OriginBasicOrbSpeed = spawnManager.basicOrbSpeed;
        OriginBasicOrbSpawnInterval = spawnManager.basicOrbSpawnInterval;
        OriginSpecialOrbSpeed = spawnManager.SpecialOrbSpeed;
        OriginSpecialOrbSpawnInterval = spawnManager.SpecialOrbSpawnInterval;
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

        if(actionBuffers.ContinuousActions[0] >= 0.1 && actionBuffers.ContinuousActions[0] <= 1.3)
            spawnManager.basicOrbSpeed = OriginBasicOrbSpeed*actionBuffers.ContinuousActions[0];

        if(actionBuffers.ContinuousActions[1] >= 0.5 && actionBuffers.ContinuousActions[1] <= 3)
            spawnManager.basicOrbSpawnInterval = OriginBasicOrbSpawnInterval*actionBuffers.ContinuousActions[1];

        if(actionBuffers.ContinuousActions[2] >= 0.1 && actionBuffers.ContinuousActions[2] <= 1.3)
            spawnManager.SpecialOrbSpeed = OriginSpecialOrbSpeed*actionBuffers.ContinuousActions[2];

        if(actionBuffers.ContinuousActions[3] >= 0.5 && actionBuffers.ContinuousActions[3] <= 3)
            spawnManager.SpecialOrbSpawnInterval = OriginSpecialOrbSpawnInterval*actionBuffers.ContinuousActions[3];

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
                AddReward(-500.0f);
            }
            else if (damagedArea.stageHP <= 500)
            {
                AddReward(-200.0f);
            }

            if (damagedArea.stageHP >= 1800)
            {
                AddReward(-500.0f);
            }
            else if (damagedArea.stageHP >= 1500)
            {
                AddReward(-200.0f);
            }

            if (MissingPoint > spawnManager.totalNumOfBasicOrb / 10)
            {
                AddReward(200.0f);
            }

            if (MissingPoint > spawnManager.totalNumOfBasicOrb / 2)
            {
                AddReward(-200.0f);
            }

            if (damagedArea.stageHP > OriginStageHP / 2 && damagedArea.stageHP < 1500)
            {
                AddReward(1000.0f);
            }

            isRewarding = true;
        }
    }

    private IEnumerator DecreaseOverTime()
    {
        yield return new WaitForSeconds(90f);

        eventManager.EnemyHP -= (OriginEnemyHP + 500);
    }
}