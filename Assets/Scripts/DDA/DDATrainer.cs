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
    private ControllerManager controllerManager;

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

    [HideInInspector] public List<float> distanceOfOrbsToUserList = new List<float>();  //파괴한 오브들의 거리 리스트
    private float averageOfDistance = 0f; //위 리스트의 평균

    [SerializeField] private Transform Enemy = null;
    [SerializeField] private Transform User = null;

    private float distanceOfUserToEnemy = 0;   //적과 유저의 거리


    [SerializeField] private bool isTargetedlevel = false;

    private void Awake()
    {
        eventManager = this.transform.GetComponent<EventManager>();
        damagedArea = this.transform.GetComponent<DamagedArea>();
        spawnManager = this.GetComponent<SpawnManager>(); 
        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();
    }

    private void Start() 
    {
        //StartCoroutine(DecreaseOverTime());
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

    private void Update()
    {
        if ((spawnManager.activeStone == true && spawnManager.activeBasicOrb == true) || spawnManager.activeBasicOrb == true)
        {
            //리스트 크기를 최대 6개(2개 기준, 스포너 개수에 비례하게 증가)로 해서, 최근 6개(2개 기준, 스포너 개수에 비례하게 증가) 구체의 파괴 위치의 평균값을 구함
            if (distanceOfOrbsToUserList.Count > 3 * spawnManager.basicOrbSpawner.Length)
            {
                distanceOfOrbsToUserList.RemoveAt(0);
            }

            averageOfDistance = distanceOfOrbsToUserList.Sum() / distanceOfOrbsToUserList.Count();

        }
    }
    // 에이전트가 환경에서 관찰하는 데이터 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(damagedArea.stageHP);
        sensor.AddObservation(HPChange);
        sensor.AddObservation((int)averageOfDistance);

        sensor.AddObservation(LevelPoint);

        sensor.AddObservation(initialDiff);

        sensor.AddObservation((int)punishmentPoint);
    }

    // 에이전트가 행동을 수행할 때 호출되는 메서드
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (isStartChangeDiff == true)
        {
            //LevelPoint 변화
            LevelPoint += Time.deltaTime * actionBuffers.ContinuousActions[0];
            if (LevelPoint <= 0.25f)
            {
                LevelPoint = 0.25f;
            }
            else if (LevelPoint >= 1.75f)
            {
                LevelPoint = 1.75f;
            }
            AddReward(-100f * Mathf.Pow(Time.deltaTime * actionBuffers.ContinuousActions[0], 2)); 

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
        punishmentPoint = -0.1f * Mathf.Pow(averageOfDistance - distanceOfUserToEnemy * 0.5f, 2); 
        AddReward((int)punishmentPoint); //나중에 바꿔 

        // 어려운 난이도에서의 보상 및 처벌
        if (initialDiff == 3)
        {
            Debug.Log("어려워");
            //easy to hard 페널티
            if (previousDiff == 1)
                AddReward(-5000);
            previousDiff = 3;

            if (Mathf.Abs(LevelPoint - 1.75f) < 0.1f)
                AddReward(-100);
        }
        else if (initialDiff == 1) // 쉬운 난이도에서의 보상 및 처벌
        {
            Debug.Log("쉬워");
            //hard to easy 페널티
            if (previousDiff == 3)
                AddReward(-5000);
            previousDiff = 1;

            if (Mathf.Abs(LevelPoint - 0.25f) < 0.1f)
                AddReward(-100);
        }
        else if (initialDiff == 2) //중간 난이도 보상
        {
            Debug.Log("적당해");
            previousDiff = 2;

            PlayerPrefs.SetFloat("SavedLevel", LevelPoint);
            AddReward(200);
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
            SetReward(-1000000);
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

            isRewarding = true;
        }
    }

    // 시간에 따른 이벤트 처리
    private IEnumerator DecreaseOverTime()
    {
        yield return new WaitForSeconds(120f);
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

            //1초 후에 현재 MissingPoint와 처음에 저장한 값을 비교
            HPChange = initialStageHP - damagedArea.stageHP;

            //현재 시점에서 5초 전 범위로만 판단
            if (readUsersDiff.Count == 5)
                readUsersDiff.RemoveAt(0);
            readUsersDiff.Add(HPChange);

            int sumOfChange = readUsersDiff.Sum();

            if (sumOfChange >= 100 || averageOfDistance <= distanceOfUserToEnemy * 0.2) //데미지가 있는 상태거나 파괴한 공들의 평균 위치가 가까우면 어려운 상태
            {
                initialDiff = 3;
            }
            else if (averageOfDistance >= distanceOfUserToEnemy * 0.7) //파괴한 공들의 평균 위치가 멀면 쉬운 상태
            {
                initialDiff = 1;
            }
            else  //파괴한 공들의 평균 위치가 적당하면 적당한 상태
            {
                initialDiff = 2;
            }
        }
    }
}