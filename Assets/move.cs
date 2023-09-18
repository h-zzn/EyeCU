using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Import the DoTween namespace


public class move : MonoBehaviour
{
public float yOffset = -20.0f;  // The Y offset (-20 in this case)
    public float zOffset = 20.0f;   // The Z offset (+20 in this case)
    public float duration = 2.0f;   // Duration of the tween animation

    void Start()
    {
        // Get the current position of the GameObject
        Vector3 currentPosition = transform.position;

        // Calculate the new position with the desired offsets
        Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y + yOffset, currentPosition.z + zOffset);

        // Use DoTween to tween the GameObject's position to the new position
        transform.DOMove(newPosition, duration);
    }
}