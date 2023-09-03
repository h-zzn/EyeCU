using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public int EnemyHP = 1000;

    [SerializeField] private GameObject[] basicOrbSpawner;
    [SerializeField] private GameObject[] stoneSpawner;
    [SerializeField] private GameObject[] SpecialOrbSpawner;

    public bool GameClear = false;
    
    [SerializeField] private float EventStartDelayTime;
    [SerializeField] private float BasicSpawnTime;
    [SerializeField] private float swordTime;

    [SerializeField] private GameObject explainUI; 

    // tutorial UI 
    [SerializeField] private GameObject magicObj;   
    [SerializeField] private GameObject specialObj;   
    [SerializeField] private GameObject stoneObj; 
    [SerializeField] private GameObject finishUI;  

    // Animator 
    Animator animator1A; 
    Animator animator1B; 
    Animator animator1C; 
    Animator animator2A; 
    Animator animator2B; 
    Animator animator3A; 
    Animator animator3B; 

    private float Timer = 0; 

    public Coroutine EventFlow = null; 

    private TutorialEvent tutorialEvent = null;  

    private int TutorialBuildIndex;

    void Start()  
    {
        TutorialBuildIndex = SceneManager.sceneCountInBuildSettings-1;
        if(SceneManager.GetActiveScene().buildIndex == TutorialBuildIndex)  
        {
           tutorialEvent = GameObject.Find("TutorialObjects").GetComponent<TutorialEvent>();    
           EnemyHP = 250;
        }

        if(magicObj != null && specialObj != null && stoneObj != null)  
        {
            animator1A = magicObj.transform.GetChild(0).gameObject.GetComponent<Animator>();  // magicObj?�� step1 ?��?��메이?�� �???��?���??
            animator1B = magicObj.transform.GetChild(1).gameObject.GetComponent<Animator>();  // magicObj?�� step2 ?��?��메이?�� �???��?���??
            animator1C = magicObj.transform.GetChild(2).gameObject.GetComponent<Animator>();  // magicObj?�� step3 ?��?��메이?�� �???��?���??
            animator2A = specialObj.transform.GetChild(0).gameObject.GetComponent<Animator>();  // speicalObj?�� step1 ?��?��메이?�� �???��?���??
            animator2B = specialObj.transform.GetChild(1).gameObject.GetComponent<Animator>();  // speicalObj?�� step2 ?��?��메이?�� �???��?���??
            animator3A = stoneObj.transform.GetChild(0).gameObject.GetComponent<Animator>();  // stoneObj?�� step1 ?��?��메이?�� �???��?���??
            animator3B = stoneObj.transform.GetChild(1).gameObject.GetComponent<Animator>();  // stoneObj?�� step2 ?��?��메이?�� �???��?���??

            animator1A.SetBool("isDone", false);  
            animator1B.SetBool("isDone", false);  
            animator1C.SetBool("isDone", false);  
            animator2A.SetBool("isDone", false);  
            animator2B.SetBool("isDone", false);  
            animator3A.SetBool("isDone", false);  
            animator3B.SetBool("isDone", false);  
        }
    }

    void Awake()  
    {
        BasicSpawnStop(true);  
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(true);
    }   

    void Update()
    {
        if(EnemyHP <= 0 && GameClear == false)
        {
            BasicSpawnStop(true); 
            StoneSpawnStop(true); 
            SpecialOrbSpawnAllStop(true); 
            GameClear = true;
        }
    }


    public IEnumerator Stage1EventFlow()
    {   
        //start window 
        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);
        BasicSpawnStop(true);

        yield return new WaitForSeconds(3);
        
        StoneSpawnStop(false); 
        yield return new WaitForSeconds(swordTime); 
        StoneSpawnStop(true); 

        yield return new WaitForSeconds(3);

        BasicSpawnStop(false); 
        yield return new WaitForSeconds(10); 
        StoneSpawnStop(false); 
        foreach (GameObject spawner in stoneSpawner)
        {
            spawner.GetComponent<Spawner>().beat *=3;
        }
        yield return new WaitForSeconds(BasicSpawnTime); 
        
        
        while (EnemyHP > 0) 
        {
            yield return null; 
        }

        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        yield return new WaitForSeconds(8); 
        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public IEnumerator Stage2EventFlow()
    {   
        //start window 
        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);
        BasicSpawnStop(true);

        yield return new WaitForSeconds(3);

        SpecialOrbSpawnAllStop(false); 
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }

        StoneSpawnStop(false); 
        yield return new WaitForSeconds(swordTime); 
        StoneSpawnStop(true); 

        yield return new WaitForSeconds(3);
        
        BasicSpawnStop(false); 
        yield return new WaitForSeconds(10); 
        StoneSpawnStop(false); 
        foreach (GameObject spawner in stoneSpawner)
        {
            spawner.GetComponent<Spawner>().beat *=3;
        }
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        
        while (EnemyHP > 0) 
        {
            yield return null; 
        }

        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(true); 
        yield return new WaitForSeconds(8);

        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public IEnumerator Stage3EventFlow()
    {   
        //start window 
        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);
        BasicSpawnStop(true);

        yield return new WaitForSeconds(3);

        SpecialOrbSpawnAllStop(false); 
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }

        StoneSpawnStop(false); 
        yield return new WaitForSeconds(swordTime); 
        StoneSpawnStop(true); 

        yield return new WaitForSeconds(3); 
        
        BasicSpawnStop(false); 
        yield return new WaitForSeconds(10); 
        StoneSpawnStop(false); 
        foreach (GameObject spawner in stoneSpawner)
        {
            spawner.GetComponent<Spawner>().beat *=2.5f;
        }
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        
        while (EnemyHP > 0) 
        {
            yield return null; 
        }

        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(true); 
        yield return new WaitForSeconds(8);
        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public IEnumerator TutorialEventFlow()
    {   
        PlayerPrefs.SetInt("knifeActive", 1);
        print("나이프 키2 = "  + PlayerPrefs.GetInt("knifeActive"));

        TrainingDummyGlow glowing = GameObject.Find("training_dummy_mesh").GetComponent<TrainingDummyGlow>();

        //[**** 게임 ?��계�?? explain window ****] 
        explainUI.SetActive(true);  
        yield return new WaitForSeconds(10);  
        explainUI.SetActive(false); 

        //[****마법 ?���?? ?���?? 방법 window***] 
        //step1 UI
        magicObj.transform.GetChild(0).gameObject.SetActive(true);    //Magic step1 UI 
        yield return new WaitForSeconds(3); 
        tutorialEvent.magicRedOrb.SetActive(true);  
        while (!tutorialEvent.magicRedOrbMission) 
        {
            yield return null; 
        }
        glowing.SetGlowing();
        animator1A.SetBool("isDone", true);  //?��?���?? 
        yield return new WaitForSeconds(2); 
        magicObj.transform.GetChild(0).gameObject.SetActive(false);

        //step2 UI
        magicObj.transform.GetChild(1).gameObject.SetActive(true);    //Magic step2 UI
        yield return new WaitForSeconds(3);
        tutorialEvent.magicBlueOrb.SetActive(true);
        while(!tutorialEvent.magicBlueOrbMission)
        {
            yield return null;
        }
        glowing.SetGlowing();
        animator1B.SetBool("isDone", true);  //?��?���??
        yield return new WaitForSeconds(2);
        magicObj.transform.GetChild(1).gameObject.SetActive(false);
        
        //step3 UI
        magicObj.transform.GetChild(2).gameObject.SetActive(true);    //Magic step3 UI
        yield return new WaitForSeconds(5); 
        tutorialEvent.magicFailMission = false; 
        BasicSpawnStop(false);
        while(!tutorialEvent.magicFailMission) 
        {
            yield return null; 
        }
        glowing.SetGlowing();
        yield return new WaitForSeconds(5);
        BasicSpawnStop(true);
        animator1C.SetBool("isDone", true);  
        yield return new WaitForSeconds(2);
        magicObj.transform.GetChild(2).gameObject.SetActive(false);  

        //[****?���?? ?���?? ?���?? 방법 window***]
        //step1 UI
        specialObj.transform.GetChild(0).gameObject.SetActive(true);    //special step1 UI
        yield return new WaitForSeconds(8);
        tutorialEvent.specialOrb.SetActive(true);
        while(!tutorialEvent.specialOrbMission) 
        {
            yield return null; 
        }
        glowing.SetGlowing();
        animator2A.SetBool("isDone", true);  
        yield return new WaitForSeconds(2);
        specialObj.transform.GetChild(0).gameObject.SetActive(false);
        
        //step2 UI
        specialObj.transform.GetChild(1).gameObject.SetActive(true);    //special step2 UI
        yield return new WaitForSeconds(3);
        animator2B.SetBool("isDone", true);  
        yield return new WaitForSeconds(2);
        specialObj.transform.GetChild(1).gameObject.SetActive(false);


        //[****?��?��?�� ?���?? 방법 window***]
        //step1 UI
        stoneObj.transform.GetChild(0).gameObject.SetActive(true);     //stone step1 UI
        yield return new WaitForSeconds(3); 
        tutorialEvent.lavaSwordMission = false;
        tutorialEvent.iceSwordMission = false; 
        while (!(tutorialEvent.lavaSwordMission && tutorialEvent.iceSwordMission))
        {
            yield return null; 
        }
        glowing.SetGlowing();
        animator3A.SetBool("isDone", true); 
        yield return new WaitForSeconds(2);
        stoneObj.transform.GetChild(0).gameObject.SetActive(false);

        stoneObj.transform.GetChild(01).gameObject.SetActive(true);  //stone step2 UI
        yield return new WaitForSeconds(8); 
        tutorialEvent.lavaStone.SetActive(true);
        tutorialEvent.iceStone.SetActive(true);  
        while (!(tutorialEvent.lavaStoneMission && tutorialEvent.iceStoneMission))
        {
            yield return null; 
        }
        glowing.SetGlowing();
        animator3B.SetBool("isDone", true);  
        yield return new WaitForSeconds(2);
        stoneObj.transform.GetChild(01).gameObject.SetActive(false);
        

        //Attack EnemyHP tutorial
        //EnemyHP step1 UI
        magicObj.transform.GetChild(2).gameObject.SetActive(true);    //EnemyHP step1 UI
        yield return new WaitForSeconds(5); 
        tutorialEvent.magicFailMission = false; 
        BasicSpawnStop(false); 
        StoneSpawnStop(false); 
        foreach (GameObject spawner in stoneSpawner)
        {
            spawner.GetComponent<Spawner>().beat *=2.5f;
        }
        while(EnemyHP > 0) 
        {   
            yield return null; 
        }
        glowing.SetGlowing();
        yield return new WaitForSeconds(5);
        BasicSpawnStop(true);
        animator1C.SetBool("isDone", true);  
        yield return new WaitForSeconds(2);
        magicObj.transform.GetChild(2).gameObject.SetActive(false);  


        finishUI.SetActive(true); 
        yield return new WaitForSeconds(12); 
        finishUI.SetActive(false); 
   
        PlayerPrefs.SetInt("knifeActive", 1);
        GameClear = true; 
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public void BasicSpawnStop(bool stop)
    {
        if(basicOrbSpawner.Length != 0)
        {
            foreach (GameObject spawner in basicOrbSpawner)
            {
                spawner.GetComponent<Spawner>().isSpawnStop = stop;
            }
        }
    }

    public void StoneSpawnStop(bool stop)
    {
        if(stoneSpawner.Length != 0)
        {
            foreach (GameObject spawner in stoneSpawner)
            {
                spawner.GetComponent<Spawner>().isSpawnStop = stop;
            }
        }
    }

    public void SpecialOrbSpawnAllStop(bool stop)
    {
        if(SpecialOrbSpawner.Length != 0)
        {
            foreach (GameObject spawner in SpecialOrbSpawner)
            {
                spawner.GetComponent<SpecialOrbSpawner>().isSpawnStop = stop;
            }
        }
    }
}
