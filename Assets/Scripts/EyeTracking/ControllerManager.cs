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

    Vector3 hitEffectPosition;

    [SerializeField] private GameObject RedMagicHitEffectPrefab;
    [SerializeField] private GameObject BlueMagicHitEffectPrefab;

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

    public float vibrationDuration = 0.5f; // 햅틱 지속 시간

    public int MissingPoint = 0;

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
        if(eyeTrackingRayRight.HoveredCube != null)
            BtnDown();
        
        activeSword();
    }

    void BtnDown()
    {
        // 오른손 Red Magic
        if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && eyeTrackingRayRight.HoveredCube != null)
        {
            // O Correct
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("redCube"))
            {   
                if (redMagicActive){ // 오른손 Red Magic이 사용 가능일 때 
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
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && eyeTrackingRayRight.HoveredCube != null)
        {
            // O Correct
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("blueCube"))
            {
                if(blueMagicActive){ // 오른손 Blue Magic이 사용 가능일 때
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

    void activeSword()
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
