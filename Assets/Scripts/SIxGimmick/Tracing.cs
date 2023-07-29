using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Tracing : MonoBehaviour
{
    public GameObject[] targets;
    private Sequence mySequence;
    public int movingTime = 3; 
    private Coroutine DrawCircle;

    [SerializeField] private AudioSource UngSound;
    private float hoverDuration = 0f;
    private float maxHoverDuration = 5f; // 5 seconds
    public bool IsHovered { get; set; }
    private bool hasAudioPlayed = false;

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

            if (!hasAudioPlayed && UngSound != null)
            {
                UngSound.Play();
                hasAudioPlayed = true; // Set the flag to true to indicate audio has played.
            }

            if (hoverDuration >= maxHoverDuration)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
        else
        {
            if (UngSound != null)
            {
                UngSound.Stop();
                hasAudioPlayed = false; // Reset the flag when the object is no longer hovered.
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("TargetPosition"))
        {
            other.gameObject.transform.position += transform.forward*4;
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
            transform.DOMoveX(targets[a].transform.position.x, movingTime*2).SetEase(Ease.InQuad);
            transform.DOMoveY(targets[a].transform.position.y, movingTime*2).SetEase(Ease.OutQuad);
            transform.DOMoveZ(targets[a].transform.position.z, movingTime*2).SetEase(Ease.OutQuad);
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

