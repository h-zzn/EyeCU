using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GameObject[] basicOrbSpawner;
    [SerializeField] private GameObject[] SpecialOrbSpawner;

    private bool eventStarted = false;
    [SerializeField] private float startDelayTime;
    
    [SerializeField] private float BasicSpawnTime;
    [SerializeField] private float swordTime;

    private float Timer = 0;

    private Coroutine EventFlow = null;

    [SerializeField] private GameObject Eye;
    [SerializeField] private AudioSource BGM;

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

    //ì²«ë²ì§? ?´ë²¤í¸ flow
    private IEnumerator EventFlowCoroutine()
    {   
        //ê¸°ë³¸ ë©ì»¤?ì¦? ?ê°?
        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);

        //?¹ë³? orb ?±?¥ ?ê°?
        BasicSpawnStop(true);
        SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop = false;
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }
        
        //swordë§? ?¸ ? ?ê²? ? ?ê°?
        removeEyeMarker();
        basicOrbSpawner[0].GetComponent<Spawner>().isSpawnStop = false;
        basicOrbSpawner[1].GetComponent<Spawner>().isSpawnStop = false;
        Eye.GetComponent<EyeTrackingRay>().enabled = false;
        yield return new WaitForSeconds(swordTime);
        
        //ê¸°ë³¸ ë©ì»¤?ì¦? ?ê°?
        BasicSpawnStop(false);
        Eye.GetComponent<EyeTrackingRay>().enabled = true;
        yield return new WaitForSeconds(BasicSpawnTime);

        //ê²ì ì¢ë£
        BasicSpawnStop(true);
        SpecialOrbSpawnAllStop();
        if (BGM != null)
        {
            BGM.Stop();
        }
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }


    //ê¸°ë³¸ ?¤?¬? ë©ì¶¤ ?¬ë¶? 
    public void BasicSpawnStop(bool stop)
    {
        foreach (GameObject spawner in basicOrbSpawner)
        {
            spawner.GetComponent<Spawner>().isSpawnStop = stop;
        }
    }

    //?¹? ?¤?¬? ?¤ ë©ì¶¤
    public void SpecialOrbSpawnAllStop()
    {
        foreach (GameObject spawner in SpecialOrbSpawner)
        {
            spawner.GetComponent<SpecialOrbSpawner>().isSpawnStop = true;
        }
    }

    private void removeEyeMarker()
    {
        Destroy(GameObject.FindWithTag("EyeMarker"));
    }

}
