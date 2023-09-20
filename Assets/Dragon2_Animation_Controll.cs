using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon2_Animation_Controll : MonoBehaviour
{
    public bool dragonIsAttacked;
    public bool dragonIsDead;

    Animator Dragon2_Animation;

    // Reference to the second AudioSource
    public AudioSource dragonAudioSource2;

    // Start is called before the first frame update
    void Start()
    {
        Dragon2_Animation = GetComponent<Animator>();
        Dragon2_Animation.SetBool("okay", true);
        Dragon2_Animation.SetBool("attacked", false);
        Dragon2_Animation.SetBool("death", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (dragonIsAttacked)
        {
            Dragon2_Animation.SetBool("attacked", true);
            Dragon2_Animation.SetBool("okay", false);

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
            Dragon2_Animation.SetBool("okay", false);
            Dragon2_Animation.SetBool("death", true);
        }
    }

    private IEnumerator ResetAnimatorParametersWithDelay()
    {
        // Wait for about 2 seconds
        yield return new WaitForSeconds(2.0f);

        // Reset animator parameters
        Dragon2_Animation.SetBool("okay", true);
        Dragon2_Animation.SetBool("attacked", false);
    }
}
