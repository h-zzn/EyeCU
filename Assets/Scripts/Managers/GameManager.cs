using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource BGM; 

    private EventManager eventManager; 
    private DamagedArea damagedArea;

    private Coroutine stageOver = null;
    private Coroutine stageClear = null; 

    [SerializeField] private float startDelayTime;
    [SerializeField] private static bool hasDeletedKey = true; 

    private bool eventStarted = false;

    private float Timer = 0;

    public Coroutine EventFlow = null;

    private static bool hasDeletedKey = false; 

    // final HP 관련
    public Text finalHPText;
    private int finalHP;

    // 플레이 도중 점수 관련
    public Text currentScoreText;
    int score;

    private enum StageLevel
    {
        tutorial, 
        stage1, 
        stage2, 
        stage3, 
        stage4, 
        stage5, 
        stage6
    }

    [SerializeField] private StageLevel stageLevel;

    void Awake()
    {
        eventManager = this.transform.GetComponent<EventManager>(); 
        damagedArea = this.transform.GetComponent<DamagedArea>();
    }

    void Start()
    {
        if (!hasDeletedKey)
        {
            PlayerPrefs.DeleteKey("FinalHP");
            hasDeletedKey = true;
        }

        finalHP = PlayerPrefs.GetInt("FinalHP");

        if(PlayerPrefs.HasKey("FinalHP")){
            StageClear();
        }
        else{
            SetText();
        }
    }

    void Update() 
    {
        if(damagedArea.stageHP <= 0)
        {
            if(stageOver == null)
                stageOver = StartCoroutine(StageOver()); 
            eventManager.EventFlow = null; 
        }

        if(damagedArea.stageHP > 0 && eventManager.GameClear == true)
        {
            if(stageClear == null)
                stageClear = StartCoroutine(StageClear()); 
        }

        // Wait for startDelayTime
        if (Timer >= startDelayTime && eventStarted == false) 
        {
            eventStarted = true;

            // Start a coroutine to handle the event flow
            if(EventFlow == null)
            {
                if(stageLevel == StageLevel.tutorial)
                    EventFlow = StartCoroutine(eventManager.TutorialEventFlow());  
                else if(stageLevel == StageLevel.stage1)
                    EventFlow = StartCoroutine(eventManager.EventFlowCoroutine());  
            }
        }

        Timer += Time.deltaTime;
    }
    
    public void GetScore(){
        score += 100;
        SetText();
    }

    public void SetText(){
        currentScoreText.text = "Score: " + score.ToString();
    }


    public IEnumerator StageOver() 
    {
        yield return new WaitForSeconds(2); 
        BGMOff();
        ActiveGameOverWindow();
        yield return new WaitForSeconds(5); 
        GoHome();
    }  

    public IEnumerator StageClear() 
    {
        if(!PlayerPrefs.HasKey("FinalHP")){
            finalScore = score;
            SaveHP(finalScore);
        }

        SaveHP(damagedArea.stageHP);
        yield return new WaitForSeconds(2); 
        BGMOff();
        ActiveGameClearWindow();
        yield return new WaitForSeconds(5); 
        GoHome();
    }

    public void BGMOff()
    {
        if (BGM != null)
        {
            BGM.Stop(); 
        }
    }

    private void ActiveGameOverWindow()
    {
        Debug.Log("GameOver");

    }

    private void ActiveGameClearWindow()
    {
        Debug.Log("GameClear");

    }

    public void GoHome(){
        SceneManager.LoadScene(0); 
    }

    public void SaveHP(int finalHP){         // 점수 데이터 저장
        PlayerPrefs.SetInt("FinalHP", finalHP); // PlayerPrefs.SetInt: 현 컴퓨터내의 레지스트리에 등록한다는 것
    }
}
