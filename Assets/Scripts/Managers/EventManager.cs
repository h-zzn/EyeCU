using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GameObject[] basicOrbSpawner;
    [SerializeField] private GameObject[] stoneSpawner;
    [SerializeField] private GameObject[] SpecialOrbSpawner;

    private bool eventStarted = false;
    [SerializeField] private float startDelayTime;
    
    [SerializeField] private float BasicSpawnTime;
    [SerializeField] private float swordTime;

    private float Timer = 0;

    private Coroutine EventFlow = null;

    void Awake()
    {
        BasicSpawnStop(true);
        SpecialOrbSpawnAllStop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!eventStarted) 
        {
            // Wait for startDelayTime
            if (Timer >= startDelayTime) 
            {
                eventStarted = true;

                // Start a coroutine to handle the event flow
                if(EventFlow == null)
                    EventFlow = StartCoroutine(EventFlowCoroutine()); 
            }
        } 

        Timer += Time.deltaTime;
    }

    //첫번�?? ?��벤트 flow
    private IEnumerator EventFlowCoroutine()
    {   
        //기본 메커?���?? ?���??
        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);

        //"Don't take your eyes off the starry Orb! Put all my energy into your eyes!" It's time to say that
        yield return new WaitForSeconds(7);

        //?���?? orb ?��?�� ?���??
        BasicSpawnStop(true);
        SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop = false;
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }
        
        //sword�?? ?�� ?�� ?���?? ?�� ?���??
        StoneSpawnStop(false); 
        yield return new WaitForSeconds(swordTime); 
        
        //It's time to let you know that the power of your eyes is back
        StoneSpawnStop(true); 
        yield return new WaitForSeconds(7); 
        

        //기본 메커?���?? ?���??
        BasicSpawnStop(false); 
        yield return new WaitForSeconds(10); 
        StoneSpawnStop(false); 
        stoneSpawner[0].GetComponent<Spawner>().beat *=3; 
        stoneSpawner[1].GetComponent<Spawner>().beat *=3; 
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        
        yield return new WaitForSeconds(5);

        //게임 종료
        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(); 
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }


    //기본 ?��?��?�� 멈춤 ?���?? 
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

    //?��?�� ?��?��?�� ?�� 멈춤
    public void SpecialOrbSpawnAllStop()
    {
        foreach (GameObject spawner in SpecialOrbSpawner)
        {
            spawner.GetComponent<SpecialOrbSpawner>().isSpawnStop = true;
        }
    }
}
