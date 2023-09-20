using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_Animation_Controll : MonoBehaviour
{
    public bool dragonIsAttacked;
    Animator dragonAnimator;

    // Reference to the second AudioSource
    public AudioSource dragonAudioSource2;

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

            // Play the second audio source
            dragonAudioSource2.Play();

            // Reset dragonIsAttacked to false immediately after playing audio
            dragonIsAttacked = false;

            // Reset animator parameters (if needed) immediately after playing audio
            dragonAnimator.SetBool("okay", true);
            dragonAnimator.SetBool("attacked", false);
        }
    }
}
