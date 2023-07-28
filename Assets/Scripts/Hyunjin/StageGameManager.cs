using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageGameManager : MonoBehaviour
{
    // 플레이 도중 점수 관련
    public Text currentScoreText;
    int score;

    // Final 점수 관련
    public Text finalScoreText;
    public GameObject[] gameItems;  // 게임이 끝났을 때 파괴할 게임 오브젝트 배열
    private int finalScore;

    private static bool hasDeletedKey = false;
    
    // 플레이 도중 점수 관련
    private void Start(){
        //PlayerPrefs.DeleteKey("FinalScore");   원래 게임에서는 밑에 if문 말고 얘만 있으면 될걸
        //PlayerPrefs.DeleteKey("levelReached");

        if (!hasDeletedKey)
        {
            PlayerPrefs.DeleteKey("FinalScore");
            hasDeletedKey = true;
        }
        
        finalScore = PlayerPrefs.GetInt("FinalScore");

        if(PlayerPrefs.HasKey("FinalScore")){       //Final score가 저장되어 있으면 바로 gameOver 실행
            GameOver();
        }
        else{
            SetText(); 
        }
       
    }

    public void GetScore(){
        score += 100;
        SetText();
    }

    public void SetText(){
        currentScoreText.text = "Score: " + score.ToString();
    }

    // Final 점수 관련
    public void GameOver(){
        //finalScore = score;

        if(!PlayerPrefs.HasKey("FinalScore")){
            finalScore = score;
            Save(finalScore);
        }
    
        
        // print("final Score : " + finalScore);
        // finalScore = PlayerPrefs.GetInt("FinalScore");
        // Save(finalScore); // 점수 데이터 저장

        //게임오버 UI
        currentScoreText.enabled = false; // 현재 스코어 안보이게
        
        for(int i=0; i<gameItems.Length; i++){
            Destroy(gameItems[i]);
        }
        //finalScoreText.enabled = true;
        
        finalScoreText.text = finalScore.ToString();

        PlayerPrefs.SetInt("levelReached", SceneManager.GetActiveScene().buildIndex);
    }

    public void Save(int finalScore){         // 점수 데이터 저장
        PlayerPrefs.SetInt("FinalScore", finalScore); // PlayerPrefs.SetInt: 현 컴퓨터내의 레지스트리에 등록한다는 것
    }

    public void GoHome(){
        SceneManager.LoadScene(0); 
    }
}
