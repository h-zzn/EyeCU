using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageManagerScript : MonoBehaviour
{

    int levelat; // 현재 스테이지 번호
    int stage1BestHP; 
    int stage2BestHP;
    int stage3BestHP;
    int stageCheckValue;
    
    public GameObject stageNumObject;
    public GameObject successGroupObject;

    public Image blackImg;

    private static bool hasDeletedKey = false;

    private void Start()
    {
        // if (!hasDeletedKey)
        // {
        //     PlayerPrefs.DeleteKey("levelReached");
        //     PlayerPrefs.DeleteKey("knifeActive");
        //     hasDeletedKey = true;
        // }

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
        stage1BestHP = PlayerPrefs.GetInt("Stage1BestHP");
        stage2BestHP = PlayerPrefs.GetInt("Stage2BestHP");
        stage3BestHP = PlayerPrefs.GetInt("Stage3BestHP");


        for (int i= levelat+1; i<stages.Length; i++){
            stages[i].SetActive(false);         // 시작할 때 스테이지 비활성화 /////////////////////////////////////////////////////////////////////////// 나중에 수정
        }

        //완료한 스테이지 비활성화 
        if(levelat > 0){
            for(int i = 0; i<levelat; i++){
                if(stages[i].gameObject.name != "TutorialObj")
                    stages[i].SetActive(false);       // 완료한 스테이지 파티클 효과 비활성화 /////////////////////////////////////////////////////////////////////////// 나중에 수정
                successBlock[i].SetActive(true);  // success 오브젝트 활성화             
            }
        }

        // 완료한 스테이지 별 활성화
        if(levelat >= 1){
            successBlock[0].transform.GetChild(0).gameObject.SetActive(true); // stage 1 첫번째 별 활성화

            if(stage1BestHP >= 1000){
                successBlock[0].transform.GetChild(1).gameObject.SetActive(true);

                if(stage1BestHP >= 1700){
                    successBlock[0].transform.GetChild(2).gameObject.SetActive(true);
                }
            }

            if(levelat>=2){
                successBlock[1].transform.GetChild(0).gameObject.SetActive(true); // stage 1 첫번째 별 활성화

                if(stage2BestHP >= 1000){
                    successBlock[1].transform.GetChild(1).gameObject.SetActive(true);

                    if(stage2BestHP >= 1700){
                        successBlock[1].transform.GetChild(2).gameObject.SetActive(true);
                    }
                }

                if(levelat>=3){
                    successBlock[2].transform.GetChild(0).gameObject.SetActive(true); // stage 3 첫번째 별 활성화

                    if(stage3BestHP >= 1000){
                        successBlock[2].transform.GetChild(1).gameObject.SetActive(true);

                        if(stage3BestHP >= 1700){
                            successBlock[2].transform.GetChild(2).gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public void GoStage(int stageNum){
        SceneManager.LoadScene(stageNum); 
    }
}
