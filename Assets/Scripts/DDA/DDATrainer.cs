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

    private float initialLevelPoint = 1;

    [SerializeField] private bool isHardDif = false;
    [SerializeField] private bool isEasyDif = false;

    private float LevelPoint;

    private void Awake()  
    {
        eventManager = this.transform.GetComponent<EventManager>(); 
        damagedArea = this.transform.GetComponent<DamagedArea>();
        spawnManager = this.GetComponent<SpawnManager>();
        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManagerDDA>();

        MissingPoint = controllerManager.MissingPoint;
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

    // 에이전트가 환경에서 관찰하는 데이터 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(damagedArea.stageHP);
        sensor.AddObservation(eventManager.EnemyHP);
        sensor.AddObservation(MissingPoint);

        sensor.AddObservation(spawnManager.basicOrbSpeed);
        sensor.AddObservation(spawnManager.basicOrbSpawnInterval);
        sensor.AddObservation(spawnManager.SpecialOrbSpeed);
        sensor.AddObservation(spawnManager.SpecialOrbSpawnInterval);

        sensor.AddObservation(isHardDif);
        sensor.AddObservation(isEasyDif);

        sensor.AddObservation(initialLevelPoint);
    }

    // 에이전트가 행동을 수행할 때 호출되는 메서드
    public override void OnActionReceived(ActionBuffers actionBuffers)  
    {
        //플레이 시간에 따른 보상 (최대한 클리어하게 유도)
        AddReward(Time.deltaTime);

        //난이도 수정에 관한 의사 결정
        if(actionBuffers.DiscreteActions[0] == 1)
        {
            LevelPoint = actionBuffers.ContinuousActions[0]*2;

            if (LevelPoint >= 0.5f && LevelPoint <= 1.5f)  
            { 
                // 속도 조절   
                spawnManager.basicOrbSpeed = OriginBasicOrbSpeed * LevelPoint;   
                spawnManager.SpecialOrbSpeed = OriginSpecialOrbSpeed * LevelPoint;   
                spawnManager.stoneSpeed = OriginStoneSpeed * LevelPoint;   

                // 생성 간격 조절   
                spawnManager.basicOrbSpawnInterval = OriginBasicOrbSpawnInterval * (2f - LevelPoint);    
                spawnManager.SpecialOrbSpawnInterval = OriginSpecialOrbSpawnInterval * (2f - LevelPoint);    
                spawnManager.stoneSpawnInterval = OriginStoneSpawnInterval * (2f - LevelPoint);     
            } 
            else
                AddReward(-1.0f); 
        }

        // 어려운 난이도에서의 보상 및 처벌   
        if (isHardDif == true)
        {
            if (initialLevelPoint > LevelPoint) 
            {
                AddReward(10.0f);
            }
            else 
            {
                AddReward(-10.0f);
            }

            if (actionBuffers.DiscreteActions[0] == 1) 
            {
                AddReward(5.0f);
            }
            else 
            {
                AddReward(-5.0f);
            }
        }
        else if (isEasyDif == true) // 쉬운 난이도에서의 보상 및 처벌
        {
            if (initialLevelPoint > LevelPoint) 
            {
                AddReward(-10.0f);
            }
            else 
            {
                AddReward(10.0f);
            }

            if (actionBuffers.DiscreteActions[0] == 1) 
            {
                AddReward(5.0f); 
            }
            else
            {
                AddReward(-5.0f);
            }
        }
        else
        {
            if (actionBuffers.DiscreteActions[0] == 0) 
            {
                AddReward(20.0f); 
            }
        }
        // 현재 행동 기록
        initialLevelPoint = actionBuffers.ContinuousActions[0];

        // 학습 종료 여부 확인
        EndMLAgent();
    }

    // 학습 종료 조건 확인 및 처리
    public void EndMLAgent() 
    {
        if (eventManager.GameClear == true)
        {
            ReviewEnding();
            EndEpisode();
        }

        if (damagedArea.stageHP < 0)
        {
            SetReward(-10000.0f);
            EndEpisode();
        }
    }

    // 학습 종료 시 마무리 작업 수행     
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

            if (damagedArea.stageHP > 700 && damagedArea.stageHP < 1500)
            {
                AddReward(10000.0f);
            }

            isRewarding = true;
        }
    }

    // 시간에 따른 이벤트 처리
    private IEnumerator DecreaseOverTime()
    {
        yield return new WaitForSeconds(90f);
        eventManager.EnemyHP -= (OriginEnemyHP + 500);
    }

    // 일정 시간 간격으로 채감 난이도 파악
    private IEnumerator CheckMissingPointChange()
    {
        yield return new WaitForSeconds(20f);

        while (true)  
        { 
            // 처음에 현재의 MissingPoint 값을 저장  
            int initialMissingPoint = MissingPoint;
            // 처음에 현재의 stageHP 값을 저장  
            int initialStageHP = damagedArea.stageHP;

            // 5초 기다림
            yield return new WaitForSeconds(5f);

            // 5초 후에 현재 MissingPoint와 처음에 저장한 값을 비교 
            int change = MissingPoint - initialMissingPoint;
            int change2 = initialStageHP - damagedArea.stageHP;

            if (change2 >= 150)   //데미지가 큰 상태이면 어려운 상태 
            {
                AddReward(-500.0f);
                isHardDif = true;
                isEasyDif = false;
            }
            else if (change2 == 0) //실수도 없고 데미지도 없는 상태이면 쉬운 상태 
            {
                AddReward(-500.0f);
                isHardDif = false;
                isEasyDif = true;
            }
            else  
            {
                AddReward(500.0f);
                isHardDif = false;
                isEasyDif = false;   
            }  

            if(change > 5)
            {
                AddReward(-500.0f);
            }

            if (change2 > 500)
            {
                AddReward(-change2*10);
            }


            Debug.Log("isHardDif: " + isHardDif + "/ isEasyDif: " + isEasyDif);

            // 현재 MissingPoint 값을 다시 저장
            initialMissingPoint = MissingPoint;
        }
    }
}
