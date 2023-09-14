using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRHaptics;

public class HandEffectCollision : MonoBehaviour
{
    public float vibrationDuration = 2.0f; // 햅틱 지속 시간
    public GameObject skillCircle; // "skill" 오브젝트를 저장할 리스트
    private bool canUseSkill = false;
    private bool hasCollided = false;

    private ControllerManager controllerManager;

    private void Start()
    {
        skillCircle.SetActive(false); 

        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (controllerManager.attackPoint >= 2000 && !hasCollided && other.CompareTag("HandEffect"))
        {
            Debug.Log("Collision Detected with: " + other.name);

            // Oculus 컨트롤러에서 햅틱 반응을 발생시킵니다.
            OVRHapticsClip clip = new OVRHapticsClip();
            for (int i = 0; i < Config.SampleRateHz * vibrationDuration; i++)
            {
                byte sample = (byte)(1.0f * 255);
                clip.WriteSample(sample);
            }

            LeftChannel.Preempt(clip);
            RightChannel.Preempt(clip);

            // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
            StartCoroutine(StopVibration());

            // Deactivate children of this GameObject
            DeactivateChildren(this.gameObject);
            DeactivateChildren(other.gameObject);

            // 활성화되지 않은 "skill" 오브젝트를 찾아서 활성화

            skillCircle.SetActive(true);

            hasCollided = true;
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

    private IEnumerator StopVibration()
    {
        // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
        yield return new WaitForSeconds(vibrationDuration);

        // Oculus 컨트롤러의 햅틱 반응을 중지합니다.
        LeftChannel.Clear();
        RightChannel.Clear();
    }
}
