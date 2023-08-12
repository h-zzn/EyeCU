using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_dragon : MonoBehaviour
{
 public float rotationSpeed = 30.0f; // Speed of rotation in degrees per second
    public float radius = 5.0f; // Radius of the circular path
    public Vector3 center = Vector3.zero; // Center of the circular path

    private float angle = 0.0f;

    void Update()
    {
        // Calculate the new position based on circular motion equation
        float x = center.x + Mathf.Cos(angle) * radius;
        float y = center.y; // You can set this to center.y if you want the object to move up/down as well
        float z = center.z + Mathf.Sin(angle) * radius;

        Vector3 newPosition = new Vector3(x, y, z);

        // Update the GameObject's position
        transform.position = newPosition;

        // Increment the angle based on time and rotation speed
        angle += rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;

        // Reset the angle when it completes a full circle
        if (angle >= Mathf.PI * 2)
        {
            angle = 0.0f;
        }
    }
}
