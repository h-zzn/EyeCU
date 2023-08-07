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
        activeSword();
    }

    void BtnDown()
    {
        if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && eyeTrackingRayRight.HoveredCube != null)
        {
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("redCube"))
            {   
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
