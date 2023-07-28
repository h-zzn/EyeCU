using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageManagerScript : MonoBehaviour
{

    int levelat; // 현재 스테이지 번호
    public GameObject stageNumObject;

    public Sprite inactiveButtonImage;

    private static bool hasDeletedKey = false;

    private void Start()
    {
        if (!hasDeletedKey)
        {
            PlayerPrefs.DeleteKey("levelReached");
            hasDeletedKey = true;
        }

        Button[] stages = stageNumObject.GetComponentsInChildren<Button>();

        levelat = PlayerPrefs.GetInt("levelReached");
        
        print(levelat);
        for (int i= levelat+1; i<stages.Length; i++){
            stages[i].interactable = false;
            stages[i-1].GetComponent<Image>().sprite = inactiveButtonImage; // 비활성화 이미지 설정
        }
    }

    public void GoStage(int stageNum){
        SceneManager.LoadScene(stageNum); 
    }
}
