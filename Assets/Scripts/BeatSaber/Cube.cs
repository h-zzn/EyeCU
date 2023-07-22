using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private EyeInteractable eyeInteractable;

    // Start is called before the first frame update
    void Awake()
    {
        eyeInteractable = this.GetComponent<EyeInteractable>();
    }

    // Update is called once per farame
    void Update()
    {
        if(!eyeInteractable.IsHovered)
        {
            transform.position += transform.forward/8;
        }
    }
}
