using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; //import

public class CircleMovingBlock : MonoBehaviour
{
    public GameObject[] targets;
    private Sequence mySequence;
    public int movingTime = 3; 
    private Coroutine DrawCircle;

    void Start()
    {
        
    }

    void Update()
    {
        if(DrawCircle == null)
        {
            DrawCircle = StartCoroutine("drawCircle");
        }
    }

    Sequence Quadrant12(int a){
        return DOTween.Sequence()
        .OnStart(() => {
            transform.DOMoveX(targets[a].transform.position.x, movingTime).SetEase(Ease.OutQuad);
            transform.DOMoveY(targets[a].transform.position.y, movingTime).SetEase(Ease.InQuad);
            transform.DOMoveZ(targets[a].transform.position.z, movingTime).SetEase(Ease.InQuad);
        })
        .SetDelay(movingTime);
    } 

    Sequence Quadrant34(int a){
        return DOTween.Sequence()
        .OnStart(() => {
            transform.DOMoveX(targets[a].transform.position.x, movingTime).SetEase(Ease.InQuad);
            transform.DOMoveY(targets[a].transform.position.y, movingTime).SetEase(Ease.OutQuad);
            transform.DOMoveZ(targets[a].transform.position.z, movingTime).SetEase(Ease.OutQuad);
        })
        .SetDelay(movingTime);
    } 

    IEnumerator drawCircle()
    {
        mySequence = Quadrant12(0)
        .Append(Quadrant34(1))
        .Append(Quadrant12(2))
        .Append(Quadrant34(3));
        yield return new WaitForSeconds( movingTime*4 );
        DrawCircle = null;
    }
}
