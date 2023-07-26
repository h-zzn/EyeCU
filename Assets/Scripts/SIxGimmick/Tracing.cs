using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Tracing : MonoBehaviour
{
    public GameObject[] targets;
    private Sequence mySequence;
    public int movingTime = 3; 
    private Coroutine DrawCircle;


    private float hoverDuration = 0f;
    [SerializeField] private float maxHoverDuration = 5f; // 5 seconds
    public bool IsHovered { get; set; }

    private void Awake() 
    {
        IsHovered = false; 
    }

    void Update()
    {
        if(DrawCircle == null)
        {
            DrawCircle = StartCoroutine("drawCircle");
        }

        // Check if the object is being gazed upon
        if (IsHovered)
        {
            // Increment the hover duration
            hoverDuration += Time.deltaTime;

            if (hoverDuration >= maxHoverDuration)
            {
                // Destroy the object after 5 seconds of continuous gaze
                Destroy(gameObject);
            }
        }
        else
        {
            // Reset the hover duration if the gaze is interrupted
            hoverDuration = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("TargetPosition"))
        {
            other.gameObject.transform.position += transform.forward*2;
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

