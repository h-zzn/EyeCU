using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealWeakness : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 현재 카메라 컴포넌트 가져오기
        Camera camera = GetComponent<Camera>();

        // 현재 Culling Mask 가져오기
        LayerMask currentCullingMask = camera.cullingMask;

        // 가져온 Culling Mask를 출력
        Debug.Log("Current Culling Mask: " + currentCullingMask.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
