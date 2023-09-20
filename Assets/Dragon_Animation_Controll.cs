using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_Animation_Controll : MonoBehaviour
{
    public bool dragonIsAttacked;
    public bool dragonIsDead;

    Animator dragonAnimator;

    // Reference to the second AudioSource
    public AudioSource dragonAudioSource2;

    // Start is called before the first frame update
    void Start()
    {
        dragonAnimator = GetComponent<Animator>();
        dragonAnimator.SetBool("okay", true);
        dragonAnimator.SetBool("attacked", false);
        dragonAnimator.SetBool("death", false);
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

            // Use StartCoroutine to introduce a delay before resetting animator parameters
            StartCoroutine(ResetAnimatorParametersWithDelay());
        }

        // Check if dragonIsDead is true and set the "death" parameter accordingly
        if (dragonIsDead)
        {
            dragonAnimator.SetBool("death", true);
        }
    }

    private IEnumerator ResetAnimatorParametersWithDelay()
    {
        // Wait for about 2 seconds
        yield return new WaitForSeconds(2.0f);

        // Reset animator parameters
        dragonAnimator.SetBool("okay", true);
        dragonAnimator.SetBool("attacked", false);
    }
}
