using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sound_roar : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;

    void Start()
    {
        // Play the first audio source immediately
        audioSource1.Play();

        // Invoke the PlayDelayed function to play audioSource2 after a delay of 2 seconds
        Invoke("PlayDelayed", 2.0f);
    }

    // Function to play audioSource2 after the delay
    void PlayDelayed()
    {
        audioSource2.Play();
    }
}
