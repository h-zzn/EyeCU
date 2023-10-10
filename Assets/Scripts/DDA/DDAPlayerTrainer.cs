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
    [SerializeField] private float rotationSpeed = 1.0f;

    GameObject movingCube = null;
    GameObject redCube = null;
    GameObject blueCube = null;

    private float DistanceOfBlueCube = 0;
    private float DistanceOfRedCube = 0;
    private float DistanceOfMovingOrb = 0;

    private bool RClicked = false;
    private bool LClicked = false;

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

        sensor.AddObservation(DistanceOfBlueCube); 
        sensor.AddObservation(DistanceOfRedCube); 
        sensor.AddObservation(DistanceOfMovingOrb); 
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        UpdateTarget(actionBuffers.DiscreteActions[2]); 

        if (eyeTrackingRay.HoveredCube != null)
        {
            // 나머지 게임 논리 및 보상 처리
            if (eyeTrackingRay.HoveredCube.transform.gameObject.CompareTag("redCube"))  
            {
                DistanceOfRedCube = Vector3.Distance(eyeTrackingRay.HoveredCube.transform.position, this.transform.position);  

                if (actionBuffers.DiscreteActions[1] == 1)   
                {
                    if (RClicked == false)
                    {
                        RClicked = true;
                        controllerManager.rightClicked = true;
                        //AddReward(50.0f);
                    }
                }
                else
                {
                    controllerManager.rightClicked = false;
                    //AddReward(-1.0f);
                    RClicked = false;
                }

                if (actionBuffers.DiscreteActions[0] == 1)   
                {
                    if (LClicked == false)
                    {
                        LClicked = true;
                        controllerManager.leftClicked = true;
                        //AddReward(-50.0f);
                    }
                }
                else
                {
                    controllerManager.leftClicked = false; 
                    //AddReward(1.0f); 
                    LClicked = false;  
                }
            }

            if (eyeTrackingRay.HoveredCube.transform.gameObject.CompareTag("blueCube"))  
            {
                DistanceOfBlueCube = Vector3.Distance(eyeTrackingRay.HoveredCube.transform.position, this.transform.position);

                if (actionBuffers.DiscreteActions[0] == 1)   
                {
                    if (LClicked == false)
                    {
                        LClicked = true;
                        controllerManager.leftClicked = true;
                        //AddReward(50.0f);
                    }
                }
                else 
                {
                    controllerManager.leftClicked = false;
                    //AddReward(-1.0f);
                    LClicked = false;
                }

                if (actionBuffers.DiscreteActions[1] == 1)   
                {
                    if (RClicked == false)
                    {
                        RClicked = true;
                        controllerManager.rightClicked = true;
                        //AddReward(-50.0f); 
                    }
                }
                else
                {
                    controllerManager.rightClicked = false;
                    //AddReward(1.0f);
                    RClicked = false;
                }
            }

            if(eyeTrackingRay.HoveredCube.transform.gameObject.CompareTag("MovingOrb"))
            {
                DistanceOfMovingOrb = Vector3.Distance(eyeTrackingRay.HoveredCube.transform.position, this.transform.position);

                //AddReward(Time.deltaTime*10f); 
            }
        }

        //EndMLAgent();
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
            else if (damagedArea.stageHP <= 1000)   
            {
                AddReward(-300.0f);
            }

            AddReward(-10f * controllerManager.MissingPoint);  

            AddReward(damagedArea.stageHP);  

            AddReward(controllerManager.skillEnergyPoint/10f);  

            if (damagedArea.stageHP == OriginStageHP)  
            {
                AddReward(10000f);
            }
            else if(damagedArea.stageHP >= 1000)   
            {
                AddReward(3000f);
            }

            isRewarding = true;
        }
    }


    private IEnumerator DecreaseOverTime()   
    {   
        yield return new WaitForSeconds(90f);

        eventManager.EnemyHP -= (OriginEnemyHP+500);
    }

    private void UpdateTarget(int DiscreteActionsNum)   
    {
        if (redCube != null && DiscreteActionsNum == 0)  
        {
            target = redCube.transform;
        }
        else if (blueCube != null && DiscreteActionsNum == 1)   
        {
            target = blueCube.transform;
        }
        else if (movingCube != null && DiscreteActionsNum == 2)   
        {
            target = movingCube.transform; 
        }

        if (redCube == null)
        {
            redCube = GameObject.FindGameObjectWithTag("redCube");
            if(redCube != null)
                DistanceOfRedCube = Vector3.Distance(redCube.transform.position, this.transform.position);
        }

        if (blueCube == null)
        {
            blueCube = GameObject.FindGameObjectWithTag("blueCube");
            if (blueCube != null)
                DistanceOfBlueCube = Vector3.Distance(blueCube.transform.position, this.transform.position);
        }

        if (movingCube == null) 
        {
            movingCube = GameObject.FindGameObjectWithTag("MovingOrb");
            if (movingCube != null)
                DistanceOfMovingOrb = Vector3.Distance(movingCube.transform.position, this.transform.position);
        }

        if (target != null)
        {
            // 대상 방향으로 회전 
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            Eye.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime/10);
        }
    }
}