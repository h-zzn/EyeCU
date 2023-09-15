using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CheckCollisionScript : MonoBehaviour
{
    StageManagerScript stageManager;  
    public Image blackImg;

    private void Start()
    {
        stageManager = FindObjectOfType<StageManagerScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(this.gameObject.name == "Stage1Effect" || this.gameObject.name == "stage1"){
                FadeOut(1); // stage 1
            }

            if(this.gameObject.name == "Stage2Effect" || this.gameObject.name == "stage2"){
                FadeOut(2); // stage 2
            }

            if(this.gameObject.name == "Stage3Effect" || this.gameObject.name == "stage3"){
                FadeOut(3); // stage 3
            }
            
            if(this.gameObject.name == "TutorialObj"){       //튜토리얼로
                FadeOut(4); // tutorial
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
