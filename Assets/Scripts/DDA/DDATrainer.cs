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
    public int initialDiff = 0;  //1: Easy, 2: ��ǥ, 3:Hard
    private int previousDiff = 0;  //1: Easy, 2: ��ǥ, 3:Hard
    private float punishmentPoint = 0;

    private int HPChange = 0;
    private List<int> readUsersDiff = new List<int>();

    [HideInInspector] public List<float> distanceOfOrbsToUserList = new List<float>();  //�ı��� ������� �Ÿ� ����Ʈ
    private float averageOfDistance = 0f; //�� ����Ʈ�� ���

    [SerializeField] private Transform Enemy = null;
    [SerializeField] private Transform User = null;

    private float distanceOfUserToEnemy = 0;   //���� ������ �Ÿ�


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
            //����Ʈ ũ�⸦ �ִ� 6��(2�� ����, ������ ������ ����ϰ� ����)�� �ؼ�, �ֱ� 6��(2�� ����, ������ ������ ����ϰ� ����) ��ü�� �ı� ��ġ�� ��հ��� ����
            if (distanceOfOrbsToUserList.Count > 3 * spawnManager.basicOrbSpawner.Length)
            {
                distanceOfOrbsToUserList.RemoveAt(0);
            }

            averageOfDistance = distanceOfOrbsToUserList.Sum() / distanceOfOrbsToUserList.Count();

        }
    }
    // ������Ʈ�� ȯ�濡�� �����ϴ� ������ ����
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(damagedArea.stageHP);
        sensor.AddObservation(HPChange);
        sensor.AddObservation((int)averageOfDistance);

        sensor.AddObservation(LevelPoint);

        sensor.AddObservation(initialDiff);

        sensor.AddObservation((int)punishmentPoint);
    }

    // ������Ʈ�� �ൿ�� ������ �� ȣ��Ǵ� �޼���
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (isStartChangeDiff == true)
        {
            //LevelPoint ��ȭ
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
        punishmentPoint = -0.1f * Mathf.Pow(averageOfDistance - distanceOfUserToEnemy * 0.5f, 2); 
        AddReward((int)punishmentPoint); //���߿� �ٲ� 

        // ����� ���̵������� ���� �� ó��
        if (initialDiff == 3)
        {
            Debug.Log("�����");
            //easy to hard ���Ƽ
            if (previousDiff == 1)
                AddReward(-5000);
            previousDiff = 3;

            if (Mathf.Abs(LevelPoint - 1.75f) < 0.1f)
                AddReward(-100);
        }
        else if (initialDiff == 1) // ���� ���̵������� ���� �� ó��
        {
            Debug.Log("����");
            //hard to easy ���Ƽ
            if (previousDiff == 3)
                AddReward(-5000);
            previousDiff = 1;

            if (Mathf.Abs(LevelPoint - 0.25f) < 0.1f)
                AddReward(-100);
        }
        else if (initialDiff == 2) //�߰� ���̵� ����
        {
            Debug.Log("������");
            previousDiff = 2;

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
            SetReward(-1000000);
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

            //1�� �Ŀ� ���� MissingPoint�� ó���� ������ ���� ��
            HPChange = initialStageHP - damagedArea.stageHP;

            //���� �������� 5�� �� �����θ� �Ǵ�
            if (readUsersDiff.Count == 5)
                readUsersDiff.RemoveAt(0);
            readUsersDiff.Add(HPChange);

            int sumOfChange = readUsersDiff.Sum();

            if (sumOfChange >= 100 || averageOfDistance <= distanceOfUserToEnemy * 0.2) //�������� �ִ� ���°ų� �ı��� ������ ��� ��ġ�� ������ ����� ����
            {
                initialDiff = 3;
            }
            else if (averageOfDistance >= distanceOfUserToEnemy * 0.7) //�ı��� ������ ��� ��ġ�� �ָ� ���� ����
            {
                initialDiff = 1;
            }
            else  //�ı��� ������ ��� ��ġ�� �����ϸ� ������ ����
            {
                initialDiff = 2;
            }
        }
    }
}