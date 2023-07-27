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

    //Ï≤´Î≤àÏß? ?ù¥Î≤§Ìä∏ flow
    private IEnumerator EventFlowCoroutine()
    {   
        //Í∏∞Î≥∏ Î©îÏª§?ãàÏ¶? ?ãúÍ∞?
        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);

        //?äπÎ≥? orb ?ì±?û• ?ãúÍ∞?
        BasicSpawnStop(true);
        SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop = false;
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }
        
        //swordÎß? ?ì∏ ?àò ?ûàÍ≤? ?êú ?ãúÍ∞?
        removeEyeMarker();
        basicOrbSpawner[0].GetComponent<Spawner>().isSpawnStop = false;
        basicOrbSpawner[1].GetComponent<Spawner>().isSpawnStop = false;
        Eye.GetComponent<EyeTrackingRay>().enabled = false;
        yield return new WaitForSeconds(swordTime);
        
        //Í∏∞Î≥∏ Î©îÏª§?ãàÏ¶? ?ãúÍ∞?
        BasicSpawnStop(false);
        Eye.GetComponent<EyeTrackingRay>().enabled = true;
        yield return new WaitForSeconds(BasicSpawnTime);

        //Í≤åÏûÑ Ï¢ÖÎ£å
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


    //Í∏∞Î≥∏ ?ä§?è¨?Ñà Î©àÏ∂§ ?ó¨Î∂? 
    public void BasicSpawnStop(bool stop)
    {
        foreach (GameObject spawner in basicOrbSpawner)
        {
            spawner.GetComponent<Spawner>().isSpawnStop = stop;
        }
    }

    //?äπ?àò ?ä§?è¨?Ñà ?ã§ Î©àÏ∂§
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
