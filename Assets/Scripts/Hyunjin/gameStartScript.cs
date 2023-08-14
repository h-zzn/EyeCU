using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameStartScript : MonoBehaviour
{    
    public float shakeTime = 1.0f;
    public float shakeSpeed = 2.0f;
    public float shakeAmount = 1.0f;

    public int gameStartValue = 0;

    public GameObject activeObj;
    public GameObject otherObj;

    public Transform cam;

    private MeshCollider knifeMeshCollider;


    void Start(){
        knifeMeshCollider = GameObject.Find("knife").GetComponent<MeshCollider>();

        if(PlayerPrefs.HasKey("knifeActive")){
            knifeMeshCollider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Shake());

            activeObj.SetActive(true);

            if(otherObj.activeSelf == true){
                otherObj.SetActive(false);
            }
            
        }
    }

    IEnumerator Shake(){
        gameStartValue = 1;
        Debug.Log("Shake~");
        Vector3 originPosition = cam.localPosition;
        float elapsedTime = 0.0f;

        while(elapsedTime < shakeTime){
            Vector3 randomPoint = originPosition + Random.insideUnitSphere * shakeAmount;
            cam.localPosition = Vector3.Lerp(cam.localPosition, randomPoint, Time.deltaTime * shakeSpeed);

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        cam.localPosition = originPosition;
    }
}
