using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.SceneManagement;
using System.Linq;

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

    private bool isStartChangeDiff = false;
    private float LevelPoint;  
    private float stackOfPenalty = 0;  
    private int initialDiff = 0;  //1: Easy, 2: ��ǥ, 3:Hard
    private float punishmentPoint = 0;
    private float targetedlevelPoint = 0;

    private int HPChange = 0;
    private List<int> readUsersDiff = new List<int>();
    [SerializeField] private bool isTargetedlevel = false;

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

    // ������Ʈ�� ȯ�濡�� �����ϴ� ������ ����    
    public override void CollectObservations(VectorSensor sensor)  
    {
        sensor.AddObservation(damagedArea.stageHP);   
        sensor.AddObservation(eventManager.EnemyHP);    

        sensor.AddObservation(spawnManager.basicOrbSpeed);    
        sensor.AddObservation(spawnManager.basicOrbSpawnInterval);    
        sensor.AddObservation(spawnManager.SpecialOrbSpeed);    
        sensor.AddObservation(spawnManager.SpecialOrbSpawnInterval);   
         
        sensor.AddObservation(isTargetedlevel);    
        sensor.AddObservation(HPChange);   

        sensor.AddObservation(punishmentPoint);  
        sensor.AddObservation(targetedlevelPoint);  
    }

    // ������Ʈ�� �ൿ�� ������ �� ȣ��Ǵ� �޼���    
    public override void OnActionReceived(ActionBuffers actionBuffers)  
    {
        targetedlevelPoint = PlayerPrefs.GetFloat("SavedLevel", 1.0f);
 
        if(spawnManager.activeSpecialOrb == true || spawnManager.activeSkill == true) 
        {  
            LevelPoint = targetedlevelPoint; 
        }
        else if (isStartChangeDiff == true)
        {
            //LevelPoint ��ȭ
            LevelPoint = actionBuffers.ContinuousActions[0]*2;
            RewardingByDiff();
        }
        else
        {
            LevelPoint = targetedlevelPoint; 
        }

        //���̵� ����
        changeDiff(); 
            
        // ���� ���̵� ����
        initialLevelPoint = LevelPoint;
        // �н� ���� ���� Ȯ�� 
        EndMLAgent();
    }

    private void changeDiff()
    {
        if (LevelPoint >= 0.5f && LevelPoint <= 1.5f)
        {
            // �ӵ� ����   
            spawnManager.basicOrbSpeed = OriginBasicOrbSpeed * LevelPoint;
            spawnManager.SpecialOrbSpeed = OriginSpecialOrbSpeed * LevelPoint; 
            spawnManager.stoneSpeed = OriginStoneSpeed * LevelPoint;

            // ���� ���� ����   
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
        //��ȭ���� ���� �г�Ƽ
        punishmentPoint = -10 * Mathf.Pow(LevelPoint - targetedlevelPoint, 2);    
        AddReward(punishmentPoint);  
        stackOfPenalty += punishmentPoint;

        //targetedlevelPoint�� ������ ������ ���� [���� ���]
        if (Mathf.Abs(initialLevelPoint - targetedlevelPoint) > Mathf.Abs(LevelPoint - targetedlevelPoint))
        {
            AddReward(-punishmentPoint/2);
        }

        // ����� ���̵������� ���� �� ó��      
        if (isHardDif == true)
        {
            Debug.Log("�����");     
            //easy to hard ���Ƽ ����
            if (initialDiff == 1)
                stackOfPenalty = 0; 
            initialDiff = 3; 
        }
        else if (isEasyDif == true) // ���� ���̵������� ���� �� ó��       
        {
            Debug.Log("����");  
            //hard to easy ���Ƽ ����
            if (initialDiff == 3)
                stackOfPenalty = 0;
            initialDiff = 1;
        }
        else if(isTargetedlevel == true)//�߰� ���̵� ���� 
        {
            Debug.Log("������");
            if (initialDiff == 1 || initialDiff == 3)
            {
                AddReward(-stackOfPenalty);
                stackOfPenalty = 0;
            }
            initialDiff = 2;

            PlayerPrefs.SetFloat("SavedLevel", LevelPoint);
            AddReward(200);   
        }
    }

    // �н� ���� ���� Ȯ�� �� ó��
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

    // �н� ���� �� ������ ���� �� ó�� ����     
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

            isRewarding = true;
        }
    }

    // �ð��� ���� �̺�Ʈ ó��    
    private IEnumerator DecreaseOverTime()      
    {
        yield return new WaitForSeconds(120f);    
        PlayerPrefs.Save();    
        eventManager.EnemyHP -= (OriginEnemyHP*2);    
    }

    // ���� �ð� �������� ä�� ���̵� �ľ�
    private IEnumerator CheckMissingPointChange()    
    {
        yield return new WaitForSeconds(20f);     
        isStartChangeDiff = true;    

        while (true)                
        {  
            // ó���� ������ MissingPoint ���� ����  
            int initialMissingPoint = MissingPoint;
            // ó���� ������ stageHP ���� ����  
            int initialStageHP = damagedArea.stageHP;

            //2�� ��ٸ�
            yield return new WaitForSeconds(2f); 

            //2�� �Ŀ� ���� MissingPoint�� ó���� ������ ���� �� 
            HPChange = initialStageHP - damagedArea.stageHP;
            
            //���� �������� 10�� �� �����θ� �Ǵ�
            if(readUsersDiff.Count == 5)
                readUsersDiff.RemoveAt(0);
            readUsersDiff.Add(HPChange); 

            int sumOfChange = readUsersDiff.Sum(); 

            if (sumOfChange > 100)    //�������� ū �����̸� ����� ���� 
            {
                isHardDif = true; 
                isEasyDif = false;

                isTargetedlevel = false;
            }
            else if (sumOfChange == 0) //�������� ���� �����̸� ���� ����  
            {
                isHardDif = false; 
                isEasyDif = true;

                isTargetedlevel = false;
            }
            else  //�� ��~�� �� ���� �� 
            {
                isHardDif = false;
                isEasyDif = false;

                isTargetedlevel = true;
            }  

            // ���� MissingPoint ���� �ٽ� ����
            initialMissingPoint = MissingPoint;
        }
    }
}
