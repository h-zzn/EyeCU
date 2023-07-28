using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : MonoBehaviour
{
    StageGameManager stageGameManager;

    void Start(){
        stageGameManager = FindObjectOfType<StageGameManager>();
    }

    private void OnTriggerEnter(Collider collision){
        if(collision.tag == "Item"){
            stageGameManager.GetScore();
            GameObject.Destroy(collision.gameObject);
        }

        if(collision.tag == "gameOver"){
            stageGameManager.GetScore(); // 제대로 작동되는지 확인 위함
            stageGameManager.GameOver();
        }
    }
}