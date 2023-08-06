using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyDragonAnim : MonoBehaviour
{
    Animator animator;

    public static bool isRun = false;
    public static bool isFly = false;
    public static bool isDestroy = false;
    
    public GameObject target1;
    public GameObject target2;

    private bool curState = false;

    void Start(){
        animator = GetComponent<Animator>();
    }

    void Update(){
        animator.SetBool("isRun", isRun);
        animator.SetBool("isFly", isFly);
        animator.SetBool("isDestroy", isDestroy);


        MoveToTarget();

        if(curState == true){
            isRun = false;
            FlyToTarget();
        }

        if(this.gameObject.transform.position == target2.transform.position){
            isDestroy = true;
            DelayAnim();
            Destroy(this.gameObject);
            Debug.Log("Destroyed");
        }
    }

    void MoveToTarget(){
        transform.position = Vector3.MoveTowards(transform.position, target1.transform.position, 1.3f);
        isRun = true;

        if(this.gameObject.transform.position == target1.transform.position){
            curState = true;
            //StartCoroutine(ActivateAttackAfterDelay());
        }
    }

    void FlyToTarget(){
        transform.position = Vector3.MoveTowards(transform.position, target2.transform.position, 2f);
        isFly = true;
    }

    IEnumerator DelayAnim()
    {
        yield return new WaitForSeconds(0.5f);
        
    }
}
