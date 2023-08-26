using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    [SerializeField] private Image blackImg;
    [SerializeField] private int fadeControlValue;
    [SerializeField] private int startValue;
    [SerializeField] private Image startImg;
    [SerializeField] private Image title;

    private static bool hasDeletedKey = false;

    // Start is called before the first frame update
    void Start()
    {
        // startImg.color = new Color(0, 0, 0, 1);
        // StartCoroutine(StartSceneFadeCoroutine());

        if(fadeControlValue == 0){
            StartCoroutine(FadeCoroutine());
        }

        if(fadeControlValue == 1){
            blackImg.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeInCoroutine());
        }
        
        if(startValue != 0){
            startImg.color = new Color(0, 0, 0, 1);
            title.color = new Color(0, 0, 0, 0);
            StartCoroutine(StartSceneFadeCoroutine());
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

    IEnumerator StartSceneFadeCoroutine()
    {
        yield return new WaitForSeconds(2);
        float fadeCount = 0; //처음 알파값
        while(fadeCount <= 1f){
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            title.color = new Color(255, 255, 255, fadeCount);
        }

        yield return new WaitForSeconds(2);

        while(fadeCount > 0){
            fadeCount -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            title.color = new Color(255, 255, 255, fadeCount);
        }

        yield return new WaitForSeconds(2);
        float fadeCount2 = 1f; // Start from the faded-out state
        while (fadeCount2 > 0)
        {
            fadeCount2 -= 0.01f;
            yield return new WaitForSeconds(0.05f);
            startImg.color = new Color(0, 0, 0, fadeCount2);
        }
    }
}
