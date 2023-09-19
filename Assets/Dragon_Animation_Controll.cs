using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_Animation_Controll : MonoBehaviour
{
    public bool dragonIsAttacked;
    Animator dragonAnimator;

    // Start is called before the first frame update
    void Start()
    {
        dragonAnimator = GetComponent<Animator>();
        dragonAnimator.SetBool("okay", true);
        dragonAnimator.SetBool("attacked", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (dragonIsAttacked)
        {
            dragonAnimator.SetBool("attacked", true);
            dragonAnimator.SetBool("okay", false);
        }
    }
}
