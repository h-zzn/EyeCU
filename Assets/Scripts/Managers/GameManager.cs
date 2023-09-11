using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource BGM; 
    [SerializeField] private AudioSource StarSFX;
    [SerializeField] private float fadeDuration = 2.0f;

    private EventManager eventManager; 
    private DamagedArea damagedArea;

    private Coroutine stageOver = null;
    private Coroutine stageClear = null; 

    [SerializeField] private float startDelayTime;
    //[SerializeField] private static bool hasDeletedKey = true;
    [SerializeField] private GameObject successUI;  
    [SerializeField] private GameObject failUI; 
    [SerializeField] private GameObject starObj; 

    private bool eventStarted = false;

    private float Timer = 0;

    public Coroutine EventFlow = null;

    // final HP ����
    public Text finalHPText;
    private int finalHP;

    private static bool hasDeletedKey = false;

    public int HPcontrol;

    int levelReached;

    private enum StageLevel
    {
        tutorial, 
        stage1, 
        stage2, 
        stage3, 
        stage4, 
        stage5, 
        stage6,
        stageDDA
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

        print("hasDeletedKey = " + hasDeletedKey);
        print("hasFinalHP = " + PlayerPrefs.HasKey("FinalHP"));

        finalHP = PlayerPrefs.GetInt("FinalHP");

        // if(PlayerPrefs.HasKey("FinalHP")){
        //     StageClear();
        // }
        if (stageLevel != StageLevel.tutorial)
        {
            Invoke("AutoHealingHP", 2f);
        }
    }

    void Update() 
    {
        //print("damagedArea.stageHP :"+ damagedArea.stageHP);
        if (damagedArea.stageHP <= 0)
        {
            if(stageLevel != StageLevel.tutorial){
                if(stageOver == null)
                    stageOver = StartCoroutine(StageOver());  // StageOver 
                eventManager.EventFlow = null;  
            }
            else
            { 
                GoHome();
            }
        }
        
        if(damagedArea.stageHP > 0 && eventManager.GameClear == true)
        {
            if(stageLevel != StageLevel.tutorial){
                if(stageClear == null)
                    stageClear = StartCoroutine(StageClear()); 
            }
            else{ 
                GoHome();
            }
        }
        else if(stageLevel == StageLevel.tutorial)
        {
            damagedArea.stageHP = 2000;
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
                    EventFlow = StartCoroutine(eventManager.Stage1EventFlow());  
                else if(stageLevel == StageLevel.stage2)
                    EventFlow = StartCoroutine(eventManager.Stage2EventFlow());
                else if(stageLevel == StageLevel.stage3)
                    EventFlow = StartCoroutine(eventManager.Stage3EventFlow());
                else 
                    EventFlow = StartCoroutine(eventManager.Stage3EventFlow());
            }
        }
        Timer += Time.deltaTime;
    } 


    public IEnumerator StageOver()
    {
        float startVolume = BGM.volume; // Store the initial volume

        // Fade out the BGM
        while (BGM.volume > 0)
        {
            BGM.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        BGM.Stop(); // Stop the BGM when the fade-out is complete

        ActiveGameOverWindow();
        yield return new WaitForSeconds(5);
        GoHome(); 
    }


    public IEnumerator StageClear() 
    {
        // if(!PlayerPrefs.HasKey("FinalHP")){
        //     finalHP = damagedArea.stageHP;
        //     SaveHP(finalHP);
        // }
        //print("FinalHP");
        finalHP = damagedArea.stageHP;
        SaveHP(finalHP); 


        finalHPText.text = finalHP.ToString();
        levelReached = PlayerPrefs.GetInt("levelReached");  

        if(levelReached < SceneManager.GetActiveScene().buildIndex)
            PlayerPrefs.SetInt("levelReached", SceneManager.GetActiveScene().buildIndex);

        float startVolume = BGM.volume; // Store the initial volume

        // Fade out the BGM
        while (BGM.volume > 0)
        {
            BGM.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        BGM.Stop(); // Stop the BGM when the fade-out is complete
         
        ActiveGameClearWindow(); 
        yield return new WaitForSeconds(5); 
        GoHome(); 
    }

    public void BGMOff()
    {
        if(BGM != null)
        {
            BGM.Stop(); 
        }
    }

    private void ActiveGameOverWindow()
    {
        Debug.Log("GameOver");
        failUI.SetActive(true);

    }

    private void ActiveGameClearWindow()
    {
        finalHPText.enabled = true;
        successUI.SetActive(true);
        StartCoroutine(ActiveStar());
    }

    public void GoHome()
    {
        if(stageLevel == StageLevel.stageDDA)
        {
            SceneManager.LoadScene("StageDDA"); 
        }
        else
        {
            SceneManager.LoadScene(0); 
        }       
    }

    public void SaveHP(int finalHP){ 
        //finalHP = HPcontrol; //bestHP Ȯ���Ҷ�

        // stage 1�϶� 
        if(SceneManager.GetActiveScene().buildIndex == 1){
            print("stage 1 finalHP Ȯ��: " + finalHP);
            if(finalHP > PlayerPrefs.GetInt("Stage1BestHP") || !PlayerPrefs.HasKey("Stage1BestHP")){
                print("set Stage1BestHP = " + PlayerPrefs.GetInt("Stage1BestHP"));
                PlayerPrefs.SetInt("Stage1BestHP", finalHP); 
            }   
        }

        // stage 2�϶� 
        if(SceneManager.GetActiveScene().buildIndex == 2){
            if(finalHP > PlayerPrefs.GetInt("Stage2BestHP") || !PlayerPrefs.HasKey("Stage2BestHP")){
                PlayerPrefs.SetInt("Stage2BestHP", finalHP); 
                print("set Stage2BestHP = " + PlayerPrefs.GetInt("Stage2BestHP"));
            }   
        }

        // stage 3�϶� 
        if(SceneManager.GetActiveScene().buildIndex == 3){
            if(finalHP > PlayerPrefs.GetInt("Stage3BestHP") || !PlayerPrefs.HasKey("Stage3BestHP")){
                print("set Stage3BestHP = " + PlayerPrefs.GetInt("Stage3BestHP"));
                PlayerPrefs.SetInt("Stage3BestHP", finalHP); 
            }   
        } 
    }

    private IEnumerator ActiveStar()
    {
        starObj.transform.GetChild(3).gameObject.SetActive(true);  
        yield return new WaitForSeconds(2.37f);
        StarSFX.Play();

        if(damagedArea.stageHP >= 1700){
            starObj.transform.GetChild(4).gameObject.SetActive(true); 
            starObj.transform.GetChild(5).gameObject.SetActive(true); 
            yield return new WaitForSeconds(1.03f);
            StarSFX.Play();
            yield return new WaitForSeconds(0.99f);
            StarSFX.Play();
        }
        else if(damagedArea.stageHP >= 1000){
            starObj.transform.GetChild(4).gameObject.SetActive(true);   
            yield return new WaitForSeconds(1.03f);
            StarSFX.Play();
        }
    }

    private void AutoHealingHP()
    {
        if (damagedArea.stageHP <= 1800)
        {
            damagedArea.stageHP += 200;
        }
        else
        {
            damagedArea.stageHP = 2000;
        }
    }
}
