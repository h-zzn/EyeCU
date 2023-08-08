using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{

    static public SoundManager instance;

    #region singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion singleton

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBgm;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;

    public void PlaySE(string _name)
    {
        for (int i=0; i<effectSounds.Length; i++)
        {
            if(_name == effectSounds[i].name)
            {
                for (int j=0; j<audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[i].isPlaying)
                    {
                        audioSourceEffects[j];
                    }
                }
            }
        }

    }

    
    //매번 활성화 되면 실행. 코루틴 X 
    void onEnable()
    {

    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
