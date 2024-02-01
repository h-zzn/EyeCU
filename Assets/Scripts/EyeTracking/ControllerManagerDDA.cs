using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

public class ControllerManagerDDA : MonoBehaviour
{
    private Vector3 OriginPosition;

    private GameObject LeftEyeInteractor;
    private GameObject RightEyeInteractor;

    private EyeTrackingRay eyeTrackingRayLeft;
    private EyeTrackingRay eyeTrackingRayRight;

    public GameObject RedMagicHitEffectPrefab;
    public GameObject BlueMagicHitEffectPrefab;
    public GameObject TaegukMagicHitEffectPrefab;

    [SerializeField] private GameObject leftSword;
    [SerializeField] private GameObject rightSword;

    [SerializeField] private GameObject leftEffect;
    [SerializeField] private GameObject rightEffect;

    [SerializeField] private AudioSource PoongSound;
    [SerializeField] private AudioSource ChengSound;
    [SerializeField] private AudioSource TikSound;

    [SerializeField] private float deactivateMagicTime = 1f;

    public bool redMagicActive = true;
    public bool blueMagicActive = true;

    private Coroutine redMagicPauseCoroutine; // Coroutine 참조 변수
    private Coroutine blueMagicPauseCoroutine; // Coroutine 참조 변수

    public int MissingPoint = 0;
    public int skillEnergyPoint = 0;
    public int attackPoint = 100;

    [SerializeField] private GameObject SkillGauge;
    [SerializeField] private GameObject SkillMaterialObj;
    [SerializeField] private GameObject SkillMagicHitEffectPrefab;

    private Renderer SkillGaugeRenderer;

    private List<Material> SkillMaterials;

    public HandEffectCollisionDDA handEffectCollision;

    public EventManager eventManager;

    private Dragon_Animation_Controll dragon_Animation_Controll;

    private Dragon1_Animation_Controll dragon1_Animation_Controll;

    private Dragon2_Animation_Controll dragon2_Animation_Controll;

    private bool isSkilled;

    private Coroutine BlinkSkillGauge = null;

    //DDA
    public bool leftClicked = false;
    public bool rightClicked = false;
    public DDATrainer dDATrainer = null;

    [HideInInspector] public List<float> distanceOfOrbsToUserList = new List<float>();  //파괴한 오브들의 거리 리스트
    private SpawnManager spawnManager;
    private float averageOfDistance = 0f; //위 리스트의 평균
    // CSV 파일 경로
    private string csvFilePath = "Assets/DDAData/averageOfDistance.csv";

    private void Awake()
    {
        spawnManager = GameObject.Find("StageCore").GetComponent<SpawnManager>();




        SkillMaterials = new List<Material>(SkillMaterialObj.GetComponent<Renderer>().materials);

        eventManager = GameObject.Find("StageCore").GetComponent<EventManager>();

        if (SceneManager.GetActiveScene().buildIndex == 3) //임시로 스테이지3만
            dragon_Animation_Controll = GameObject.FindWithTag("Enemy").GetComponent<Dragon_Animation_Controll>();
        else if (SceneManager.GetActiveScene().buildIndex == 2)
            dragon2_Animation_Controll = GameObject.FindWithTag("Enemy").GetComponent<Dragon2_Animation_Controll>();
        else if (SceneManager.GetActiveScene().buildIndex == 1)
            dragon1_Animation_Controll = GameObject.FindWithTag("Enemy").GetComponent<Dragon1_Animation_Controll>();

        dDATrainer = GameObject.Find("StageCore").GetComponent<DDATrainer>();
    }

    void Start()
    {
        StartCoroutine(DecreaseOverTime());
        StartCoroutine(CheckMissingPointChange());

        OriginPosition = this.transform.position;

        RightEyeInteractor = GameObject.Find("RightEyeInteractor");

        eyeTrackingRayRight = RightEyeInteractor.GetComponent<EyeTrackingRay>();

        SkillGaugeRenderer = SkillGauge.GetComponent<Renderer>();
        isSkilled = false;
    }

    void Update()
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

        this.transform.position = OriginPosition;

        if (eyeTrackingRayRight.HoveredCube != null)
        {
            AttackBasicOrbBtnDown();

            ActiveSkillBtnDown();
        }

        chargeSkillGauge();
    }

    private void chargeSkillGauge()
    {
        if (skillEnergyPoint < 100)
        {
            SkillGaugeRenderer.material = SkillMaterials[0];
        }
        else if (skillEnergyPoint <= 100)
        {
            SkillGaugeRenderer.material = SkillMaterials[1];
        }
        else if (skillEnergyPoint <= 300)
        {
            SkillGaugeRenderer.material = SkillMaterials[2];
        }
        else if (skillEnergyPoint <= 500)
        {
            SkillGaugeRenderer.material = SkillMaterials[3];
        }
        else if (skillEnergyPoint <= 700)
        {
            SkillGaugeRenderer.material = SkillMaterials[4];
        }
        else if (skillEnergyPoint <= 900)
        {
            SkillGaugeRenderer.material = SkillMaterials[5];
        }
        else if (skillEnergyPoint <= 1100)
        {
            SkillGaugeRenderer.material = SkillMaterials[6];
        }
        else if (skillEnergyPoint <= 1300)
        {
            SkillGaugeRenderer.material = SkillMaterials[7];
        }
        else if (skillEnergyPoint <= 1500)
        {
            SkillGaugeRenderer.material = SkillMaterials[8];
        }
        else if (skillEnergyPoint <= 1700)
        {
            SkillGaugeRenderer.material = SkillMaterials[9];
        }
        else if (skillEnergyPoint <= 1900)
        {
            SkillGaugeRenderer.material = SkillMaterials[10];
        }
        else if (skillEnergyPoint <= 1950)
        {
            SkillGaugeRenderer.material = SkillMaterials[11];
        }
        else
        {
            skillEnergyPoint = 2000; //스킬게이지가 2000을 넘어가면 2000으로 고정

            //여기에 스킬 게이지 반짝이 기능 넣어줘야함 to 현진
            if (BlinkSkillGauge == null)
                BlinkSkillGauge = StartCoroutine(ChangeSkillMaterial());
        }
    }

    private IEnumerator ChangeSkillMaterial()
    {
        while (handEffectCollision.canUseSkill == false)
        {
            SkillGaugeRenderer.material = SkillMaterials[11];
            yield return new WaitForSeconds(1f);
            SkillGaugeRenderer.material = SkillMaterials[12];
            yield return new WaitForSeconds(1f);
        }

        BlinkSkillGauge = null;
    }

    public void ActiveSkillBtnDown()
    {
        if (handEffectCollision.canUseSkill)
        {
            if (eyeTrackingRayRight.HoveredCube.CompareTag("WeakPoint"))
            {
                ActiveEnemyAnimation();

                // 스킬 이후 버튼 눌러서 어떻게 되는지 여기에 넣어야 함
                eventManager.EnemyHP -= 10;

                // Magic hit effect play at eyeTrackingRayRight.HoveredCube.transform.position
                Vector3 hitEffectPosition = eyeTrackingRayRight.markerSparks.transform.position;
                GameObject SkillMagicHitInstance = Instantiate(SkillMagicHitEffectPrefab, hitEffectPosition, Quaternion.identity);

                SkillMagicHitInstance.SetActive(true);
                Destroy(SkillMagicHitInstance, 3f);

                if (PoongSound != null)
                {
                    PoongSound.Play();
                }
            }
        }
    }

    private void ActiveEnemyAnimation()
    {
        if (eventManager.EnemyHP == 750)
        {
            if (SceneManager.GetActiveScene().buildIndex == 3)
                dragon_Animation_Controll.dragonIsAttacked = true;
            else if (SceneManager.GetActiveScene().buildIndex == 2)
                dragon2_Animation_Controll.dragonIsAttacked = true;
            else if (SceneManager.GetActiveScene().buildIndex == 1)
                dragon1_Animation_Controll.dragonIsAttacked = true;
            Invoke("DeActiveEnemyAnimation", 1f);
        }
        else if (eventManager.EnemyHP == 500)
        {
            if (SceneManager.GetActiveScene().buildIndex == 3)
                dragon_Animation_Controll.dragonIsAttacked = true;
            else if (SceneManager.GetActiveScene().buildIndex == 2)
                dragon2_Animation_Controll.dragonIsAttacked = true;
            else if (SceneManager.GetActiveScene().buildIndex == 1)
                dragon1_Animation_Controll.dragonIsAttacked = true;
            Invoke("DeActiveEnemyAnimation", 1f);
        }
        else if (eventManager.EnemyHP == 250)
        {
            if (SceneManager.GetActiveScene().buildIndex == 3)
                dragon_Animation_Controll.dragonIsAttacked = true;
            else if (SceneManager.GetActiveScene().buildIndex == 2)
                dragon2_Animation_Controll.dragonIsAttacked = true;
            else if (SceneManager.GetActiveScene().buildIndex == 1)
                dragon1_Animation_Controll.dragonIsAttacked = true;
            Invoke("DeActiveEnemyAnimation", 1f);
        }
        else if (eventManager.EnemyHP == 10)
        {
            if (SceneManager.GetActiveScene().buildIndex == 3)
                dragon_Animation_Controll.dragonIsDead = true;
            else if (SceneManager.GetActiveScene().buildIndex == 2)
                dragon2_Animation_Controll.dragonIsDead = true;
            else if (SceneManager.GetActiveScene().buildIndex == 1)
                dragon1_Animation_Controll.dragonIsDead = true;
        }
    }

    private void DeActiveEnemyAnimation()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3) //임시로 스테이지3만
            dragon_Animation_Controll.dragonIsAttacked = false;
        else if (SceneManager.GetActiveScene().buildIndex == 2)
            dragon2_Animation_Controll.dragonIsAttacked = false;
        else if (SceneManager.GetActiveScene().buildIndex == 1)
            dragon1_Animation_Controll.dragonIsAttacked = false;
    }

    private void AttackBasicOrbBtnDown()
    {
        if (rightClicked)
        {
            // O Correct
            if (eyeTrackingRayRight.HoveredCube.CompareTag("redCube"))
            {
                if (redMagicActive)
                { // 오른손 Red Magic이 사용 가능일 때 

                    skillEnergyPoint += attackPoint;

                    // Magic hit effect play at eyeTrackingRayRight.HoveredCube.transform.position
                    Vector3 hitEffectPosition = eyeTrackingRayRight.HoveredCube.transform.position;
                    GameObject redMagicHitInstance = Instantiate(RedMagicHitEffectPrefab, hitEffectPosition, Quaternion.identity);
                    redMagicHitInstance.SetActive(true);
                    Destroy(redMagicHitInstance, 3f);

                    Destroy(eyeTrackingRayRight.HoveredCube);
                    distanceOfOrbsToUserList.Add(Vector3.Distance(this.transform.position, hitEffectPosition));
                    eyeTrackingRayRight.HoveredCube = null;
                }
                else
                {
                    TikSound?.Play();
                }
            }
            else if (eyeTrackingRayRight.HoveredCube.CompareTag("blueCube")) // X Wrong
            {
                if (redMagicActive) // redMagicActive가 활성화되어 있을 때
                {
                    MissingPoint += 1;
                    skillEnergyPoint -= 50;

                    redMagicActive = false; // redMagic 비활성화
                    rightEffect.SetActive(false);

                    if (redMagicPauseCoroutine != null)
                    {
                        StopCoroutine(redMagicPauseCoroutine); // 기존 Coroutine 중지
                    }
                    redMagicPauseCoroutine = StartCoroutine(ActivateRedMagicAfter(deactivateMagicTime));
                }
            }

            rightClicked = false;
        }

        if (leftClicked)
        {
            // O Correct
            if (eyeTrackingRayRight.HoveredCube.CompareTag("blueCube"))   
            {
                if (blueMagicActive)
                { // 오른손 Blue Magic이 사용 가능일 때
                    skillEnergyPoint += attackPoint;

                    // Magic hit effect play at eyeTrackingRayRight.HoveredCube.transform.position
                    Vector3 hitEffectPosition = eyeTrackingRayRight.HoveredCube.transform.position;
                    GameObject blueMagicHitInstance = Instantiate(BlueMagicHitEffectPrefab, hitEffectPosition, Quaternion.identity);
                    blueMagicHitInstance.SetActive(true);
                    Destroy(blueMagicHitInstance, 3f);

                    Destroy(eyeTrackingRayRight.HoveredCube);
                    distanceOfOrbsToUserList.Add(Vector3.Distance(this.transform.position, hitEffectPosition));
                    eyeTrackingRayRight.HoveredCube = null;
                }
                else
                {
                    TikSound?.Play();
                }
            }
            else if (eyeTrackingRayRight.HoveredCube.CompareTag("redCube")) // X Wrong   
            {
                if (blueMagicActive) // blueMagicActive가 활성화되어 있을 때  
                {
                    MissingPoint += 1;
                    skillEnergyPoint -= 50;

                    blueMagicActive = false; // blueMagic 비활성화
                    leftEffect.SetActive(false);

                    if (blueMagicPauseCoroutine != null)
                    {
                        StopCoroutine(blueMagicPauseCoroutine); // 기존 Coroutine 중지
                    }
                    blueMagicPauseCoroutine = StartCoroutine(ActivateBlueMagicAfter(deactivateMagicTime));
                }
            }

            leftClicked = false;  
        }
    }

    IEnumerator ActivateRedMagicAfter(float second)
    {
        yield return new WaitForSeconds(second);
        redMagicActive = true;
        rightEffect.SetActive(true);
    }

    IEnumerator ActivateBlueMagicAfter(float second)
    {
        yield return new WaitForSeconds(second);
        blueMagicActive = true;
        leftEffect.SetActive(true);
    }

    void WriteCSVHeader()
    {
        // CSV 파일 생성 또는 덮어쓰기
        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            // 헤더 작성
            writer.WriteLine("averageOfDistance");
        }
    }

    void WriteFrameDataToCSV()
    {
        // CSV 파일에 프레임 데이터 추가
        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            // 현재 시간과 변수 값 기록
            writer.WriteLine($"{Time.time},{averageOfDistance}");
        }
    }

    private IEnumerator CheckMissingPointChange()
    {
        WriteCSVHeader();
        yield return new WaitForSeconds(10f);

        while (true)
        {
            //1초 기다림
            yield return new WaitForSeconds(1f);
            Debug.Log("a"); 
            WriteFrameDataToCSV();
        }
    }

    // 시간에 따른 이벤트 처리
    private IEnumerator DecreaseOverTime()
    {
        yield return new WaitForSeconds(120f);
        PlayerPrefs.Save();
        eventManager.EnemyHP = -100;
    }
}
