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
            Debug.Log("collision!");
            FadeOut(1);
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
