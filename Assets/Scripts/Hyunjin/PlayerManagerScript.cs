using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManagerScript : MonoBehaviour
{
    StageGameManager stageGameManager;
    public Image blackImg;

    void Start(){
        stageGameManager = FindObjectOfType<StageGameManager>();
    }

    private void OnTriggerEnter(Collider collision){
        if(collision.tag == "Item"){
            stageGameManager.GetScore();
            GameObject.Destroy(collision.gameObject);
        }

        if(collision.tag == "gameOver"){
            stageGameManager.GetScore(); // 제대로 작동되는지 확인 위함
            stageGameManager.GameOver();
        }

        if(collision.tag == "gameStartButton"){
            FadeOut(0);
        }

        if(collision.tag == "tutorialButton"){
            FadeOut(2);
        }
    }

    public void GoStage(int stageNum){
        SceneManager.LoadScene(stageNum); 
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

        GoStage(stageNum);
    }
}