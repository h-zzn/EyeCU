using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stageManager : MonoBehaviour
{
    [SerializeField] private AudioSource BGM; 

    private EventManager eventManager; 

    private DamagedArea damagedArea;

    private Coroutine stageOver = null;
    private Coroutine stageClear = null;

    void Awake()
    {
        eventManager = this.transform.GetComponent<EventManager>(); 
        damagedArea = this.transform.GetComponent<DamagedArea>();
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
    }

    public IEnumerator StageOver() 
    {
        yield return new WaitForSeconds(5); 
        BGMOff();
        ActiveGameOverWindow();
    }  

    public IEnumerator StageClear() 
    {
        yield return new WaitForSeconds(5); 
        BGMOff();
        ActiveGameClearWindow();
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
}
