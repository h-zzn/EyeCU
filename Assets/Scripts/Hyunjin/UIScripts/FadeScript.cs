using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public Image blackImg;
    public int fadeControlValue;
    // Start is called before the first frame update
    void Start()
    {
        if(fadeControlValue == 0){
            StartCoroutine(FadeCoroutine());
        }

        else{
            blackImg.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeInCoroutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }


    IEnumerator FadeCoroutine(){
        float fadeCount = 0; //처음 알파값
        while(fadeCount < 0.7f){
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            blackImg.color = new Color(0, 0, 0, fadeCount);
        }
    }

    IEnumerator FadeInCoroutine()
    {
        float fadeCount = 0.5f; // Start from the faded-out state
        while (fadeCount > 0)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            blackImg.color = new Color(0, 0, 0, fadeCount);
        }

    }
}
