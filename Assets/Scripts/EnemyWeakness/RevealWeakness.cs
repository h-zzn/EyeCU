using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealWeakness : MonoBehaviour
{
    [SerializeField] private GameObject monsterObject;
    private Material material;
    
    // Start is called before the first frame update
    void Start()
    {
        if (material == null)
        {
            material = monsterObject.GetComponent<Renderer>().material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void changeTransparent(){
        //셰이더에서 Transparent로 바꾸기
    }
}
