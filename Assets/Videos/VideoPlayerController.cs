using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public GameObject myVideo;

    public VideoPlayer videoClip;

    void Start()
    {
        // VideoPlayer 컴포넌트를 가져오거나 추가합니다.
        myVideo.SetActive(true);

        // 영상을 재생합니다.
        videoClip.Play();
    }
}