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

    private string orbsTag = "Orbs";
    private string spawnerTag = "Enemy";
    private int maxOrbs = 3;

    private GameObject[] orbsArray = new GameObject[3];
    private Vector3[] orbsPositions = new Vector3[3];

    private float DistanceOfBlueCube = 0;
    private float DistanceOfRedCube = 0;
    private float DistanceOfMovingOrb = 0;

    private bool RClicked = false;
    private bool LClicked = false;

    private void Start()
    {
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

        // 최초에 Orbs를 찾아서 배열에 추가
        FindAndAddOrbs();
    }

    public override void CollectObservations(VectorSensor sensor)   
    {
        // 정보 수집 
        sensor.AddObservation(damagedArea.stageHP);
        sensor.AddObservation(eventManager.EnemyHP);
        sensor.AddObservation(controllerManager.MissingPoint);
        sensor.AddObservation(controllerManager.skillEnergyPoint);

        sensor.AddObservation(Eye.transform.rotation);

        sensor.AddObservation(orbsPositions[0]); 
        sensor.AddObservation(orbsPositions[1]); 
        sensor.AddObservation(orbsPositions[2]);

        sensor.AddObservation(FindClosestOrbIndex());
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

                    //가장 가까운 오브를 바라볼 때 보상 2배 제시
                    if (actionBuffers.DiscreteActions[2] == FindClosestOrbIndex())
                    {
                        //AddReward(50f);
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
                        //AddReward(-500.0f);
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

                    //가장 가까운 오브를 바라볼 때 보상 2배 제시
                    if (actionBuffers.DiscreteActions[2] == FindClosestOrbIndex())
                    {
                        //AddReward(50f);
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
                        //AddReward(-500.0f); 
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
                //가장 가까운 오브를 바라볼 때 보상 2배 제시
                if (actionBuffers.DiscreteActions[2] == FindClosestOrbIndex())
                {
                    //AddReward(Time.deltaTime * 10f);
                }
            }

            /*
            if (eyeTrackingRay.HoveredCube.transform.gameObject.CompareTag("Enemy"))
            {
                //가장 가까운게 Enemy일 때만 처벌 없고 나머지는 다 처벌
                if (actionBuffers.DiscreteActions[2] != FindClosestOrbIndex())
                {
                    //AddReward(Time.deltaTime * 100f * (-1.0f));
                }
            } 
            */
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
                //AddReward(-8000.0f);
            }
            else if (damagedArea.stageHP <= 500)   
            {
                //AddReward(-5000.0f);
            }
            else if (damagedArea.stageHP <= 1000)   
            {
                //AddReward(-3000.0f);
            }

            //AddReward(-10f * controllerManager.MissingPoint);  

            //AddReward(damagedArea.stageHP);  

            //AddReward(controllerManager.skillEnergyPoint/10f);  

            if (damagedArea.stageHP == OriginStageHP)  
            {
                //AddReward(100000f);
            }
            else if(damagedArea.stageHP >= 1000)   
            {
                //AddReward(30000f);
            }

            isRewarding = true; 
        }
    }

    private void UpdateTarget(int DiscreteActionsNum)   
    {
        FindAndAddOrbs();

        if (orbsArray[0] != null && DiscreteActionsNum == 0 && orbsArray[0].tag != "Enemy")   
        {
            target = orbsArray[0].transform; 
        } 
        else if (orbsArray[1] != null && DiscreteActionsNum == 1 && orbsArray[1].tag != "Enemy")     
        {
            target = orbsArray[1].transform;
        } 
        else if (orbsArray[2] != null && DiscreteActionsNum == 2 && orbsArray[2].tag != "Enemy")    
        {
            target = orbsArray[2].transform;  
        } 

        if (target != null)
        {
            // 대상 방향으로 회전 
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            Eye.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime/10);
        }

        // 배열에 있는 Orbs 중 파괴된 것이 있다면 대체
        for (int i = 0; i < orbsArray.Length; i++)
        {
            if (orbsArray[i] == null || orbsArray[i].tag == spawnerTag)
            {
                ReplaceDestroyedOrb(i);
            }
        }

        // orbsPositions 배열 업데이트
        for (int i = 0; i < orbsArray.Length; i++)
        {
            if (orbsArray[i] != null)
            {
                orbsPositions[i] = orbsArray[i].transform.position; 
            }
        }
    }

    void FindAndAddOrbs()
    {
        GameObject[] orbs = GameObject.FindGameObjectsWithTag(orbsTag);

        // 최대 3개까지 배열에 추가
        for (int i = 0; i < Mathf.Min(orbs.Length, maxOrbs); i++)
        {
            orbsArray[i] = orbs[i];
        }

        // Orbs가 최대 개수보다 많을 경우, 초과된 부분을 제거
        for (int i = maxOrbs; i < orbsArray.Length; i++)
        {
            orbsArray[i] = null;
        }

        // Orbs가 최대 개수보다 적을 경우, 부족한 부분을 채워넣음
        for (int i = Mathf.Min(orbs.Length, maxOrbs); i < maxOrbs; i++)
        {
            GameObject replacement = FindReplacementOrbs();
            orbsArray[i] = replacement;
        }
    }

    void ReplaceDestroyedOrb(int index)
    {
        // 배열에서 파괴된 Orbs를 대체
        GameObject replacement = FindReplacementOrbs();
        orbsArray[index] = replacement;
    }

    GameObject FindReplacementOrbs()
    {
        GameObject orb = GameObject.FindGameObjectWithTag(orbsTag);

        if (!ArrayContainsObject(orbsArray, orb))
        {
            return orb;
        }

        // Orbs를 찾지 못한 경우, Spawner를 찾아서 반환
        GameObject spawner = GameObject.FindGameObjectWithTag(spawnerTag);

        // 배열에 이미 존재하지 않는 경우에만 반환
        if (!ArrayContainsObject(orbsArray, spawner))
        {
            return spawner;
        }

        return null; // Orbs나 Spawner를 찾지 못한 경우 null 반환
    }

    bool ArrayContainsObject(GameObject[] array, GameObject obj)
    {
        foreach (GameObject element in array)
        {
            if (element == obj)
            {
                return true;
            }
        }
        return false;
    }

    private int FindClosestOrbIndex()
    {
        float closestDistance = float.MaxValue;
        int closestIndex = -1;

        bool allDistancesEqual = true; // Assume all distances are equal initially

        for (int i = 0; i < orbsPositions.Length; i++)
        {
            if (orbsPositions[i] != null)
            {
                float distance = Vector3.Distance(this.transform.position, orbsPositions[i]);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                    allDistancesEqual = false; // Reset the flag since we found a closer distance
                }
                else if (distance > closestDistance)
                {
                    allDistancesEqual = false; // Reset the flag if we find a greater distance
                }
            }
        }

        // Check if all distances are equal, then return index 4
        if (allDistancesEqual)
        {
            return 4;
        }

        return closestIndex;
    }
}