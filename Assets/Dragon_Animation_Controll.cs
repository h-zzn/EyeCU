using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_Animation_Controll : MonoBehaviour
{
    public bool dragonIsAttacked;
    Animator dragonAnimator;

    // Add a boolean to track whether the audio has been played
    private bool hasAudioPlayed = false;

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
        if (dragonIsAttacked && !hasAudioPlayed)
        {
            dragonAnimator.SetBool("attacked", true);
            dragonAnimator.SetBool("okay", false);

            // Play the second audio source and mark it as played
            dragonAudioSource2.Play();
            hasAudioPlayed = true;
        }
    }
}
