using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRHaptics; 

public class ControllerManager : MonoBehaviour
{
    private Vector3 OriginPosition;

    private GameObject LeftEyeInteractor;
    private GameObject RightEyeInteractor;

    private EyeTrackingRay eyeTrackingRayLeft;
    private EyeTrackingRay eyeTrackingRayRight;

    public OVRInput.Controller controllerType; // 컨트롤러 종류 선택

    Vector3 hitEffectPosition;

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
    public bool dragonIsAttacked = false;

    private Coroutine redMagicPauseCoroutine; // Coroutine 참조 변수
    private Coroutine blueMagicPauseCoroutine; // Coroutine 참조 변수

    public float vibrationDuration = 0.5f; // 햅틱 지속 시간

    public int MissingPoint = 0;
    public int skillEnergyPoint = 0;
    public int attackPoint = 100;

    [SerializeField] private GameObject SkillGauge;
    [SerializeField] private GameObject SkillMaterialObj;
    [SerializeField] private GameObject SkillMagicHitEffectPrefab;

    private Renderer SkillGaugeRenderer;

    private List<Material> SkillMaterials;

    public HandEffectCollision handEffectCollision;

    public EventManager eventManager;

    private int SkillAnimaGauge = 0;

    private bool isSkilled;

    private Coroutine BlinkSkillGauge = null;

    private void Awake() 
    {
        SkillMaterials = new List<Material>(SkillMaterialObj.GetComponent<Renderer>().materials); 

        eventManager = GameObject.Find("StageCore").GetComponent<EventManager>();
    }

    void Start()
    {
        OriginPosition = this.transform.position;

        //LeftEyeInteractor = GameObject.Find("LeftEyeInteractor");
        RightEyeInteractor = GameObject.Find("RightEyeInteractor");
        
        //eyeTrackingRayLeft = LeftEyeInteractor.GetComponent<EyeTrackingRay>();
        eyeTrackingRayRight = RightEyeInteractor.GetComponent<EyeTrackingRay>();

        SkillGaugeRenderer = SkillGauge.GetComponent<Renderer>();
        isSkilled = false;
    }

    void Update()
    {
        this.transform.position = OriginPosition; 

        if(eyeTrackingRayRight.HoveredCube != null) 
        {
            AttackBasicOrbBtnDown();  
            
            ActiveSkillBtnDown(); 
        }
        

        activeSword();
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
            //SkillGaugeRenderer.material = SkillMaterials[11]; 
            skillEnergyPoint = 2000; //스킬게이지가 2000을 넘어가면 2000으로 고정


            //여기에 스킬 게이지 반짝이 기능 넣어줘야함 to 현진
            if(BlinkSkillGauge==null) 
                BlinkSkillGauge = StartCoroutine(ChangeSkillMaterial());

        }
    }

    private IEnumerator ChangeSkillMaterial()
    {
        while(handEffectCollision.canUseSkill == false)
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
        if (handEffectCollision.canUseSkill && (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)))
        {
            
            if(eyeTrackingRayRight.HoveredCube.CompareTag("WeakPoint"))
            {
                // 스킬 이후 버튼 눌러서 어떻게 되는지 여기에 넣어야 함
                eventManager.EnemyHP -= 10;
                SkillAnimaGauge += 1;

                if(SkillAnimaGauge >= 10)
                {
                    SkillAnimaGauge = 0;
                    dragonIsAttacked = true;
                }
                else if(SkillAnimaGauge >= 8)
                {
                    dragonIsAttacked = false;
                }

                // Oculus 컨트롤러에서 햅틱 반응을 발생시킵니다.
                OVRHapticsClip clip = new OVRHapticsClip();
                for (int i = 0; i < Config.SampleRateHz * vibrationDuration; i++)
                {
                    byte sample = (byte)(1.0f * 255);
                    clip.WriteSample(sample);
                }

                RightChannel.Preempt(clip);
                LeftChannel.Preempt(clip);

                // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
                StartCoroutine(StopVibration());
                
                // Magic hit effect play at eyeTrackingRayRight.HoveredCube.transform.position
                hitEffectPosition = eyeTrackingRayRight.markerSparks.transform.position;
                Quaternion rotation = Quaternion.Euler(0, 90, 0);
                GameObject SkillMagicHitInstance = Instantiate(SkillMagicHitEffectPrefab, hitEffectPosition, rotation);

                SkillMagicHitInstance.SetActive(true);
                Destroy(SkillMagicHitInstance, 3f);

                if (PoongSound != null)
                {
                    PoongSound.Play();
                }
            }
        }
    }


    private void AttackBasicOrbBtnDown()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("TaegukCube"))
            {
                if (redMagicActive && blueMagicActive)
                { // 오른손 Red Magic이 사용 가능일 때 

                    skillEnergyPoint += attackPoint;

                    if (PoongSound != null)
                    {
                        PoongSound.Play();
                    }

                    // Oculus 컨트롤러에서 햅틱 반응을 발생시킵니다.
                    OVRHapticsClip clip = new OVRHapticsClip();
                    for (int i = 0; i < Config.SampleRateHz * vibrationDuration; i++)
                    {
                        byte sample = (byte)(1.0f * 255);
                        clip.WriteSample(sample);
                    }

                    RightChannel.Preempt(clip);
                    LeftChannel.Preempt(clip);

                    // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
                    StartCoroutine(StopVibration());

                    // Magic hit effect play at eyeTrackingRayRight.HoveredCube.transform.position
                    hitEffectPosition = eyeTrackingRayRight.HoveredCube.transform.position;
                    GameObject TaegukMagicHitInstance = Instantiate(TaegukMagicHitEffectPrefab, hitEffectPosition, Quaternion.identity);
                    TaegukMagicHitInstance.SetActive(true);
                    Destroy(TaegukMagicHitInstance, 3f);

                    Destroy(eyeTrackingRayRight.HoveredCube);
                    eyeTrackingRayRight.HoveredCube = null;
                }
                else
                {
                    TikSound?.Play();
                }
            }
        }

        // 오른손 Red Magic
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            // O Correct
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("redCube"))
            {   
                if (redMagicActive){ // 오른손 Red Magic이 사용 가능일 때 

                    skillEnergyPoint += attackPoint;

                    if (PoongSound != null)
                    {
                        PoongSound.Play();
                    }

                    // Oculus 컨트롤러에서 햅틱 반응을 발생시킵니다.
                    OVRHapticsClip clip = new OVRHapticsClip();
                    for (int i = 0; i < Config.SampleRateHz * vibrationDuration; i++)
                    {
                        byte sample = (byte)(1.0f * 255);
                        clip.WriteSample(sample);
                    }

                    RightChannel.Preempt(clip);

                    // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
                    StartCoroutine(StopVibration());

                    // Magic hit effect play at eyeTrackingRayRight.HoveredCube.transform.position
                    hitEffectPosition = eyeTrackingRayRight.HoveredCube.transform.position;
                    GameObject redMagicHitInstance = Instantiate(RedMagicHitEffectPrefab, hitEffectPosition, Quaternion.identity);
                    redMagicHitInstance.SetActive(true);
                    Destroy(redMagicHitInstance,3f);

                    Destroy(eyeTrackingRayRight.HoveredCube);
                    eyeTrackingRayRight.HoveredCube = null;
                }
                else
                {
                    TikSound?.Play();
                }
            }  
            else if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("blueCube")) // X Wrong
            {
                if (redMagicActive) // redMagicActive가 활성화되어 있을 때
                {
                    MissingPoint += 1;
                    skillEnergyPoint -= 100;
                    // Debug.Log("Wrong Target! : Red Magic Deactivated for 1 second!!!!!!!!!!!!!!!!!!!!!"); 
                    redMagicActive = false; // redMagic 비활성화
                    rightEffect.SetActive(false);
                    // 1초 뒤에 redMagicActive 다시 false로 변경하는 Coroutine 시작
                    if(redMagicPauseCoroutine != null)
                    {
                        StopCoroutine(redMagicPauseCoroutine); // 기존 Coroutine 중지
                    }
                    redMagicPauseCoroutine = StartCoroutine(ActivateRedMagicAfter(deactivateMagicTime));
                }
            }
        }

        // 왼손 Blue Magic
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            // O Correct
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("blueCube"))
            {
                if(blueMagicActive){ // 오른손 Blue Magic이 사용 가능일 때
                    skillEnergyPoint += attackPoint;

                    if (PoongSound != null)
                    {
                        PoongSound.Play();
                    }

                    // Oculus 컨트롤러에서 햅틱 반응을 발생시킵니다.
                    OVRHapticsClip clip = new OVRHapticsClip();
                    for (int i = 0; i < Config.SampleRateHz * vibrationDuration; i++)
                    {
                        byte sample = (byte)(1.0f * 255);
                        clip.WriteSample(sample);
                    }

                    LeftChannel.Preempt(clip);

                    // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
                    StartCoroutine(StopVibration());

                    // Magic hit effect play at eyeTrackingRayRight.HoveredCube.transform.position
                    hitEffectPosition = eyeTrackingRayRight.HoveredCube.transform.position;
                    GameObject blueMagicHitInstance = Instantiate(BlueMagicHitEffectPrefab, hitEffectPosition, Quaternion.identity);
                    blueMagicHitInstance.SetActive(true);
                    Destroy(blueMagicHitInstance,3f);

                    Destroy(eyeTrackingRayRight.HoveredCube);
                    eyeTrackingRayRight.HoveredCube = null;
                }
                else
                {
                    // Tik Sound play
                    TikSound?.Play();
                }
            }
            else if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("redCube")) // X Wrong
            {
                if (blueMagicActive) // blueMagicActive가 활성화되어 있을 때  
                {
                    MissingPoint += 1;
                    skillEnergyPoint -= 100;
                    // Debug.Log("Wrong Target! : Blue Magic Deactivated for 1 second!!!!!!!!!!!!!!!!!!!!!");  
                    blueMagicActive = false; // blueMagic 비활성화
                    leftEffect.SetActive(false);
                    // 1초 뒤에 blueMagicActive 다시 false로 변경하는 Coroutine 시작
                    if (blueMagicPauseCoroutine != null)
                    {
                        StopCoroutine(blueMagicPauseCoroutine); // 기존 Coroutine 중지
                    }
                    blueMagicPauseCoroutine = StartCoroutine(ActivateBlueMagicAfter(deactivateMagicTime));
                }
            }
        }
    }

    IEnumerator ActivateRedMagicAfter(float second)
    {
        yield return new WaitForSeconds(second);
        redMagicActive = true;
        rightEffect.SetActive(true);
        // Debug.Log("Red Magic Activated!!!!!!!!!!!!!!!!!!");
    }

    IEnumerator ActivateBlueMagicAfter(float second)
    {
        yield return new WaitForSeconds(second);
        blueMagicActive = true;
        leftEffect.SetActive(true);
        // Debug.Log("Blue Magic Activated!!!!!!!!!!!!!!!!!!");
    }

    private IEnumerator StopVibration()
    {
        // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
        yield return new WaitForSeconds(vibrationDuration);

        // Oculus 컨트롤러의 햅틱 반응을 중지합니다.
        OVRHapticsChannel hapticsChannel = (controllerType == OVRInput.Controller.LTouch) ? LeftChannel : RightChannel;
        hapticsChannel.Clear();
    }

    private void activeSword()
    {
        //sword ON effect off
        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            if (ChengSound != null)
            { 
                ChengSound.Play();
            } 

            if (rightSword.activeSelf == false)
            { 
                rightSword.SetActive(true);
            } 

            if (rightEffect.activeSelf == true)
            { 
                rightEffect.SetActive(false);
            } 
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            if (ChengSound != null) 
            { 
                ChengSound.Play(); 
            } 

            if (leftSword.activeSelf == false)
            {
                leftSword.SetActive(true);
            }

            if (leftEffect.activeSelf == true)
            {
                leftEffect.SetActive(false);
            }
        }

        //sword Off effect on
        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            if (rightSword.activeSelf == true)
            {
                rightSword.SetActive(false);
            }

            if (rightEffect.activeSelf == false)
            {
                rightEffect.SetActive(true);
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            if (leftSword.activeSelf == true)
            {
                leftSword.SetActive(false);
            }

            if (leftEffect.activeSelf == false)
            {
                leftEffect.SetActive(true);
            }
        }
    }
}
