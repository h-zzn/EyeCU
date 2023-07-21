using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    private GameObject LeftEyeInteractor;
    private GameObject RightEyeInteractor;

    private EyeTrackingRay eyeTrackingRayLeft;
    private EyeTrackingRay eyeTrackingRayRight;

    // Start is called before the first frame update
    void Start()
    {
        LeftEyeInteractor = GameObject.Find("LeftEyeInteractor");
        RightEyeInteractor = GameObject.Find("RightEyeInteractor");
        
        eyeTrackingRayLeft = LeftEyeInteractor.GetComponent<EyeTrackingRay>();
        eyeTrackingRayRight = LeftEyeInteractor.GetComponent<EyeTrackingRay>();
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
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("redCube"))
                Destroy(eyeTrackingRayRight.HoveredCube);
        }

        if(OVRInput.GetDown(OVRInput.Button.Three))
        {
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("blueCube"))
                Destroy(eyeTrackingRayLeft.HoveredCube);
        }
    }
}
