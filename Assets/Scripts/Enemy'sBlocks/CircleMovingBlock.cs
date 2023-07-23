using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; //import

public class CircleMovingBlock : MonoBehaviour
{
    public GameObject[] targets;
    Sequence mySequence;

    void Start()
    {
        mySequence = DOTween.Sequence()
        .OnStart(() => {
            transform.DOMoveX(targets[0].transform.position.x, 3).SetEase(Ease.OutQuad);
            transform.DOMoveY(targets[0].transform.position.y, 3).SetEase(Ease.InQuad);
            transform.DOMoveZ(targets[0].transform.position.z, 3).SetEase(Ease.InQuad);
        })
        .SetDelay(0.5f)
        .Append(MySequence2(1))
        .Append(MySequence2(2))
        .Append(MySequence2(3));
    }
    
    void Update()
    {
        
    }

    Sequence MySequence2(int a){
        return DOTween.Sequence()
        .OnStart(() => {
            transform.DOMoveX(targets[a].transform.position.x, 3).SetEase(Ease.OutQuad);
            transform.DOMoveY(targets[a].transform.position.y, 3).SetEase(Ease.InQuad);
            transform.DOMoveZ(targets[a].transform.position.z, 3).SetEase(Ease.InQuad);
        })
        .SetDelay(0.5f);
    }
}
