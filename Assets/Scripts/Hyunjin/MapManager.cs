using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // 저장 시스템 관련 변수
    private int mapActiveValue;
    private static bool isMapActive = false;

    public GameObject knife;
    public GameObject stageObjectGroup;
    private gameStartScript gameStartScriptInstance;

    // fadein 관련 변수 
    public float fadeInDuration = 1.5f; // 나타나는 데 걸리는 시간
    private Material objectMaterial;
    private Color initialColor;
    private Color targetColor = Color.white; // 최종적으로 알파값이 1이 되도록 설정


    void Start(){
        gameStartScriptInstance = knife.GetComponent<gameStartScript>();

        //PlayerPrefs.DeleteKey("mapActived"); 
        if(!isMapActive){
            PlayerPrefs.DeleteKey("mapActived"); 
            isMapActive = true;
        }
        
        //print("mapActiveValue" + mapActiveValue);

        objectMaterial = GetComponent<Renderer>().material;
        initialColor = objectMaterial.color;

        mapActiveValue = PlayerPrefs.GetInt("mapActived");

        // if(mapActiveValue == 0){
        //     StartCoroutine(FadeInCoroutine());
        // }

        // else{
        //     Debug.Log("!!");    
        // }
    }

    
    void Update(){
        if(!PlayerPrefs.HasKey("mapActived")){
        //StartCoroutine(FadeInCoroutine());
            //Debug.Log("!!~" + gameStartScriptInstance.gameStartValue);
            if(gameStartScriptInstance.gameStartValue == 1){
                Debug.Log("!~" + gameStartScriptInstance.gameStartValue);
                StartCoroutine(FadeInCoroutine());
                stageObjectGroup.SetActive(true);
            } 
        }
        else{
            stageObjectGroup.SetActive(true);
            Color finalColor = objectMaterial.color;
            finalColor.a = 1f;
            objectMaterial.color = finalColor;
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        isMapActive = true;
        mapActiveValue = 1;
        PlayerPrefs.SetInt("mapActived", mapActiveValue);

        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float t = elapsedTime / fadeInDuration;
            Color newColor = Color.Lerp(initialColor, targetColor, t);
            newColor.a = Mathf.Lerp(0f, 1f, t); // 알파값을 서서히 증가시킴
            objectMaterial.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectMaterial.color = targetColor; // 알파값을 확실히 1로 설정 (경우에 따라 부동소수점 오류가 있을 수 있음)
    }
}
