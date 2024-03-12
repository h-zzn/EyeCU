
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DDATrainer : Agent
{
    private bool isRewarding = false;
    private SpawnManager spawnManager;
    private EventManager eventManager;
    private DamagedArea damagedArea;
    private ControllerManagerDDA controllerManager;
    private int OriginStageHP;
    private int OriginEnemyHP;

    private float OriginBasicOrbSpeed;
    private float OriginBasicOrbSpawnInterval;
    private float OriginSpecialOrbSpeed;
    private float OriginSpecialOrbSpawnInterval;
    private float OriginStoneSpeed;
    private float OriginStoneSpawnInterval;

    [SerializeField] private bool isHardDif = false;
    [SerializeField] private bool isEasyDif = false;

    private bool isStartChangeDiff = false;
    [SerializeField] public float LevelPoint = 1;
    public int initialDiff = 0;  //1: Easy, 2: 목표, 3:Hard
    private int previousDiff = 0;  //1: Easy, 2: 목표, 3:Hard
    private float punishmentPoint = 0;

    private int HPChange = 0;
    private List<int> readUsersDiff = new List<int>();

    private float averageOfDistance = 0f; //위 리스트의 평균

    [SerializeField] private Transform Enemy = null;
    [SerializeField] private Transform User = null;

    private float distanceOfUserToEnemy = 0;   //적과 유저의 거리


    [SerializeField] private bool isTargetedlevel = false;



    public GameObject[] basicOrbSpawnerListForDDA;
    private float randomExchangeInterval = 600f; // 10분에 한 번
    private float playTime = 120f;
    private float LevelController = 1;


    private int previousAction = 1;

    private void Awake()
    {
        eventManager = this.transform.GetComponent<EventManager>();
        damagedArea = this.transform.GetComponent<DamagedArea>();
        spawnManager = this.GetComponent<SpawnManager>();
        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManagerDDA>();
    }

    private void Start()
    {
        //학습용
        //StartCoroutine(RandomlyExchangeSpawnersCoroutine());
        //학습용

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

        distanceOfUserToEnemy = Vector3.Distance(Enemy.position, User.position);
    }

    // 에이전트가 환경에서 관찰하는 데이터 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(HPChange);

        sensor.AddObservation(LevelPoint);
        sensor.AddObservation(initialDiff);

        sensor.AddObservation((int)controllerManager.averageOfDistance);
    }

    // 에이전트가 행동을 수행할 때 호출되는 메서드
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (actionBuffers.DiscreteActions[0] == 0)
        {
            if (previousAction != 0)
            {
                // 새로운 연속된 조건의 시작
                LevelController = -0.1f;
            }
            else
            {
                // 기존에 2가 연속된 경우
                // LevelController 값을 천천히 0.01씩 증가시킴
                LevelController += 0.0001f;

                if (LevelController > -0.005f)
                {
                    LevelController = -0.005f;
                }
            }
        }
        else if (actionBuffers.DiscreteActions[0] == 2)
        {
            if (previousAction != 2)
            {
                // 새로운 연속된 조건의 시작
                LevelController = 0.1f;
            }
            else
            {
                // 기존에 2가 연속된 경우
                // LevelController 값을 천천히 0.01씩 증가시킴
                LevelController -= 0.0001f;

                if (LevelController < 0.005f)
                {
                    LevelController = 0.005f;
                }
            }
        }
        else if (actionBuffers.DiscreteActions[0] == 1)
        {
            LevelController = 0;
        }

        previousAction = actionBuffers.DiscreteActions[0];



        if (isStartChangeDiff == true)
        {
            //LevelPoint 변화
            LevelPoint += Time.deltaTime * LevelController;

            if (LevelPoint <= 0.2f)
            {
                LevelPoint = 0.2f;
            }
            else if (LevelPoint >= 1.8f)
            {
                LevelPoint = 1.8f;
            }
            AddReward(-100f * Mathf.Pow(Time.deltaTime * LevelController, 2));

            RewardingByDiff();
        }
        else
        {
            LevelPoint = 1;
        }

        //난이도 조절
        changeDiff();

        // 학습 종료 여부 확인
        EndMLAgent();
    }

    private void changeDiff()
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

    private void RewardingByDiff()
    {
        //적절한 거리에서 분석
        punishmentPoint = -10f * Mathf.Pow(controllerManager.averageOfDistance - distanceOfUserToEnemy * 0.6f, 2); 
        AddReward(punishmentPoint);

        //어려운 난이도에서의 보상 및 처벌
        if (initialDiff == 3)
        {
            Debug.Log("어려워");
            previousDiff = 3;

            if (LevelController < 0)
                AddReward(200);
            else 
                AddReward(-400);
        }
        else if (initialDiff == 1) //쉬운 난이도에서의 보상 및 처벌
        {
            Debug.Log("쉬워");
            previousDiff = 1;

            if (LevelController > 0)
                AddReward(200);
            else 
                AddReward(-400);
        }
        else if (initialDiff == 2) //중간 난이도 보상
        {
            Debug.Log("적당해");
            previousDiff = 2;

            AddReward(600);

            if (LevelController == 0)
                AddReward(400);
            else
                AddReward(-400);
        }
    }

    // 학습 종료 조건 확인 및 처리
    public void EndMLAgent()
    {
        if (eventManager.GameClear == true)
        {
            EndEpisode();
        }

        if (damagedArea.stageHP < 0)
        {
            SetReward(-1000000);
            EndEpisode();
        }
    }

    // 시간에 따른 이벤트 처리
    private IEnumerator DecreaseOverTime()
    {
        yield return new WaitForSeconds(playTime);
        PlayerPrefs.Save();
        eventManager.EnemyHP -= (OriginEnemyHP * 2);
    }

    // 일정 시간 간격으로 채감 난이도 파악
    private IEnumerator CheckMissingPointChange()
    {
        yield return new WaitForSeconds(20f);
        isStartChangeDiff = true;

        while (true)
        {
            // 처음에 현재의 stageHP 값을 저장
            int initialStageHP = damagedArea.stageHP;

            //1초 기다림
            yield return new WaitForSeconds(1f);

            //1초 후에 현재 StageHP와 처음에 저장한 값을 비교
            HPChange = initialStageHP - damagedArea.stageHP;

            //현재 시점에서 3초 전 범위로만 판단
            if (readUsersDiff.Count == 3)
                readUsersDiff.RemoveAt(0);
            readUsersDiff.Add(HPChange);

            int sumOfChange = readUsersDiff.Sum();

            if (sumOfChange > 100 || controllerManager.averageOfDistance <= distanceOfUserToEnemy * 0.4) //데미지가 있는 상태거나 파괴한 공들의 평균 위치가 가까우면 어려운 상태
            {
                initialDiff = 3;
            }
            else if (controllerManager.averageOfDistance >= distanceOfUserToEnemy * 0.8) //파괴한 공들의 평균 위치가 멀면 쉬운 상태
            {
                initialDiff = 1;
            }
            else  //파괴한 공들의 평균 위치가 적당하면 적당한 상태
            {
                initialDiff = 2;
            }
        }
    }

    IEnumerator RandomlyExchangeSpawnersCoroutine()
    {
        playTime = 5000f;  //학습할 때는 5000f로 --> 5만 스텝마다 에피소드가 마무리 된다는 의미
        StartCoroutine(Heal());
        while (true)
        {
            RandomlyExchangeSpawners();
            yield return new WaitForSeconds(randomExchangeInterval);
        }
    }

    void RandomlyExchangeSpawners()
    {
        int newSpawnerCount = Random.Range(1, 6); // 1에서 5 사이의 랜덤한 값으로 새로운 spawner 개수를 선택

        List<GameObject> newSpawners = new List<GameObject>();

        spawnManager.BasicSpawnStop(true); // 기존 스포너들을 비활성화

        // 새로운 spawner 배열에 기존 spawner 추가
        newSpawners.AddRange(spawnManager.basicOrbSpawner);

        // 새로운 spawner 배열의 크기를 변경
        if (newSpawnerCount > spawnManager.basicOrbSpawner.Length)
        {
            int addCount = newSpawnerCount - spawnManager.basicOrbSpawner.Length;
            for (int i = 0; i < addCount; i++)
            {
                int randomIndex = Random.Range(0, basicOrbSpawnerListForDDA.Length);
                GameObject newSpawner = basicOrbSpawnerListForDDA[randomIndex];

                // 중복된 스포너가 새로운 배열 내부에 존재하지 않도록 확인
                if (!newSpawners.Contains(newSpawner))
                {
                    newSpawners.Add(newSpawner);
                }
            }
        }
        else if (newSpawnerCount < spawnManager.basicOrbSpawner.Length)
        {
            int removeCount = spawnManager.basicOrbSpawner.Length - newSpawnerCount;
            for (int i = 0; i < removeCount; i++)
            {
                int randomIndex = Random.Range(0, newSpawners.Count);
                GameObject spawnerToRemove = newSpawners[randomIndex];
                newSpawners.Remove(spawnerToRemove);
            }
        }

        // 새로운 spawner 배열로 교체
        spawnManager.basicOrbSpawner = newSpawners.ToArray();
    }

    // 30초마다 회복
    IEnumerator Heal()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            damagedArea.stageHP = 2000000;
        }
    }
}