using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRHaptics;

public class ControllerManager : MonoBehaviour
{
    private GameObject LeftEyeInteractor;
    private GameObject RightEyeInteractor;

    private EyeTrackingRay eyeTrackingRayLeft;
    private EyeTrackingRay eyeTrackingRayRight;

    public OVRInput.Controller controllerType; // 컨트롤러 종류 선택
    public float vibrationDuration = 1.0f; // 햅틱 지속 시간

    // Start is called before the first frame update
    void Start()
    {
        //LeftEyeInteractor = GameObject.Find("LeftEyeInteractor");
        RightEyeInteractor = GameObject.Find("RightEyeInteractor");
        
        //eyeTrackingRayLeft = LeftEyeInteractor.GetComponent<EyeTrackingRay>();
        eyeTrackingRayRight = RightEyeInteractor.GetComponent<EyeTrackingRay>();
    }

    // Update is called once per frame
    void Update()
    {
        BtnDown();
    }

    void BtnDown()
    {
        if(OVRInput.GetDown(OVRInput.Button.One))
        {
            OVRHapticsClip clip = new OVRHapticsClip();
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("redCube")
                && eyeTrackingRayRight.HoveredCube != null)
                Destroy(eyeTrackingRayRight.HoveredCube);

            OVRHapticsChannel hapticsChannel = (controllerType == OVRInput.Controller.RTouch) ? RightChannel : LeftChannel;
            hapticsChannel.Preempt(clip);

            StartCoroutine(StopVibration());
        }

        if(OVRInput.GetDown(OVRInput.Button.Three))
        {
            OVRHapticsClip clip = new OVRHapticsClip();
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("blueCube")
                && eyeTrackingRayRight.HoveredCube != null)
                Destroy(eyeTrackingRayRight.HoveredCube);
            
            OVRHapticsChannel hapticsChannel = (controllerType == OVRInput.Controller.RTouch) ? LeftChannel : RightChannel;
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
