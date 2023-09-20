using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.PostProcessing; // Post-processing에 관련된 네임스페이스 추가
using static OVRHaptics;

public class HandEffectCollision : MonoBehaviour
{
    public float SkillTriggerDuration;

    public GameObject skillCircle; // "skill"시작을 알리는 오브젝트를 저장할 리스트
    public GameObject LeftPurpleEffect;
    public GameObject RightPurpleEffect;

    public bool canUseSkill = false;

    private ControllerManager controllerManager;
    private DeleteEnemyAttack deleteEnemyAttack;
    private SpawnManager spawnManager;


    public Coroutine reduceSkillCoroutine = null;

    private Collider otherCollider;

    // Weakness 관련 변수
    [SerializeField] private Material transparentMaterial;
    private Material originalMaterial;
    [SerializeField] private GameObject monsterMeshObject; 

    [SerializeField] private GameObject weaknessObject;

    private Transform cam;
    // Assign Post Processing Volume from the Scene
    [SerializeField] private Volume postProcessingVolumeObject;
    private UnityEngine.Rendering.Universal.Vignette vignette;

    private float originalVignetteColorValue;
    private float originalVignetteIntensityValue;

    private void Start()
    {
        // Scene에서 OVRInPlayMode를 찾아 cam에 assign
        cam = GameObject.Find("OVRInPlayMode").transform;

        // Scene에서 PostProcessVolume을 가져옴
        postProcessingVolumeObject = GameObject.Find("Post Processing").GetComponent<Volume>();
        // PostProcessVolume에서 Vignette 설정 값을 가져옴
        postProcessingVolumeObject.profile.TryGet(out vignette);

        skillCircle.SetActive(false);
        LeftPurpleEffect.SetActive(false);
        RightPurpleEffect.SetActive(false);

        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();

        deleteEnemyAttack = GameObject.Find("Eraser").GetComponent<DeleteEnemyAttack>();

        spawnManager = GameObject.Find("StageCore").GetComponent<SpawnManager>();

        // 현재 오브젝트의 Material 컴포넌트 가져와 저장
        originalMaterial = monsterMeshObject.GetComponent<Renderer>().material;
        
        // Weakness 오브젝트 비활성화
        weaknessObject.SetActive(false);
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

        if (reduceSkillCoroutine == null)
        {
            reduceSkillCoroutine = StartCoroutine("reduceSkillGauge");  
        }

        spawnManager.BasicSpawnStop(true); 
        spawnManager.StoneSpawnStop(true); 
        spawnManager.SpecialOrbSpawnAllStop(true); 

        deleteEnemyAttack.StartCoroutine("DeleteAll"); 

        turnMonsterTransparent();

        // TODO: 기존 Vignette value 저장
        
        // TODO: Vignette의 color 검은색으로 변경

        // TODO: Vignette의 intensity 특정 값으로 변경

    }

    public IEnumerator reduceSkillGauge()  //스킬 시작되고 20초동안 게이지가 감소 후 종료
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
        turnMonsterOpaque();

        LeftPurpleEffect.SetActive(false);
        RightPurpleEffect.SetActive(false);

        spawnManager.activeSkill = false; 

        skillCircle.SetActive(false);

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
}
