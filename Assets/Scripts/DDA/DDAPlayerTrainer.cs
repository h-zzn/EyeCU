using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DDAPlayerTrainer : Agent  
{
    private bool isRewarding = false;

    private SpawnManager spawnManager;  
    private EventManager eventManager;  
    private DamagedArea damagedArea;  

    private int OriginStageHP;  
    private int OriginEnemyHP;  

    [SerializeField] private GameObject Eye;

    private EyeTrackingRay eyeTrackingRay;
    private ControllerManagerDDA controllerManager;

    private Transform target;

    GameObject movingCube = null;
    GameObject redCube = null;
    GameObject blueCube = null;


    private void Start()
    {
        StartCoroutine(DecreaseOverTime());

        isRewarding = false;
    }

    private void Awake()  
    {
        eventManager = GameObject.Find("StageCore").GetComponent<EventManager>();  
        damagedArea = GameObject.Find("StageCore").GetComponent<DamagedArea>();  
        spawnManager = GameObject.Find("StageCore").GetComponent<SpawnManager>();  

        eyeTrackingRay = Eye.GetComponent<EyeTrackingRay>();


        controllerManager = this.GetComponent<ControllerManagerDDA>();  


        OriginStageHP = damagedArea.stageHP; 
        OriginEnemyHP = eventManager.EnemyHP; 
    }

    public override void CollectObservations(VectorSensor sensor)   
    {
        // 정보 수집 
        sensor.AddObservation(damagedArea.stageHP);
        sensor.AddObservation(eventManager.EnemyHP);
        sensor.AddObservation(controllerManager.MissingPoint);
        sensor.AddObservation(controllerManager.skillEnergyPoint);

        sensor.AddObservation(Eye.transform.rotation);

        //sensor.AddObservation(spawnManager.basicOrbSpeed);
        //sensor.AddObservation(spawnManager.basicOrbSpawnInterval);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        /*
        if (actionBuffers.ContinuousActions.Length >= 2)
        {
            float horizontalMovement = actionBuffers.ContinuousActions[0]; // -1.0부터 1.0 사이의 값을 받아옴
            float verticalMovement = actionBuffers.ContinuousActions[1]; // -1.0부터 1.0 사이의 값을 받아옴

            // 움직일 각도 범위 설정 (-90도부터 90도로 제한)
            float maxAngle = 90.0f; // 최대 각도
            float xAngle = maxAngle * horizontalMovement; // x 축 각도 계산 (-90도부터 90도)
            float yAngle = maxAngle * verticalMovement; // y 축 각도 계산 (-90도부터 90도)

            // Eye 오브젝트 회전 (x 축과 y 축 주위로 회전)
            Eye.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
        }
        */
        UpdateTarget(); 

        Eye.transform.LookAt(target);
        

        if (eyeTrackingRay.HoveredCube != null)
        {
            // 나머지 게임 논리 및 보상 처리
            if (eyeTrackingRay.HoveredCube.transform.gameObject.CompareTag("redCube"))
            {
                if (actionBuffers.DiscreteActions[1] == 1)
                {
                    controllerManager.rightClicked = true;
                    AddReward(50.0f);
                }
                else
                {
                    controllerManager.rightClicked = false;
                    AddReward(-1.0f);
                }

                if (actionBuffers.DiscreteActions[0] == 1)
                {
                    controllerManager.leftClicked = true;
                    AddReward(-50.0f);
                }
                else
                {
                    controllerManager.leftClicked = false;
                    AddReward(1.0f);
                }
            }

            if (eyeTrackingRay.HoveredCube.transform.gameObject.CompareTag("blueCube"))
            {
                if (actionBuffers.DiscreteActions[0] == 1)
                {
                    controllerManager.leftClicked = true;
                    AddReward(50.0f);
                }
                else
                {
                    controllerManager.leftClicked = false;
                    AddReward(-1.0f);
                }

                if (actionBuffers.DiscreteActions[1] == 1)
                {
                    controllerManager.rightClicked = true;
                    AddReward(-50.0f);
                }
                else
                {
                    controllerManager.rightClicked = false;
                    AddReward(1.0f);
                }
            }

            if(eyeTrackingRay.HoveredCube.transform.gameObject.CompareTag("MovingOrb"))
            {
                AddReward(Time.deltaTime*10f); 
            }
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
            SetReward(-100.0f);
            EndEpisode();
        }
    }

    private void ReviewEnding()
    {
        if (isRewarding == false)
        {
            if (damagedArea.stageHP <= 200)
            {
                AddReward(-800.0f);
            }
            else if (damagedArea.stageHP <= 500)
            {
                AddReward(-500.0f);
            }

            AddReward(-10f * controllerManager.MissingPoint);

            AddReward(damagedArea.stageHP);

            AddReward(controllerManager.skillEnergyPoint/10f);

            if (damagedArea.stageHP == OriginStageHP)
            {
                AddReward(1000f);
            }

            isRewarding = true;
        }
    }


    private IEnumerator DecreaseOverTime()
    {   
        yield return new WaitForSeconds(90f);

        eventManager.EnemyHP -= (OriginEnemyHP+500);
    }

    private void UpdateTarget()
    {   
        if(movingCube != null)
        {
            target = movingCube.transform;
        }
        else if (redCube != null)
        {
            target = redCube.transform;
        }
        else if (blueCube != null)
        {
            target = blueCube.transform;
        }
        else
        {
            movingCube = GameObject.FindGameObjectWithTag("MovingOrb");
            redCube = GameObject.FindGameObjectWithTag("redCube");
            blueCube = GameObject.FindGameObjectWithTag("blueCube");
        }
    }
}