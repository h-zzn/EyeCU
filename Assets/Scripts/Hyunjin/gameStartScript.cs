using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameStartScript : MonoBehaviour
{    
    public int gameStartValue = 0;

    public GameObject activeObj;
    public GameObject otherObj;

    public GameObject MAP_UI;

    public Transform cam;

    private MeshCollider knifeMeshCollider;
    private MeshCollider bookMeshCollider;

    float timer = 0.0f;


    void Start(){
        knifeMeshCollider = GameObject.Find("knife").GetComponent<MeshCollider>();
        bookMeshCollider = GameObject.Find("book").GetComponent<MeshCollider>();

        print("나이프 키 = "  + PlayerPrefs.GetInt("knifeActive"));

        if(PlayerPrefs.HasKey("knifeActive")){
            knifeMeshCollider.enabled = true;
            bookMeshCollider.enabled = true;
        }
    }

    void Update(){
        if(!PlayerPrefs.HasKey("knifeActive"))
            timer += Time.deltaTime;
            if(timer > 10.0f){
                bookMeshCollider.enabled = true;
                //print("book");
            }
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player"))
        {
            activeObj.SetActive(true);
            MAP_UI.SetActive(false);
            if(otherObj.activeSelf == true){
                otherObj.SetActive(false);
            }
            
        }
    }
}
