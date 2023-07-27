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

    [SerializeField] private GameObject leftSword;
    [SerializeField] private GameObject rightSword;

    public AudioClip yourAudioClip;
    [SerializeField] private AudioSource PoongSound;

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
                Destroy(eyeTrackingRayRight.HoveredCube);
                eyeTrackingRayRight.HoveredCube = null;
            }
        }
    }

    void activeSword()
    {
        //ON
        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            if (rightSword.activeSelf == false)
            {
                rightSword.SetActive(true);
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            if (leftSword.activeSelf == false)
            {
                leftSword.SetActive(true);
            }
        }

        //OFF
        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            if (rightSword.activeSelf == true)
            {
                rightSword.SetActive(false);
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            if (leftSword.activeSelf == true)
            {
                leftSword.SetActive(false);
            }
        }
    }
}
