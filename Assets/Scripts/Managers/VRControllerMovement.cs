using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRControllerMovement : MonoBehaviour
{
    private GameObject LeftEyeInteractor;
    private GameObject RightEyeInteractor;

    private EyeTrackingRay eyeTrackingRayLeft;
    private EyeTrackingRay eyeTrackingRayRight;

    public OVRInput.Controller controllerType; // 컨트롤러 종류 선택

    [SerializeField] private AudioSource selectSound;
    [SerializeField] private Image blackImg;

    StageManagerScript stageManager;  


    void Start()
    {
        //LeftEyeInteractor = GameObject.Find("LeftEyeInteractor");
        RightEyeInteractor = GameObject.Find("RightEyeInteractor");
        
        //eyeTrackingRayLeft = LeftEyeInteractor.GetComponent<EyeTrackingRay>();
        eyeTrackingRayRight = RightEyeInteractor.GetComponent<EyeTrackingRay>();

        stageManager = FindObjectOfType<StageManagerScript>();
    }

    void Update()
    {
        if(eyeTrackingRayRight.HoveredCube != null)
            BtnDown();
    }

    void BtnDown()
    {
        Debug.Log("눌렀음");
        if((OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) 
            && eyeTrackingRayRight.HoveredCube != null)
        {
            if(eyeTrackingRayRight.HoveredCube.transform.gameObject.CompareTag("stage"))
            {   
                if (selectSound != null)
                {
                    selectSound.Play();
                }

                if(eyeTrackingRayRight.HoveredCube.gameObject.name == "Stage1Effect" || eyeTrackingRayRight.HoveredCube.gameObject.name == "stage1"){
                    FadeOut(1); // stage 1
                }

                if(eyeTrackingRayRight.HoveredCube.gameObject.name == "Stage2Effect" || eyeTrackingRayRight.HoveredCube.gameObject.name == "stage2"){
                    FadeOut(2); // stage 2
                }

                if(eyeTrackingRayRight.HoveredCube.gameObject.name == "Stage3Effect" || eyeTrackingRayRight.HoveredCube.gameObject.name == "stage3"){
                    FadeOut(3); // stage 3
                }
                
                if(eyeTrackingRayRight.HoveredCube.gameObject.name == "dragon"){       //튜토리얼로
                    FadeOut(4); // stage 3
                }

                
            }
        }
    }

    public void FadeOut(int stageNum){
        StartCoroutine(FadeCoroutine(stageNum));
    }

    IEnumerator FadeCoroutine(int stageNum){
        float fadeCount = 0; //처음 알파값
        while(fadeCount < 1.0f){
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            blackImg.color = new Color(0, 0, 0, fadeCount);
        }

        stageManager.GoStage(stageNum);
    }
}
