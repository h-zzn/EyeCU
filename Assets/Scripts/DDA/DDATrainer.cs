
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
    public int initialDiff = 0;  //1: Easy, 2: ��ǥ, 3:Hard
    private int previousDiff = 0;  //1: Easy, 2: ��ǥ, 3:Hard
    private float punishmentPoint = 0;

    private int HPChange = 0;
    private List<int> readUsersDiff = new List<int>();

    private float averageOfDistance = 0f; //�� ����Ʈ�� ���

    [SerializeField] private Transform Enemy = null;
    [SerializeField] private Transform User = null;

    private float distanceOfUserToEnemy = 0;   //���� ������ �Ÿ�


    [SerializeField] private bool isTargetedlevel = false;



    public GameObject[] basicOrbSpawnerListForDDA;
    private float randomExchangeInterval = 600f; // 10�п� �� ��
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
        //�н���
        //StartCoroutine(RandomlyExchangeSpawnersCoroutine());
        //�н���

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

    // ������Ʈ�� ȯ�濡�� �����ϴ� ������ ����
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(HPChange);

        sensor.AddObservation(LevelPoint);
        sensor.AddObservation(initialDiff);

        sensor.AddObservation((int)controllerManager.averageOfDistance);
    }

    // ������Ʈ�� �ൿ�� ������ �� ȣ��Ǵ� �޼���
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (actionBuffers.DiscreteActions[0] == 0)
        {
            if (previousAction != 0)
            {
                // ���ο� ���ӵ� ������ ����
                LevelController = -0.1f;
            }
            else
            {
                // ������ 2�� ���ӵ� ���
                // LevelController ���� õõ�� 0.01�� ������Ŵ
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
                // ���ο� ���ӵ� ������ ����
                LevelController = 0.1f;
            }
            else
            {
                // ������ 2�� ���ӵ� ���
                // LevelController ���� õõ�� 0.01�� ������Ŵ
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
            //LevelPoint ��ȭ
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

        //���̵� ����
        changeDiff();

        // �н� ���� ���� Ȯ��
        EndMLAgent();
    }

    private void changeDiff()
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

    private void RewardingByDiff()
    {
        //������ �Ÿ����� �м�
        punishmentPoint = -10f * Mathf.Pow(controllerManager.averageOfDistance - distanceOfUserToEnemy * 0.6f, 2); 
        AddReward(punishmentPoint);

        //����� ���̵������� ���� �� ó��
        if (initialDiff == 3)
        {
            Debug.Log("�����");
            previousDiff = 3;

            if (LevelController < 0)
                AddReward(200);
            else 
                AddReward(-400);
        }
        else if (initialDiff == 1) //���� ���̵������� ���� �� ó��
        {
            Debug.Log("����");
            previousDiff = 1;

            if (LevelController > 0)
                AddReward(200);
            else 
                AddReward(-400);
        }
        else if (initialDiff == 2) //�߰� ���̵� ����
        {
            Debug.Log("������");
            previousDiff = 2;

            AddReward(600);

            if (LevelController == 0)
                AddReward(400);
            else
                AddReward(-400);
        }
    }

    // �н� ���� ���� Ȯ�� �� ó��
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

    // �ð��� ���� �̺�Ʈ ó��
    private IEnumerator DecreaseOverTime()
    {
        yield return new WaitForSeconds(playTime);
        PlayerPrefs.Save();
        eventManager.EnemyHP -= (OriginEnemyHP * 2);
    }

    // ���� �ð� �������� ä�� ���̵� �ľ�
    private IEnumerator CheckMissingPointChange()
    {
        yield return new WaitForSeconds(20f);
        isStartChangeDiff = true;

        while (true)
        {
            // ó���� ������ stageHP ���� ����
            int initialStageHP = damagedArea.stageHP;

            //1�� ��ٸ�
            yield return new WaitForSeconds(1f);

            //1�� �Ŀ� ���� StageHP�� ó���� ������ ���� ��
            HPChange = initialStageHP - damagedArea.stageHP;

            //���� �������� 3�� �� �����θ� �Ǵ�
            if (readUsersDiff.Count == 3)
                readUsersDiff.RemoveAt(0);
            readUsersDiff.Add(HPChange);

            int sumOfChange = readUsersDiff.Sum();

            if (sumOfChange > 100 || controllerManager.averageOfDistance <= distanceOfUserToEnemy * 0.4) //�������� �ִ� ���°ų� �ı��� ������ ��� ��ġ�� ������ ����� ����
            {
                initialDiff = 3;
            }
            else if (controllerManager.averageOfDistance >= distanceOfUserToEnemy * 0.8) //�ı��� ������ ��� ��ġ�� �ָ� ���� ����
            {
                initialDiff = 1;
            }
            else  //�ı��� ������ ��� ��ġ�� �����ϸ� ������ ����
            {
                initialDiff = 2;
            }
        }
    }

    IEnumerator RandomlyExchangeSpawnersCoroutine()
    {
        playTime = 5000f;  //�н��� ���� 5000f�� --> 5�� ���ܸ��� ���Ǽҵ尡 ������ �ȴٴ� �ǹ�
        StartCoroutine(Heal());
        while (true)
        {
            RandomlyExchangeSpawners();
            yield return new WaitForSeconds(randomExchangeInterval);
        }
    }

    void RandomlyExchangeSpawners()
    {
        int newSpawnerCount = Random.Range(1, 6); // 1���� 5 ������ ������ ������ ���ο� spawner ������ ����

        List<GameObject> newSpawners = new List<GameObject>();

        spawnManager.BasicSpawnStop(true); // ���� �����ʵ��� ��Ȱ��ȭ

        // ���ο� spawner �迭�� ���� spawner �߰�
        newSpawners.AddRange(spawnManager.basicOrbSpawner);

        // ���ο� spawner �迭�� ũ�⸦ ����
        if (newSpawnerCount > spawnManager.basicOrbSpawner.Length)
        {
            int addCount = newSpawnerCount - spawnManager.basicOrbSpawner.Length;
            for (int i = 0; i < addCount; i++)
            {
                int randomIndex = Random.Range(0, basicOrbSpawnerListForDDA.Length);
                GameObject newSpawner = basicOrbSpawnerListForDDA[randomIndex];

                // �ߺ��� �����ʰ� ���ο� �迭 ���ο� �������� �ʵ��� Ȯ��
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

        // ���ο� spawner �迭�� ��ü
        spawnManager.basicOrbSpawner = newSpawners.ToArray();
    }

    // 30�ʸ��� ȸ��
    IEnumerator Heal()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            damagedArea.stageHP = 2000000;
        }
    }
}