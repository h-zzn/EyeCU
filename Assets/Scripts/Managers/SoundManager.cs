using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData
{
    public string title;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public List<AudioData> bgmList;
    public List<AudioData> sfxList;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            bgmSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            foreach (AudioData bgmData in bgmList)
            {
                bgmClips[bgmData.title] = bgmData.clip;
            }

            foreach (AudioData sfxData in sfxList)
            {
                sfxClips[sfxData.title] = sfxData.clip;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(string bgmTitle)
    {
        if (!bgmClips.ContainsKey(bgmTitle))
        {
            Debug.LogError("Invalid BGM title");
            return;
        }

        bgmSource.clip = bgmClips[bgmTitle];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(string sfxTitle)
    {
        if (!sfxClips.ContainsKey(sfxTitle))
        {
            Debug.LogError("Invalid SFX title");
            return;
        }

        sfxSource.PlayOneShot(sfxClips[sfxTitle]);
    }
}
