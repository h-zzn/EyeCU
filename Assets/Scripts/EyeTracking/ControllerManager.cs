using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private float deactivateRedMagicTime = 1f;
    [SerializeField] private float deactivateBlueMagicTime = 1f;

    private bool redMagicActive = false;
    private bool blueMagicActive = false;

    private Coroutine redMagicPauseCoroutine; // Coroutine 참조 변수
    private Coroutine blueMagicPauseCoroutine; // Coroutine 참조 변수

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
        if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && eyeTrackingRayRight.HoveredCube != null)
        {
            // O Correct
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("redCube"))
            {   
                if (redMagicActive){
                    if (PoongSound != null)
                    {
                        PoongSound.Play();
                    }

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
                    // // Tik Sound play
                    // if (TikSound != null)
                    // {
                    //     TikSound.Play();
                    // }
                    Debug.Log("Red Magic is not active!!!!!!!!!!!!!!!!!!!!!");
                }
            }

            // X Wrong
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("blueCube"))
            {
                // // Wrong Sound play
                // if (WrongSound != null)
                // {
                //     WrongSound.Play();
                // }
                Debug.Log("Wrong Sound play!!!!!!!!!!!!!!!!!!!!");

                if (redMagicActive) // redMagicActive가 활성화되어 있을 때
                {
                    Debug.Log("Wrong Target! : Red Magic Deactivated for 1 second!!!!!!!!!!!!!!!!!!!!!");

                    redMagicActive = false; // 일단 true로 설정
                    // 1초 뒤에 redMagicActive 다시 false로 변경하는 Coroutine 시작
                    if (redMagicPauseCoroutine != null)
                    {
                        StopCoroutine(redMagicPauseCoroutine); // 기존 Coroutine 중지
                    }
                    redMagicPauseCoroutine = StartCoroutine(ActivateRedMagicAfter(deactivateRedMagicTime));
                }
            }
        }

        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && eyeTrackingRayRight.HoveredCube != null)
        {
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("blueCube"))
            {
                if (PoongSound != null)
                {
                    PoongSound.Play();
                }

                // Magic hit effect play at eyeTrackingRayRight.HoveredCube.transform.position
                hitEffectPosition = eyeTrackingRayRight.HoveredCube.transform.position;
                GameObject blueMagicHitInstance = Instantiate(BlueMagicHitEffectPrefab, hitEffectPosition, Quaternion.identity);
                blueMagicHitInstance.SetActive(true);
                Destroy(blueMagicHitInstance,3f);

                Destroy(eyeTrackingRayRight.HoveredCube);
                eyeTrackingRayRight.HoveredCube = null;
            }
        }
    }

    IEnumerator ActivateRedMagicAfter(float second)
    {
        yield return new WaitForSeconds(second);
        redMagicActive = true;
        
        Debug.Log("Red Magic Activated!!!!!!!!!!!!!!!!!!");
    }

    IEnumerator ActivateBlueMagicAfter(float second)
    {
        yield return new WaitForSeconds(second);
        blueMagicActive = true;

        Debug.Log("Blue Magic Activated!!!!!!!!!!!!!!!!!!");
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
