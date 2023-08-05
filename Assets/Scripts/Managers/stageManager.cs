using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stageManager : MonoBehaviour
{
    [SerializeField] private AudioSource BGM;
    
    public int stageHP = 1000;

    private float Timer = 0;

    void Update() 
    {
        if(stageHP <= 0)
        {
            StartCoroutine(StageOver());    
        }

        Timer += Time.deltaTime; 
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
