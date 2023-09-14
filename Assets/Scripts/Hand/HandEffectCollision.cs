using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRHaptics;

public class HandEffectCollision : MonoBehaviour
{
    public float SKillTriggerDuration;

    public GameObject skillCircle; // "skill"시작을 알리는 오브젝트를 저장할 리스트

    public bool canUseSkill = false;

    private ControllerManager controllerManager;

    public Coroutine reduceSkillCoroutine = null;


    private void Start()
    {
        skillCircle.SetActive(false); 

        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (controllerManager.attackPoint >= 2000 && other.CompareTag("HandEffect"))
        {
            // Deactivate children of this GameObject
            DeactivateChildren(this.gameObject);
            DeactivateChildren(other.gameObject);

            StartVibration();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (controllerManager.attackPoint >= 2000 && other.CompareTag("HandEffect"))
        {
            if(SKillTriggerDuration <= 3)
            {
                SKillTriggerDuration += Time.deltaTime;
            }
            else if (canUseSkill == false)
            {
                ActiveSkill();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (controllerManager.attackPoint >= 2000 && other.CompareTag("HandEffect"))
        {
            SKillTriggerDuration = 0;

            StopVibration();

            ReactivateChildren(this.gameObject); 
            ReactivateChildren(other.gameObject); 
        }
    }

    private void ActiveSkill() 
    {
        canUseSkill = true;
        StopVibration();
       
        //활성화되지 않은 "skill" 오브젝트를 찾아서 활성화
        skillCircle.SetActive(true);

        if (reduceSkillCoroutine == null)
        {
            reduceSkillCoroutine = StartCoroutine("reduceSkillGauge");  
        } 

        //시각 변화 함수 만들어서 넣어줘요 (예시. 드레곤의 약점만 강조되고 다른 것들은 흑백) +용석
    }

    public IEnumerator reduceSkillGauge()  //스킬 시작되고 20초동안 게이지가 감소 후 종료
    {
        while (controllerManager.skillEnergyPoint > 0)
        {
            yield return new WaitForSeconds(1);

            controllerManager.skillEnergyPoint -= 100;

            if (controllerManager.skillEnergyPoint < 100)
            {
                controllerManager.skillEnergyPoint = 0;
            }
        }

        canUseSkill = false;

        //시각 변화 사라지고 원래로 돌려주는 함수 넣어줘요 +용석
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
}
