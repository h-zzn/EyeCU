using UnityEngine;
using static OVRHaptics;

public class SwordCollision : MonoBehaviour
{
    public OVRInput.Controller controllerType; // 컨트롤러 종류 선택
    public float vibrationDuration = 0.2f; // 햅틱 지속 시간
    public float vibrationIntensity = 0.5f; // 햅틱 진동 강도 (0.0f부터 1.0f 사이)

    private bool isVibrating = false;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("blueSword") || collision.gameObject.CompareTag("redSword"))
        {
            // 두 Sword 오브젝트가 충돌했을 때만 햅틱 반응을 발생시킵니다.
            if (!isVibrating)
            {
                StartVibration();
            }
        }
    }

    private void StartVibration()
    {
        // Oculus 컨트롤러에서 햅틱 반응을 시작합니다.
        OVRHapticsChannel hapticsChannel = OVRHaptics.LeftChannel; // 왼손 컨트롤러에 적용
        if (controllerType == OVRInput.Controller.RTouch)
            hapticsChannel = OVRHaptics.RightChannel; // 오른손 컨트롤러에 적용

        OVRHapticsClip clip = new OVRHapticsClip();
        for (int i = 0; i < OVRHaptics.Config.SampleRateHz * vibrationDuration; i++)
        {
            byte sample = (byte)(vibrationIntensity * 255);
            clip.WriteSample(sample);
        }

        hapticsChannel.Preempt(clip);

        isVibrating = true;

        // 지정된 시간 이후에 햅틱 반응을 정지시킵니다.
        Invoke("StopVibration", vibrationDuration);
    }

    private void StopVibration()
    {
        // Oculus 컨트롤러에서 햅틱 반응을 정지합니다.
        OVRHapticsChannel hapticsChannel = OVRHaptics.LeftChannel; // 왼손 컨트롤러에 적용
        if (controllerType == OVRInput.Controller.RTouch)
            hapticsChannel = OVRHaptics.RightChannel; // 오른손 컨트롤러에 적용

        hapticsChannel.Clear();

        isVibrating = false;
    }
}
