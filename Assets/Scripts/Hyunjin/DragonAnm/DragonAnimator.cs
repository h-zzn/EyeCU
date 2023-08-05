using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimator : MonoBehaviour
{
    Animator animator;

    public static bool isWalk = false;
    public static bool isAttack = false;
    
    public GameObject target;

    void Start(){
        animator = GetComponent<Animator>();
    }

    void Update(){
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isAttack", isAttack);

        MoveToTarget();

        if(this.gameObject.transform.position == target.transform.position){
            isWalk = false;
            isAttack = true;
            //StartCoroutine(ActivateAttackAfterDelay());
        }  
    }

    void MoveToTarget(){
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 0.5f);
        isWalk = true;
    }

    IEnumerator ActivateAttackAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        
    }
}
