using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageManagerScript : MonoBehaviour
{

    int levelat; // 현재 스테이지 번호
    int bestHP; 
    public GameObject stageNumObject;
    public GameObject successGroupObject;

    private static bool hasDeletedKey = false;

    private void Start()
    {
        if (!hasDeletedKey)
        {
            PlayerPrefs.DeleteKey("levelReached");
            hasDeletedKey = true;
        }

        GameObject[] stages = new GameObject[stageNumObject.transform.childCount];
        for (int i = 0; i < stageNumObject.transform.childCount; i++)
        {
            stages[i] = stageNumObject.transform.GetChild(i).gameObject;
        }

        GameObject[] successBlock = new GameObject[successGroupObject.transform.childCount];
        for (int i = 0; i < successGroupObject.transform.childCount; i++)
        {
            successBlock[i] = successGroupObject.transform.GetChild(i).gameObject;
        }

        

        levelat = PlayerPrefs.GetInt("levelReached");
        
        print(levelat);
        for (int i= levelat+1; i<stages.Length; i++){
            stages[i].SetActive(false);
        }

        //완료한 스테이지 비활성화 
        if(levelat > 0){
            
            for(int i = levelat; i<=levelat; i++){
                stages[i-1].SetActive(false);
                successBlock[i-1].SetActive(true);

                successBlock[i+2].transform.GetChild(0).gameObject.SetActive(true);

                bestHP = PlayerPrefs.GetInt("BestHP");
                print("Best HP: " + bestHP);

                if(bestHP >= 1700){
                    successBlock[i+2].transform.GetChild(1).gameObject.SetActive(true);
                    successBlock[i+2].transform.GetChild(2).gameObject.SetActive(true);
                }

                else if(bestHP >= 1000){
                    successBlock[i+1].transform.GetChild(1).gameObject.SetActive(true);
                }
                
            }
        }
    }

    public void GoStage(int stageNum){
        SceneManager.LoadScene(stageNum); 
    }
}
