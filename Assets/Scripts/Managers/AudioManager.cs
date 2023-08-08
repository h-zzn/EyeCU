using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public List<AudioClip> bgmClips;
    public List<AudioClip> sfxClips;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            bgmSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(int bgmIndex)
    {
        if (bgmIndex < 0 || bgmIndex >= bgmClips.Count)
        {
            Debug.LogError("Invalid BGM index");
            return;
        }

        bgmSource.clip = bgmClips[bgmIndex];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(int sfxIndex)
    {
        if (sfxIndex < 0 || sfxIndex >= sfxClips.Count)
        {
            Debug.LogError("Invalid SFX index");
            return;
        }

        sfxSource.PlayOneShot(sfxClips[sfxIndex]);
    }
}
