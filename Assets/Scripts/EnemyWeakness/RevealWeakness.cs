using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealWeakness : MonoBehaviour
{
    [SerializeField] private Material transparentMaterial;
    private Material originalMaterial;

    public bool isTransparent = false;
    
    // LayerMask originalCullingMask;
    
    // int layerMaskValue = (1 << 5) | (1 << 2);
    // LayerMask layerMask = layerMaskValue;
    // Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        // 현재 오브젝트의 Material 컴포넌트 가져와 저장
        originalMaterial = GetComponent<Renderer>().material;

        // // 현재 카메라 컴포넌트 가져오기
        // camera = GetComponent<Camera>();

        // // 현재 Culling Mask 가져오기
        // originalCullingMask = camera.cullingMask;

        // // 가져온 Culling Mask를 출력
        // Debug.Log("Current Culling Mask: " + originalCullingMask.value);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void turnMonsterTransparent()
    {
        // 현재 오브젝트의 Material 컴포넌트를 투명한 Material로 변경
        GetComponent<Renderer>().material = transparentMaterial;
    }
}
