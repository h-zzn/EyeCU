using UnityEngine;

public class IntroBgm : MonoBehaviour
{
    private void Start()
    {
        SoundManager.instance.PlayBGM("Intro");
    }
}
