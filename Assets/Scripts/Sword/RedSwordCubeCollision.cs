using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRHaptics;

public class RedSwordCubeCollision : MonoBehaviour
{
    public OVRInput.Controller controllerType; // 컨트롤러 종류 선택
    public float vibrationDuration = 1.0f; // 햅틱 지속 시간

    [SerializeField] private AudioSource SsingSound;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("redCube"))
        {
            if (SsingSound != null)
            {
                SsingSound.Play();
            }
            // 충돌한 오브젝트가 blueCube 태그를 가지고 있다면 해당 오브젝트를 삭제합니다.
            Destroy(collision.gameObject);

            // Oculus 컨트롤러에서 햅틱 반응을 발생시킵니다.
            OVRHapticsClip clip = new OVRHapticsClip();
            for (int i = 0; i < Config.SampleRateHz * vibrationDuration; i++)
            {
                byte sample = (byte)(1.0f * 255);
                clip.WriteSample(sample);
            }

            OVRHapticsChannel hapticsChannel = (controllerType == OVRInput.Controller.RTouch) ? RightChannel : LeftChannel;
            hapticsChannel.Preempt(clip);

            // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
            StartCoroutine(StopVibration());
        }
    }

    private IEnumerator StopVibration()
    {
        // 햅틱 반응 시간 이후에 반응을 중지시킵니다.
        yield return new WaitForSeconds(vibrationDuration);

        // Oculus 컨트롤러의 햅틱 반응을 중지합니다.
        OVRHapticsChannel hapticsChannel = (controllerType == OVRInput.Controller.RTouch) ? RightChannel : LeftChannel;
        hapticsChannel.Clear();
    }
}

