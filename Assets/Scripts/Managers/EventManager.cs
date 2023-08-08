using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GameObject[] basicOrbSpawner;
    [SerializeField] private GameObject[] stoneSpawner;
    [SerializeField] private GameObject[] SpecialOrbSpawner;

    public bool GameClear = false;
    
    [SerializeField] private float EventStartDelayTime;
    [SerializeField] private float BasicSpawnTime;
    [SerializeField] private float swordTime;

    [SerializeField] private GameObject explainUI; 
    [SerializeField] private GameObject successUI;  
    [SerializeField] private GameObject failUI; 

    // tutorial UI 
    [SerializeField] private GameObject magicObj;  
    [SerializeField] private GameObject specialObj;  
    [SerializeField] private GameObject stoneObj; 

    Animator animator;
    public static bool isDone = false; 


    private float Timer = 0;

    public Coroutine EventFlow = null;

    void Awake()  
    {
        BasicSpawnStop(true);  
        SpecialOrbSpawnAllStop();
        animator = magicObj.transform.GetChild(0).gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isDone", isDone);

        
    }

    public IEnumerator EventFlowCoroutine()
    {   
        //start window 

        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);

        //"Don't take your eyes off the starry Orb! Put all my energy into your eyes!" It's time to say that
        yield return new WaitForSeconds(7);
        
        BasicSpawnStop(true);
        SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop = false;
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }

        StoneSpawnStop(false); 
        yield return new WaitForSeconds(swordTime); 
        
        //It's time to let you know that the power of your eyes is back
        StoneSpawnStop(true); 
        yield return new WaitForSeconds(7); 
        
        BasicSpawnStop(false); 
        yield return new WaitForSeconds(10); 

        StoneSpawnStop(false); 
        stoneSpawner[0].GetComponent<Spawner>().beat *=3; 
        stoneSpawner[1].GetComponent<Spawner>().beat *=3; 
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        

        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(); 
        yield return new WaitForSeconds(5);
        
        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public IEnumerator TutorialEventFlow()
    {   
        //Game Explain window 
        explainUI.SetActive(true);
        yield return new WaitForSeconds(10);
        explainUI.SetActive(false); 

        //마법 오브 제거 방법 window 
            //step1 UI
        magicObj.transform.GetChild(0).gameObject.SetActive(true);    // step1 UI
        // step1 완료하면 (if)
            // magicObj.transform.GetChild(0).gameObject.SetActive(true);    // step1 UI
            // isDone = true;  // 사라짐
        yield return new WaitForSeconds(2);
        Destroy(magicObj.transform.GetChild(0).gameObject);
        //  isDone = true;

        //yield return new WaitForSeconds(5); 
        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);

        //"Don't take your eyes off the starry Orb! Put all my energy into your eyes!" It's time to say that
        yield return new WaitForSeconds(7);
        
        //특별 오브 제거 방법 window 
        //yield return new WaitForSeconds(5); 
        BasicSpawnStop(true);
        SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop = false;
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }

        //돌덩이 제거 방법 window 
        //yield return new WaitForSeconds(5); 
        StoneSpawnStop(false); 
        yield return new WaitForSeconds(swordTime); 
        
        //It's time to let you know that the power of your eyes is back
        StoneSpawnStop(true); 
        yield return new WaitForSeconds(7); 
        
        BasicSpawnStop(false); 
        yield return new WaitForSeconds(10); 

        StoneSpawnStop(false); 
        stoneSpawner[0].GetComponent<Spawner>().beat *=3; 
        stoneSpawner[1].GetComponent<Spawner>().beat *=3; 
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        

        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(); 
        yield return new WaitForSeconds(5);
        
        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public void BasicSpawnStop(bool stop)
    {
        foreach (GameObject spawner in basicOrbSpawner)
        {
            spawner.GetComponent<Spawner>().isSpawnStop = stop;
        }
    }

    public void StoneSpawnStop(bool stop)
    {
        foreach (GameObject spawner in stoneSpawner)
        {
            spawner.GetComponent<Spawner>().isSpawnStop = stop;
        }
    }

    public void SpecialOrbSpawnAllStop()
    {
        foreach (GameObject spawner in SpecialOrbSpawner)
        {
            spawner.GetComponent<SpecialOrbSpawner>().isSpawnStop = true;
        }
    }
}
