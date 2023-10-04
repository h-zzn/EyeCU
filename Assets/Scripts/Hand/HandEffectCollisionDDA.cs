using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.PostProcessing; // Post-processing에 관련된 네임스페이스 추가
using static OVRHaptics;

public class HandEffectCollisionDDA : MonoBehaviour
{
    public float SkillTriggerDuration;

    public GameObject skillCircle; // "skill"시작을 알리는 오브젝트를 저장할 리스트
    public GameObject LeftPurpleEffect;
    public GameObject RightPurpleEffect;

    public bool canUseSkill = false;

    private ControllerManager controllerManager = null;
    private ControllerManagerDDA controllerManagerDDA;

    private DeleteEnemyAttack deleteEnemyAttack;
    private SpawnManager spawnManager;


    public Coroutine reduceSkillCoroutine = null;

    private Collider otherCollider;

    // Weakness 관련 변수
    [SerializeField] private Material transparentMaterial;
    private Material originalMaterial;
    [SerializeField] private GameObject monsterMeshObject; 

    [SerializeField] private GameObject weaknessObject;
    [SerializeField] private GameObject EnemyGauge;

    // ===시야 전환 관련===
    private Transform cam;
    // Assign Post Processing Volume from the Scene
    [SerializeField] private Volume postProcessingVolumeObject;
    private UnityEngine.Rendering.Universal.Vignette vignette;
    private Color originalVignetteColorValue;
    private float originalVignetteIntensityValue;
    public float transitionDuration = 1f; // 전환 시간 (초)
    private float sightTransitionTimer = 0f;
    private bool isSightTransitioning = false;
    private float sightBackTransitionTimer = 0f;
    private bool isSightBackTransitioning = false;
    // target color
    [SerializeField] private Color targetVignetteColor = Color.black;
    // target intensity
    [SerializeField] private float targetVignetteIntensity = 0.8f;

    private void Start()
    {
        // Scene에서 OVRInPlayMode를 찾아 cam에 assign
        cam = GameObject.Find("OVRInPlayMode").transform;

        // Scene에서 PostProcessVolume을 가져옴
        postProcessingVolumeObject = GameObject.Find("Post Processing").GetComponent<Volume>();

        skillCircle.SetActive(false);
        LeftPurpleEffect.SetActive(false);
        RightPurpleEffect.SetActive(false);

        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();
        if(controllerManager == null) 
            controllerManagerDDA = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManagerDDA>();

        deleteEnemyAttack = GameObject.Find("Eraser").GetComponent<DeleteEnemyAttack>();

        spawnManager = GameObject.Find("StageCore").GetComponent<SpawnManager>();

        // 현재 오브젝트의 Material 컴포넌트 가져와 저장
        originalMaterial = monsterMeshObject.GetComponent<Renderer>().material;
        
        // Weakness 오브젝트 비활성화
        weaknessObject.SetActive(false);
    }

    private void Update()
    {
        // 시야 전환
        if (isSightTransitioning)
        {
            sightTransitionTimer += Time.deltaTime;
            
            if (sightTransitionTimer >= transitionDuration)
            {
                sightTransitionTimer = transitionDuration;
                isSightTransitioning = false;
            }

            // 시간에 따라 Vignette 값 변경
            vignette.color.Override(Color.Lerp(originalVignetteColorValue, targetVignetteColor, sightTransitionTimer / transitionDuration));
            vignette.intensity.Override(Mathf.Lerp(originalVignetteIntensityValue, targetVignetteIntensity, sightTransitionTimer / transitionDuration));
        }

        // 시야 전환 복귀
        if (isSightBackTransitioning)
        {
            sightBackTransitionTimer += Time.deltaTime;

            if (sightBackTransitionTimer >= transitionDuration)
            {
                sightBackTransitionTimer = transitionDuration;
                isSightBackTransitioning = false;
            }

            // 시간에 따라 Vignette 값 변경
            vignette.color.Override(Color.Lerp(targetVignetteColor, originalVignetteColorValue, sightBackTransitionTimer / transitionDuration));
            vignette.intensity.Override(Mathf.Lerp(targetVignetteIntensity, originalVignetteIntensityValue, sightBackTransitionTimer / transitionDuration));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (controllerManager.skillEnergyPoint >= 2000 && other.CompareTag("HandEffect"))  
        {
            // Deactivate children of this GameObject
            DeactivateChildren(this.gameObject);
            DeactivateChildren(other.gameObject);
            LeftPurpleEffect.SetActive(true);
            RightPurpleEffect.SetActive(true);

            StartVibration();
            ActiveSkill();

            otherCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (controllerManager.skillEnergyPoint >= 2000 && other.CompareTag("HandEffect"))
        {
            SkillTriggerDuration = 0;

            StopVibration();

            otherCollider = null;

          //ReactivateChildren(this.gameObject); 
          //ReactivateChildren(other.gameObject); 
        }
    }

    private void ActiveSkill() 
    {
        canUseSkill = true;

        spawnManager.activeSkill = true; 

        StopVibration();
       
        //활성화되지 않은 "skill" 오브젝트를 찾아서 활성화
        skillCircle.SetActive(true);
        EnemyGauge.SetActive(true);

        if (reduceSkillCoroutine == null)
        {
            reduceSkillCoroutine = StartCoroutine("reduceSkillGauge");  
        }

        spawnManager.BasicSpawnStop(true); 
        spawnManager.StoneSpawnStop(true); 
        spawnManager.SpecialOrbSpawnAllStop(true);  

        deleteEnemyAttack.StartCoroutine("DeleteAll"); 

        turnMonsterTransparent();
        activateSkillSight();
    }

    public IEnumerator reduceSkillGauge()  //스킬 시작되고 10초동안 게이지가 감소 후 종료
    {
        while (controllerManager.skillEnergyPoint > 0) 
        {
            yield return new WaitForSeconds(1);

            controllerManager.skillEnergyPoint -= 200;  

            if (controllerManager.skillEnergyPoint < 100)
            {
                controllerManager.skillEnergyPoint = 0;
            }
        }

        canUseSkill = false;
        reduceSkillCoroutine = null;
        // 적 투시 종료
        turnMonsterOpaque();
        // 시야 정상화
        deactivateSkillSight();

        LeftPurpleEffect.SetActive(false);
        RightPurpleEffect.SetActive(false);

        spawnManager.activeSkill = false; 

        skillCircle.SetActive(false);
        EnemyGauge.SetActive(false);

        ReactivateChildren(this.gameObject); 
        if (otherCollider!=null)
        {
            ReactivateChildren(otherCollider.gameObject);
        }
    }

    private void DeactivateChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void ReactivateChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void StartVibration()
    {
        // Oculus 컨트롤러에서 햅틱 반응을 발생시킵니다.
        OVRHapticsClip clip = new OVRHapticsClip();
        for (int i = 0; i < Config.SampleRateHz * 3; i++)
        {
            byte sample = (byte)(1.0f * 255);
            clip.WriteSample(sample);
        }

        LeftChannel.Preempt(clip);
        RightChannel.Preempt(clip);
    }

    private void StopVibration()
    {
        // Oculus 컨트롤러의 햅틱 반응을 중지합니다.
        LeftChannel.Clear();
        RightChannel.Clear();
    }

    // Monster 투시 관련 함수들
    void turnMonsterTransparent()
    {
        // 현재 오브젝트의 Material 컴포넌트를 투명한 Material로 변경
        monsterMeshObject.GetComponent<Renderer>().material = transparentMaterial; 

        // Weakness 오브젝트 활성화
        weaknessObject.SetActive(true);
    }

    void turnMonsterOpaque()
    {
        // 현재 오브젝트의 Material 컴포넌트를 투명한 Material로 변경
        monsterMeshObject.GetComponent<Renderer>().material = originalMaterial;

        // Weakness 오브젝트 활성화
        weaknessObject.SetActive(false);
    }

    // Vignette를 사용하여 서서히 시야 좁히기
    void activateSkillSight()
    {
        // PostProcessVolume에서 Vignette 설정 값을 가져옴
        postProcessingVolumeObject.profile.TryGet(out vignette);

        // TODO: 기존 Vignette value 저장
        originalVignetteColorValue = vignette.color.value;
        originalVignetteIntensityValue = vignette.intensity.value;

        isSightTransitioning = true;
        sightTransitionTimer = 0f;
    }

    // Vignette를 사용하여 서서히 시야 넓히기
    void deactivateSkillSight()
    {
        isSightBackTransitioning = true;
        sightBackTransitionTimer = 0f;
    }
}
