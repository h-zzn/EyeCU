using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; //import

public class CircleMovingBlock : MonoBehaviour
{
    public GameObject target;

    void Start()
    {
        transform.DOMoveX(target.transform.position.x, 3).SetEase(Ease.OutQuad);
        transform.DOMoveY(target.transform.position.y, 3).SetEase(Ease.InQuad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
