using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAttackDragonAnim : MonoBehaviour
{
    Animator animator;

    public static bool isFly = false;
    public static bool isAttack = false;
    
    public GameObject target;

    void Start(){
        animator = GetComponent<Animator>();
    }

    void Update(){
        animator.SetBool("isFly", isFly);
        animator.SetBool("isAttack", isAttack);

        FlyToTarget();

        if(this.gameObject.transform.position == target.transform.position){
            isFly = false;
            isAttack = true;
        }
    }

    void FlyToTarget(){
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 1.3f);
        isFly = true;
    }
}
