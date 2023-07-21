using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    [SerializeField]
    private float rayDistance = 100.0f;

    [SerializeField]
    private float rayWidth = 0.01f;

    [SerializeField]
    private LayerMask layersToInclude;

    [SerializeField]
    private Color rayColorDefaultState = Color.yellow;

    [SerializeField]
    private Color rayColorHoverState = Color.red;

    private LineRenderer lineRenderer;

    private List<EyeInteractable> eyeInteractables = new List<EyeInteractable>();

    public GameObject HoveredCube = null; 


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.startColor = rayColorDefaultState;
        lineRenderer.endColor = rayColorDefaultState;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z+rayDistance));
    }

    void FixedUpdate()
{
    RaycastHit hit;

    Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward) * rayDistance;

    if (Physics.Raycast(transform.position, rayCastDirection, out hit, Mathf.Infinity, layersToInclude)) 
    {
        UnSelect(); // Change this line
        lineRenderer.startColor = rayColorHoverState;
        lineRenderer.endColor = rayColorHoverState;
        var eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
        if (eyeInteractable != null) // Add a null check here
        {
            eyeInteractables.Add(eyeInteractable);
            eyeInteractable.IsHovered = true;
        }
    }
    else 
    {
        lineRenderer.startColor = rayColorDefaultState;
        lineRenderer.endColor = rayColorDefaultState;
        UnSelect(true);
    }

    if(hit.transform != null && hit.transform.gameObject.CompareTag("Cube")) // Add a null check here
        HoveredCube = hit.transform.gameObject;
    }


    void UnSelect(bool clear = false)
    {
        foreach (var interactable in eyeInteractables)
        {
            interactable.IsHovered = false;
        }
        if (clear)
        {
            eyeInteractables.Clear();
        }
    }
}
