using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRHaptics;

public class HandEffectCollision : MonoBehaviour
{
    public OVRInput.Controller controllerType; // 컨트롤러 종류 선택
    public float vibrationDuration = 2.0f; // 햅틱 지속 시간

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandEffect"))
        {
            Debug.Log("Collision Detected with: " + other.name);

            // Oculus 컨트롤러에서 햅틱 반응을 발생시킵니다.
            OVRHapticsClip clip = new OVRHapticsClip();
            for (int i = 0; i < Config.SampleRateHz * vibrationDuration; i++)
            {
                byte sample = (byte)(1.0f * 255);
                clip.WriteSample(sample);
            }

            OVRHapticsChannel hapticsChannel = (controllerType == OVRInput.Controller.LTouch) ? LeftChannel : RightChannel;
            hapticsChannel.Preempt(clip);

            // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
            StartCoroutine(StopVibration());

            // Deactivate children of this GameObject
            DeactivateChildren(this.gameObject);

            // Deactivate children of the colliding GameObject
            DeactivateChildren(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HandEffect"))
        {
            ReactivateChildren(this.gameObject);

            ReactivateChildren(other.gameObject);
        }
    }

    private void DeactivateChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Debug.Log("Deactivating: " + child.name);
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
        OVRHapticsChannel hapticsChannel = (controllerType == OVRInput.Controller.LTouch) ? LeftChannel : RightChannel;
        hapticsChannel.Clear();
    }
}
