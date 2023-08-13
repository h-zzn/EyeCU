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

    // tutorial UI Í¥??†®
    [SerializeField] private GameObject magicObj;  
    [SerializeField] private GameObject specialObj;  
    [SerializeField] private GameObject stoneObj; 

    // Animator Í¥??†® 
    Animator animator1A;
    Animator animator1B;
    Animator animator1C;
    Animator animator2A;
    Animator animator2B;
    Animator animator3A;
    Animator animator3B;

    private float Timer = 0;

    public Coroutine EventFlow = null;

    void Start(){
        // ?ï†?ãàÎ©îÏù¥?Ñ∞ Í¥??†® ?Ç¥?ö©?ì§?ù∏?ç∞,, ?†úÍ∞? Ï°∞Í∏à ?çî ?ö®?ú®?†Å?úºÎ°? ?†ïÎ¶¨Ìïò?äî Î∞©Î≤ï?ùÑ Ï∞æÏïÑÎ≥¥Í≤†?ùå?ã§ ?Öé
        animator1A = magicObj.transform.GetChild(0).gameObject.GetComponent<Animator>();  // magicObj?ùò step1 ?ï†?ãàÎ©îÏù¥?Ñ∞ Í∞??†∏?ò§Í∏?
        animator1B = magicObj.transform.GetChild(1).gameObject.GetComponent<Animator>();  // magicObj?ùò step2 ?ï†?ãàÎ©îÏù¥?Ñ∞ Í∞??†∏?ò§Í∏?
        animator1C = magicObj.transform.GetChild(2).gameObject.GetComponent<Animator>();  // magicObj?ùò step3 ?ï†?ãàÎ©îÏù¥?Ñ∞ Í∞??†∏?ò§Í∏?
        animator2A = specialObj.transform.GetChild(0).gameObject.GetComponent<Animator>();  // speicalObj?ùò step1 ?ï†?ãàÎ©îÏù¥?Ñ∞ Í∞??†∏?ò§Í∏?
        animator2B = specialObj.transform.GetChild(1).gameObject.GetComponent<Animator>();  // speicalObj?ùò step2 ?ï†?ãàÎ©îÏù¥?Ñ∞ Í∞??†∏?ò§Í∏?
        animator3A = stoneObj.transform.GetChild(0).gameObject.GetComponent<Animator>();  // stoneObj?ùò step1 ?ï†?ãàÎ©îÏù¥?Ñ∞ Í∞??†∏?ò§Í∏?
        animator3B = stoneObj.transform.GetChild(1).gameObject.GetComponent<Animator>();  // stoneObj?ùò step2 ?ï†?ãàÎ©îÏù¥?Ñ∞ Í∞??†∏?ò§Í∏?

        animator1A.SetBool("isDone", false);
        animator1B.SetBool("isDone", false);
        animator1C.SetBool("isDone", false);
        animator2A.SetBool("isDone", false);
        animator2B.SetBool("isDone", false);
        animator3A.SetBool("isDone", false);
        animator3B.SetBool("isDone", false);
    }

    void Awake()  
    {
        BasicSpawnStop(true);  
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(true);
    }


    public IEnumerator EventFlowCoroutine()
    {   
       
        //start window 

        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);

        //"Don't take your eyes off the starry Orb! Put all my energy into your eyes!" It's time to say that
        yield return new WaitForSeconds(7);
        
        BasicSpawnStop(true);
        SpecialOrbSpawnAllStop(false); 
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
        foreach (GameObject spawner in stoneSpawner)
        {
            spawner.GetComponent<Spawner>().beat *=3;
        }
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        

        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(true); 
        yield return new WaitForSeconds(5);
        
        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public IEnumerator TutorialEventFlow()
    {   
        //[**** Í≤åÏûÑ ?Ñ∏Í≥ÑÍ?? explain window ****]
        explainUI.SetActive(true);
        yield return new WaitForSeconds(10);
        explainUI.SetActive(false); 

        //[****ÎßàÎ≤ï ?ò§Î∏? ?†úÍ±? Î∞©Î≤ï window***]
        //step1 UI
        magicObj.transform.GetChild(0).gameObject.SetActive(true);    // step1 UI
        // if(step1 øœ∑·«ﬂ¿ª ∂ß ¡∂∞«){
        //     animator1A.SetBool("isDone", true);  // ?Ç¨?ùºÏß?
        //     yield return new WaitForSeconds(2);
        //     Destroy(magicObj.transform.GetChild(0).gameObject);
        // }

        //step2 UI
        magicObj.transform.GetChild(1).gameObject.SetActive(true);    // step2 UI
        // if(step2 Ïß??ãú ?ôÑÎ£åÌïòÎ©?){
        //     animator1B.SetBool("isDone", true);  // ?Ç¨?ùºÏß?
        //     yield return new WaitForSeconds(2);
        //     Destroy(magicObj.transform.GetChild(1).gameObject);
        // }

        //step3 UI
        magicObj.transform.GetChild(2).gameObject.SetActive(true);    // step2 UI
        // if(step3 Ïß??ãú ?ôÑÎ£åÌïòÎ©?){
        //     animator1C.SetBool("isDone", true);  // ?Ç¨?ùºÏß?
        //     yield return new WaitForSeconds(2);
        //     Destroy(magicObj.transform.GetChild(2).gameObject);
        // }

        yield return new WaitForSeconds(20);
 
        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);

        //"Don't take your eyes off the starry Orb! Put all my energy into your eyes!" It's time to say that
        yield return new WaitForSeconds(7);
        

        //[****?äπÎ≥? ?ò§Î∏? ?†úÍ±? Î∞©Î≤ï window***]
        //step1 UI
        specialObj.transform.GetChild(0).gameObject.SetActive(true);    // step1 UI
        // if(step1 Ïß??ãú ?ôÑÎ£åÌïòÎ©?){
        //     animator2A.SetBool("isDone", true);  // ?Ç¨?ùºÏß?
        //     yield return new WaitForSeconds(2);
        //     Destroy(specialObj.transform.GetChild(0).gameObject);
        // }

        //step2 UI
        specialObj.transform.GetChild(1).gameObject.SetActive(true);    // step1 UI
        // if(step2 Ïß??ãú ?ôÑÎ£åÌïòÎ©?){
        //     animator2B.SetBool("isDone", true);  // ?Ç¨?ùºÏß?
        //     yield return new WaitForSeconds(2);
        //     Destroy(specialObj.transform.GetChild(1).gameObject);
        // }
        yield return new WaitForSeconds(20);

        BasicSpawnStop(true);
        SpecialOrbSpawnAllStop(false); 
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }

        
        //[****?èå?ç©?ù¥ ?†úÍ±? Î∞©Î≤ï window***]
        //step1 UI
        stoneObj.transform.GetChild(0).gameObject.SetActive(true);    // step1 UI
        // if(step1 Ïß??ãú ?ôÑÎ£åÌïòÎ©?){
        //     animator3A.SetBool("isDone", true);  // ?Ç¨?ùºÏß?
        //     yield return new WaitForSeconds(2);
        //     Destroy(stoneObj.transform.GetChild(0).gameObject);
        // }

        //step2 UI
        stoneObj.transform.GetChild(01).gameObject.SetActive(true);    // step1 UI
        // if(step2 Ïß??ãú ?ôÑÎ£åÌïòÎ©?){
        //     animator3B.SetBool("isDone", true);  // ?Ç¨?ùºÏß?
        //     yield return new WaitForSeconds(2);
        //     Destroy(stoneObj.transform.GetChild(1).gameObject);
        // }

        yield return new WaitForSeconds(20);

        StoneSpawnStop(false); 
        yield return new WaitForSeconds(swordTime); 
        
        //It's time to let you know that the power of your eyes is back
        StoneSpawnStop(true); 
        yield return new WaitForSeconds(7); 
        
        BasicSpawnStop(false); 
        yield return new WaitForSeconds(10); 

        StoneSpawnStop(false); 
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        

        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(true); 
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

    public void SpecialOrbSpawnAllStop(bool stop)
    {
        foreach (GameObject spawner in SpecialOrbSpawner)
        {
            spawner.GetComponent<SpecialOrbSpawner>().isSpawnStop = stop;
        }
    }
}
