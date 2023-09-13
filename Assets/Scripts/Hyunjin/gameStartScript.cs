using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameStartScript : MonoBehaviour
{    
    public int gameStartValue = 0;

    public GameObject activeObj;
    public GameObject otherObj;

    public Transform cam;

    private MeshCollider knifeMeshCollider;
    private MeshCollider bookMeshCollider;

    [SerializeField] private GameObject canvasObj;

    //canvas Animator
    Animator canvasAnimator;


    float timer = 0.0f;


    void Start(){
        PlayerPrefs.DeleteKey("knifeActive"); 
        canvasAnimator = canvasObj.transform.gameObject.GetComponent<Animator>();
        canvasAnimator.SetBool("isStory", false);  // true로 바꾸기


        knifeMeshCollider = GameObject.Find("knife").GetComponent<MeshCollider>();
        //bookMeshCollider = GameObject.Find("book").GetComponent<MeshCollider>();

        print("나이프 키 = "  + PlayerPrefs.GetInt("knifeActive"));

        if(PlayerPrefs.HasKey("knifeActive")){  // 튜토리얼을 완료했다면
            knifeMeshCollider.enabled = true;
            //bookMeshCollider.enabled = true;
            canvasAnimator.SetBool("isStory", false);
        }
    }

    void Update(){
        // if(!PlayerPrefs.HasKey("knifeActive"))
        //     timer += Time.deltaTime;
        //     if(timer > 5.0f){
        //         bookMeshCollider.enabled = true;
        //         //print("book");
        //     }
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player"))
        {
            activeObj.SetActive(true);

            if(otherObj.activeSelf == true){
                otherObj.SetActive(false);
            }
            
        }
    }
}
