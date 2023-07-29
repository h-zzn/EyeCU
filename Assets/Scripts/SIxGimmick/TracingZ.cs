using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class TracingZ : MonoBehaviour
{
    public GameObject[] targets;
    private Sequence mySequence;
    public float movingTime = 3; 
    [SerializeField] private float HMT = 1;
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
            DrawCircle = StartCoroutine("drawZ");
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
            transform.DOMoveX(targets[a].transform.position.x, movingTime);
            transform.DOMoveY(targets[a].transform.position.y, movingTime);
            transform.DOMoveZ(targets[a].transform.position.z, movingTime);
        })
        .SetDelay(movingTime);
    } 

    Sequence Quadrant34(int a){
        return DOTween.Sequence()
        .OnStart(() => {
            transform.DOMoveX(targets[a].transform.position.x, movingTime*HMT);
            transform.DOMoveY(targets[a].transform.position.y, movingTime*HMT);
            transform.DOMoveZ(targets[a].transform.position.z, movingTime*HMT);
        })
        .SetDelay(movingTime);
    } 

    IEnumerator drawZ()
    {
        mySequence = Quadrant12(0)
        .Append(Quadrant34(1))
        .Append(Quadrant12(2))
        .Append(Quadrant34(3));
        yield return new WaitForSeconds( movingTime*4 );
        DrawCircle = null;
    }
}

