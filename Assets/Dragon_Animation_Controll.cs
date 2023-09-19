using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_Animation_Controll : MonoBehaviour
{
    public bool dragonIsAttacked;

    // Start is called before the first frame update
    void Start()
    {
        Animator dragonAnimator = GetComponent<Animator>();
        dragonAnimator.SetBool("okay", true);
        dragonAnimator.SetBool("attacked", false);
    }

    // Update is called once per frame
    void Update()
    {
        Animator dragonAnimator = GetComponent<Animator>();
        if (dragonIsAttacked)
        {
            dragonAnimator.SetBool("attacked", true);
            dragonAnimator.SetBool("okay", false);
        }
    }
}
