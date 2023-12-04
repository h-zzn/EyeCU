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
    private ControllerManager controllerManager;   

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

    private bool isStartChangeDiff = false;
    private float LevelPoint;  
    private float stackOfPenalty = 0;  
    private int initialDiff = 0;  //1: Easy, 2: 목표, 3:Hard
    private float punishmentPoint = 0;
    private float targetedlevelPoint = 0;

    private void Awake()    
    {  
        eventManager = this.transform.GetComponent<EventManager>(); 
        damagedArea = this.transform.GetComponent<DamagedArea>(); 
        spawnManager = this.GetComponent<SpawnManager>(); 
        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>(); 

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

        sensor.AddObservation(spawnManager.basicOrbSpeed);    
        sensor.AddObservation(spawnManager.basicOrbSpawnInterval);    
        sensor.AddObservation(spawnManager.SpecialOrbSpeed);    
        sensor.AddObservation(spawnManager.SpecialOrbSpawnInterval);   
         
        sensor.AddObservation(isHardDif);  
        sensor.AddObservation(isEasyDif);  

        sensor.AddObservation(punishmentPoint);  
        sensor.AddObservation(targetedlevelPoint);  
    }

    // 에이전트가 행동을 수행할 때 호출되는 메서드    
    public override void OnActionReceived(ActionBuffers actionBuffers)  
    {
        targetedlevelPoint = PlayerPrefs.GetFloat("SavedLevel", 1.0f);
 
        if(spawnManager.activeSpecialOrb == true || spawnManager.activeSkill == true) 
        {  
            LevelPoint = targetedlevelPoint; 
        }
        else if (isStartChangeDiff == true)
        {
            //LevelPoint 변화
            LevelPoint = actionBuffers.ContinuousActions[0]*2;
            RewardingByDiff();
        }
        else
        {
            LevelPoint = targetedlevelPoint; 
        }

        //난이도 조절
        changeDiff(); 
            
        // 현재 난이도 저장
        initialLevelPoint = LevelPoint;
        // 학습 종료 여부 확인 
        EndMLAgent();
    }

    private void changeDiff()
    {
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
        {
            AddReward(punishmentPoint); 
        }
    }

    private void RewardingByDiff()
    {
        //변화값에 따른 패널티 
        punishmentPoint = -10 * Mathf.Pow(LevelPoint - targetedlevelPoint, 2);    
        AddReward(punishmentPoint);  
        stackOfPenalty += punishmentPoint;   

        // 어려운 난이도에서의 보상 및 처벌      
        if (isHardDif == true)
        {
            Debug.Log("어려워");     
            if (initialLevelPoint > LevelPoint)
            {
                AddReward(punishmentPoint/2); 
            }
            else
            {
                AddReward(-punishmentPoint/2); 
            }

            //easy to hard 페널티 감당
            if (initialDiff == 1)
                stackOfPenalty = 0; 
            initialDiff = 3; 
        }
        else if (isEasyDif == true) // 쉬운 난이도에서의 보상 및 처벌       
        {
            Debug.Log("쉬워");  
            if (initialLevelPoint < LevelPoint)
            {
                AddReward(punishmentPoint / 2 * 1.5f);
            }
            else
            {
                AddReward(-punishmentPoint / 2 * 1.5f);
            }

            //hard to easy 페널티 감당
            if (initialDiff == 3)
                stackOfPenalty = 0;
            initialDiff = 1;
        }
        else //중간 난이도 보상 
        {
            Debug.Log("적당해");
            if (initialDiff == 1 || initialDiff == 3)
            {
                AddReward(-stackOfPenalty);
                stackOfPenalty = 0;
            }
            initialDiff = 2;

            PlayerPrefs.SetFloat("SavedLevel", LevelPoint);
            AddReward(100); 
        }
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

    // 학습 종료 시 마무리 보상 및 처벌 수행     
    private void ReviewEnding()
    {
        if (isRewarding == false)
        {
            if (damagedArea.stageHP <= 200)
            {
                AddReward(-50000.0f);
            }
            else if (damagedArea.stageHP <= 500)
            {
                AddReward(-20000.0f);
            }

            if (damagedArea.stageHP >= 1700)
            {
                AddReward(-50000.0f);
            }
            else if (damagedArea.stageHP >= 1300) 
            {
                AddReward(-20000.0f);
            }

            if (damagedArea.stageHP > 500 && damagedArea.stageHP < 1300)
            {
                AddReward(100000.0f);
            }

            if (MissingPoint > spawnManager.totalNumOfBasicOrb / 2)
            {
                AddReward(-20000.0f);
            }
            isRewarding = true;
        }
    }

    // 시간에 따른 이벤트 처리
    private IEnumerator DecreaseOverTime()  
    {
        yield return new WaitForSeconds(90f);
        PlayerPrefs.Save();  
        eventManager.EnemyHP -= (OriginEnemyHP*2); 
    }

    // 일정 시간 간격으로 채감 난이도 파악
    private IEnumerator CheckMissingPointChange()    
    {
        yield return new WaitForSeconds(20f);    
        isStartChangeDiff = true;  

        while (true)   
        {  
            // 처음에 현재의 MissingPoint 값을 저장  
            int initialMissingPoint = MissingPoint;
            // 처음에 현재의 stageHP 값을 저장  
            int initialStageHP = damagedArea.stageHP;

            // 5초 기다림
            yield return new WaitForSeconds(5f);

            // 5초 후에 현재 MissingPoint와 처음에 저장한 값을 비교 
            int change2 = initialStageHP - damagedArea.stageHP;

            if (change2 >= 100)   //데미지가 큰 상태이면 어려운 상태 
            {
                AddReward(-1000.0f);
                isHardDif = true; 
                isEasyDif = false; 
            }
            else if (change2 == 0) //데미지기 없는 상태이면 쉬운 상태  
            {
                AddReward(-1000.0f); 
                isHardDif = false; 
                isEasyDif = true; 
            }
            else  //한 대 맞을 때 
            {
                AddReward(1000.0f);
                isHardDif = false;
                isEasyDif = false;   
            }  

            // 현재 MissingPoint 값을 다시 저장
            initialMissingPoint = MissingPoint;
        }
    }
}
