using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    // final HP 관련
    public Text finalHPText;
    private int finalHP;

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
        finalHPText.enabled = false;

        if (!hasDeletedKey)
        {
            PlayerPrefs.DeleteKey("FinalHP");
            hasDeletedKey = true;
        }

        finalHP = PlayerPrefs.GetInt("FinalHP");

        if(PlayerPrefs.HasKey("FinalHP")){
            StageClear();
        }
    }

    void Update() 
    {
        //print("damagedArea.stageHP :"+ damagedArea.stageHP);
        if(damagedArea.stageHP <= 0)
        {
            if(stageOver == null)
                stageOver = StartCoroutine(StageClear());       // 이거 StageOver로 바꾸기
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
            finalHP = damagedArea.stageHP;
            SaveHP(finalHP);
        }

        // finalHPText.text = finalHP.ToString();

        PlayerPrefs.SetInt("levelReached", SceneManager.GetActiveScene().buildIndex);

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
        Debug.Log("damagedArea.stageHP : " + damagedArea.stageHP);
        finalHPText.enabled = true;
    }

    public void GoHome(){
        SceneManager.LoadScene(0); 
    }

    public void SaveHP(int finalHP){         // 점수 데이터 저장
        PlayerPrefs.SetInt("FinalHP", finalHP); // PlayerPrefs.SetInt: 현 컴퓨터내의 레지스트리에 등록한다는 것
    }
}
