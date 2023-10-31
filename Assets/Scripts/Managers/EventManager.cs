using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using static GameManager;

public class EventManager : MonoBehaviour
{
    public int EnemyHP = 1000;

    private SpawnManager spawnManager;

    public bool GameClear = false;
    
    [SerializeField] private float EventStartDelayTime; 
    [SerializeField] private float BasicSpawnTime; 
    [SerializeField] private float swordTime; 

    [SerializeField] private GameObject OVRInteractionObj;  // 손 끄는거  

    [SerializeField] private GameObject explainUI; 
    // tutorial UI 
    [SerializeField] private GameObject magicObj;   
    [SerializeField] private GameObject specialObj;   
    [SerializeField] private GameObject stoneObj; 
    [SerializeField] private GameObject HPUI; 
    [SerializeField] private GameObject SkillUI; 
    [SerializeField] private GameObject finishUI;  
    [SerializeField] private GameObject gaugeObj;

    [SerializeField] private GameObject MonsterHPGauge;
    [SerializeField] private GameObject MonsterMaterialObj;
    private List<Material> MonsterHPMaterials;

    // Animator 
    Animator animator1A; 
    Animator animator1B; 
    Animator animator1C; 
    Animator animator2A; 
    Animator animator2B; 
    Animator animator3A; 
    Animator animator3B; 

    // gauge Animator
    Animator animator4A;
    Animator animator4B;
    Animator animator4C;
    Animator animator5A;
    Animator animator5B;
    Animator animator5C;

    private float Timer = 0; 

    public Coroutine EventFlow = null; 

    private TutorialEvent tutorialEvent = null;  

    private int TutorialBuildIndex;

    private DeleteEnemyAttack deleteEnemyAttack;
    private ControllerManager controllerManager;

    void Start()  
    {
        if(this.GetComponent<GameManager>().stageLevel == StageLevel.tutorial)  
        {
           tutorialEvent = GameObject.Find("TutorialObjects").GetComponent<TutorialEvent>();
           controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();
           deleteEnemyAttack = GameObject.Find("Eraser").GetComponent<DeleteEnemyAttack>();
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

            animator4A = HPUI.transform.GetChild(0).gameObject.GetComponent<Animator>();  // HPUI step1 
            animator4B = HPUI.transform.GetChild(1).gameObject.GetComponent<Animator>();  // HPUI step2 
            animator4C = HPUI.transform.GetChild(2).gameObject.GetComponent<Animator>();  // HPUI step3 

            animator5A = SkillUI.transform.GetChild(0).gameObject.GetComponent<Animator>();  // Skill step1 
            animator5B = SkillUI.transform.GetChild(1).gameObject.GetComponent<Animator>();  // Skill step2 
            animator5C = SkillUI.transform.GetChild(2).gameObject.GetComponent<Animator>();  // Skill step3 


            animator1A.SetBool("isDone", false);  
            animator1B.SetBool("isDone", false);  
            animator1C.SetBool("isDone", false);  
            animator2A.SetBool("isDone", false);  
            animator2B.SetBool("isDone", false);  
            animator3A.SetBool("isDone", false);  
            animator3B.SetBool("isDone", false);  
            animator4A.SetBool("isDone", false);  
            animator4B.SetBool("isDone", false);  
            animator4C.SetBool("isDone", false); 
            animator5A.SetBool("isDone", false);  
            animator5B.SetBool("isDone", false);  
            animator5C.SetBool("isDone", false);  
        }
    }

    void Awake()  
    {
        spawnManager = this.GetComponent<SpawnManager>();
        spawnManager.activeBasicOrb = false;  
        spawnManager.activeStone = false; 
        spawnManager.activeSpecialOrb = false;

        MonsterHPMaterials = new List<Material>(MonsterMaterialObj.GetComponent<Renderer>().materials);
    }   

    void Update()
    {
        if(EnemyHP <= 0 && GameClear == false)
        {
            spawnManager.activeBasicOrb = false; 
            spawnManager.activeStone = false; 
            spawnManager.activeSpecialOrb = false; 
            GameClear = true;
        }

        reduceEnemyHPGauge();
    }

    private void reduceEnemyHPGauge()
    {
        if(MonsterHPGauge.transform.parent.gameObject.activeSelf)
        {
            if (EnemyHP <= 0)
            {
                MonsterHPGauge.GetComponent<Renderer>().material = MonsterHPMaterials[4];
            }
            else if (EnemyHP <= 250)
            {
                MonsterHPGauge.GetComponent<Renderer>().material = MonsterHPMaterials[3];
            }
            else if (EnemyHP <= 500)
            {
                MonsterHPGauge.GetComponent<Renderer>().material = MonsterHPMaterials[2];
            }
            else if (EnemyHP <= 750)
            {
                MonsterHPGauge.GetComponent<Renderer>().material = MonsterHPMaterials[1];
            }
            else if (EnemyHP <= 1000)
            {
                MonsterHPGauge.GetComponent<Renderer>().material = MonsterHPMaterials[0];
            }
        }
    }

    public IEnumerator Stage1EventFlow()
    {
        yield return new WaitForSeconds(2);
        //start window 
        spawnManager.activeBasicOrb = true;
        yield return new WaitForSeconds(BasicSpawnTime);  
        spawnManager.activeBasicOrb = false;

        yield return new WaitForSeconds(3);
        
        spawnManager.activeStone = true;
        yield return new WaitForSeconds(swordTime);  
        spawnManager.activeStone = false;

        yield return new WaitForSeconds(3);

        spawnManager.activeBasicOrb = true;
        yield return new WaitForSeconds(10); 
        spawnManager.activeStone = true;
        spawnManager.stoneSpawnInterval *= 3f;
        yield return new WaitForSeconds(BasicSpawnTime);   
        
        while (EnemyHP > 0)  
        {
            yield return null; 
        } 

        spawnManager.activeBasicOrb = false;
        spawnManager.activeStone = false;
        spawnManager.activeSpecialOrb = false; 
        yield return new WaitForSeconds(8); 
        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public IEnumerator Stage2EventFlow()
    {
        yield return new WaitForSeconds(2);
        //start window 
        spawnManager.activeBasicOrb = true; 
        yield return new WaitForSeconds(BasicSpawnTime); 
        spawnManager.activeBasicOrb = false;  

        yield return new WaitForSeconds(3); 

        spawnManager.activeSpecialOrb = true;  
        yield return new WaitForSeconds(1);
        while (!spawnManager.SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop) 
        {
            yield return null; 
        }
        spawnManager.activeSpecialOrb = false;

        spawnManager.activeStone = true; 
        yield return new WaitForSeconds(swordTime); 
        spawnManager.activeStone = false; 

        yield return new WaitForSeconds(3); 
        
        spawnManager.activeBasicOrb = true; 
        yield return new WaitForSeconds(10); 
        spawnManager.activeStone = true;
        spawnManager.stoneSpawnInterval *= 2.5f;
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        
        while (EnemyHP > 0) 
        {
            yield return null; 
        }

        spawnManager.activeBasicOrb = false; 
        spawnManager.activeStone = false; 
        spawnManager.activeSpecialOrb = false; 
        yield return new WaitForSeconds(8);

        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public IEnumerator Stage3EventFlow()
    {
        yield return new WaitForSeconds(2);
        //start window 
        spawnManager.activeBasicOrb = true;
        yield return new WaitForSeconds(BasicSpawnTime);
        spawnManager.activeBasicOrb = false;

        yield return new WaitForSeconds(3);

        spawnManager.activeSpecialOrb = true;  
        yield return new WaitForSeconds(1);
        while (!spawnManager.SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)  
        { 
            yield return null;  
        } 
        spawnManager.activeSpecialOrb = false;

        spawnManager.activeStone = true; 
        yield return new WaitForSeconds(swordTime); 
        spawnManager.activeStone = false; 

        yield return new WaitForSeconds(3); 
        
        spawnManager.activeBasicOrb = true; 
        yield return new WaitForSeconds(10); 
        spawnManager.activeStone = true;
        spawnManager.stoneSpeed *= 0.6f;
        spawnManager.stoneSpawnInterval *= 2f;
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        
        while (EnemyHP > 0) 
        {
            yield return null; 
        }

        spawnManager.activeBasicOrb = false; 
        spawnManager.activeStone = false; 
        spawnManager.activeSpecialOrb = false; 
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
        tutorialEvent.lavaSwordMission = false;
        tutorialEvent.iceSwordMission = false;
        stoneObj.transform.GetChild(0).gameObject.SetActive(true);     //stone step1 UI
        yield return new WaitForSeconds(3); 
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
        HPUI.SetActive(true); 
        yield return new WaitForSeconds(3);  
        stoneObj.transform.GetChild(01).gameObject.SetActive(false);

        
        //[*** HP 설명 window ***]
        controllerManager.skillEnergyPoint = 0;
        gaugeObj.SetActive(true);
        //HPUI.transform.GetChild(0).gameObject.SetActive(true);     // HP step1 UI
        yield return new WaitForSeconds(5); 
        animator4A.SetBool("isDone", true); 
        yield return new WaitForSeconds(2);
        HPUI.transform.GetChild(0).gameObject.SetActive(false); 
          
        HPUI.transform.GetChild(1).gameObject.SetActive(true);     // HP step2 UI
        OVRInteractionObj.SetActive(false);     // 손 못 쓰게 
        controllerManager.redMagicActive = false;
        controllerManager.blueMagicActive = false;
        spawnManager.activeBasicOrb = true;
        spawnManager.basicOrbSpeed *= 1.5f; 
        while (!tutorialEvent.HPMission)
        {
            yield return null;
        }
        glowing.SetGlowing();
        spawnManager.basicOrbSpeed /= 1.5f; 
        spawnManager.activeBasicOrb = false;
        deleteEnemyAttack.StartCoroutine("DeleteAll"); 
        
        yield return new WaitForSeconds(3); 
        animator4B.SetBool("isDone", true);
        yield return new WaitForSeconds(2);
        HPUI.transform.GetChild(1).gameObject.SetActive(false);
        OVRInteractionObj.SetActive(true);  // 손 다시 쓸 수 있게
        controllerManager.redMagicActive = true;
        controllerManager.blueMagicActive = true; 

        HPUI.transform.GetChild(2).gameObject.SetActive(true);  // MP step1 UI
        yield return new WaitForSeconds(3);
        controllerManager.skillEnergyPoint = 0;
        // MP 조금 차면 클리어
        spawnManager.activeBasicOrb = true;
        while (!tutorialEvent.MPMission)
        {
            yield return null;
        }
        glowing.SetGlowing();
        animator4C.SetBool("isDone", true); 
        yield return new WaitForSeconds(2);
        spawnManager.activeBasicOrb = false;
        deleteEnemyAttack.StartCoroutine("DeleteAll");
        HPUI.transform.GetChild(2).gameObject.SetActive(false);   
        HPUI.SetActive(false);

        //[잘못 눌렀을 경우 게이지 감소 및 패널티 설명]
        magicObj.transform.GetChild(2).gameObject.SetActive(true);    // Magic step3 UI
        yield return new WaitForSeconds(5);
        
        spawnManager.activeBasicOrb = true; 
        while (!tutorialEvent.magicFailMission)
        {
            yield return null; 
        }
        glowing.SetGlowing();
        yield return new WaitForSeconds(5);
        spawnManager.activeBasicOrb = false;
        deleteEnemyAttack.StartCoroutine("DeleteAll"); 
        animator1C.SetBool("isDone", true);
        yield return new WaitForSeconds(2);
        magicObj.transform.GetChild(2).gameObject.SetActive(false);

        //[*** Skill 설명 window ***]
        SkillUI.SetActive(true);
        SkillUI.transform.GetChild(0).gameObject.SetActive(true);      // Skill step1 UI   
        // 부수고 스킬 차는 애니메이션
        controllerManager.skillEnergyPoint = 1950; 
        yield return new WaitForSeconds(3); 
        animator5A.SetBool("isDone", true); 
        yield return new WaitForSeconds(2);
        SkillUI.transform.GetChild(0).gameObject.SetActive(false);   

        yield return new WaitForSeconds(2); 
        controllerManager.skillEnergyPoint = 2000; 
        SkillUI.transform.GetChild(1).gameObject.SetActive(true);      // Skill step2 UI   
        while (!tutorialEvent.skillActivateMission)  
        {
            yield return null; 
        }
        yield return new WaitForSeconds(3); 
        animator5B.SetBool("isDone", true);
        yield return new WaitForSeconds(2); 
        SkillUI.transform.GetChild(1).gameObject.SetActive(false);  
        yield return new WaitForSeconds(3);

        SkillUI.transform.GetChild(2).gameObject.SetActive(true);      // Skill step3 UI   
        while (!tutorialEvent.skillAttackMission)
        {
            controllerManager.skillEnergyPoint = 1000; 
            yield return null;
        }
        yield return new WaitForSeconds(3);
        animator5C.SetBool("isDone", true);
        yield return new WaitForSeconds(2);
        SkillUI.transform.GetChild(2).gameObject.SetActive(false);
        controllerManager.skillEnergyPoint = 0;
        yield return new WaitForSeconds(3);
        glowing.SetGlowing();

        // finish UI 
        finishUI.SetActive(true); 
        yield return new WaitForSeconds(12); 
        finishUI.SetActive(false); 
   
        PlayerPrefs.SetInt("knifeActive", 1);
        GameClear = true; 
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    } 

    public IEnumerator StageDDAEventFlow() 
    {   
        //start window 
        spawnManager.activeBasicOrb = true;
        yield return new WaitForSeconds(50);
        spawnManager.activeBasicOrb = false;

        spawnManager.activeSpecialOrb = true;
        yield return new WaitForSeconds(1);
        while (!spawnManager.SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }
        spawnManager.activeSpecialOrb = false;

        spawnManager.activeBasicOrb = true;

        while (EnemyHP > 0) 
        {
            yield return null; 
        }

        spawnManager.activeBasicOrb = false;

        yield return new WaitForSeconds(8);
    
        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }
}
