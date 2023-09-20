using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_sound : MonoBehaviour
{
    private Animator dragonAnimator;
    private AudioSource dragonAudioSource;

    public AudioClip roarMainDragon;

    private bool goingToAttack;

    private void Start()
    {
        dragonAnimator = GetComponent<Animator>();
        dragonAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Check if the "GoingToAttack" parameter has changed to true.
        bool newGoingToAttack = dragonAnimator.GetBool("attacked");

        if (newGoingToAttack && !goingToAttack)
        {
            // The parameter has just changed to true; play the audio.
            PlayAudio(roarMainDragon);
        }

        goingToAttack = newGoingToAttack;
    }

    private void PlayAudio(AudioClip clip)
    {
        if (dragonAudioSource == null)
        {
            Debug.LogError("Dragon Audio Source is not assigned.");
            return;
        }

        if (clip == null)
        {
            Debug.LogError("Audio clip is not assigned.");
            return;
        }

        dragonAudioSource.clip = clip;
        dragonAudioSource.Play();
    }
}