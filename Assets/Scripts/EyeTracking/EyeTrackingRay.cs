using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    [SerializeField]
    private GameObject markerPrefab; 

    private GameObject markerSparks;
    
    [SerializeField]
    private float rayDistance = 100.0f;

    [SerializeField]
    private float rayWidth = 0.1f;

    [SerializeField]
    private LayerMask layersToInclude;

    [SerializeField]
    private Color rayColorDefaultState = Color.yellow;

    [SerializeField]
    private Color rayColorHoverState = Color.red;

    private LineRenderer lineRenderer;

    private GameObject EyeTargetingObject;

    public GameObject HoveredCube = null; 


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();  
        SetupRay();  

        markerSparks = markerPrefab.transform.GetChild(0).gameObject;
        markerSparks.SetActive(false);
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

            if(hit.transform != null && (hit.transform.gameObject.CompareTag("redCube") || hit.transform.gameObject.CompareTag("blueCube")))
            {
                EyeTargetingObject = hit.transform.gameObject;
                EyeTargetingObject.GetComponent<Cube>().IsHovered = true;
                HoveredCube = hit.transform.gameObject;
            }
            else if(hit.transform != null && hit.transform.gameObject.CompareTag("MovingOrb"))
            {
                EyeTargetingObject = hit.transform.gameObject; 
                EyeTargetingObject.GetComponent<Tracing>().IsHovered = true; 
                EyeTargetingObject.GetComponent<Tracing>().HoverPosition = hit.transform.position; 
            }
            
            /*
            var Cube = hit.transform.GetComponent<Cube>();
            if (Cube != null) // Add a null check here
            {
                EyeTargetingObject = Cube;
                Cube.IsHovered = true;
            }

            var Tracing = hit.transform.GetComponent<Tracing>();
            if (Tracing != null) // Add a null check here
            {
                EyeTargetingObject = Tracing;
                Tracing.IsHovered = true;
            }
            */

            SpawnMarker(hit);
        }
        else 
        {
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            UnSelect();

            // Ray가 아무것도 맞지 않은 경우 표식 비활성화
            DestroyMarker();
        }
    }


    void UnSelect()
    {
        if (EyeTargetingObject != null)
        {
            if (EyeTargetingObject.CompareTag("redCube") || EyeTargetingObject.CompareTag("blueCube"))
            {
                Cube cubeComponent = EyeTargetingObject.GetComponent<Cube>();
                if (cubeComponent != null)
                {
                    cubeComponent.IsHovered = false;
                }
            }
            else if (EyeTargetingObject.CompareTag("MovingOrb"))
            {
                Tracing tracingComponent = EyeTargetingObject.GetComponent<Tracing>();
                if (tracingComponent != null)
                {
                    tracingComponent.IsHovered = false;
                }

                if(markerSparks.activeSelf == false)
                    markerSparks.SetActive(true); 
            }

            EyeTargetingObject = null;
        }  
    }

    void SpawnMarker(RaycastHit hit)
    {
        if (markerPrefab.activeSelf == false)
        {
            // 표식이 없는 경우 생성
            markerPrefab.SetActive(true);
        }
        else
        {
            // 표식이 이미 있는 경우 위치를 업데이트 
            markerPrefab.transform.position = hit.point;
        }
    }

    void DestroyMarker()
    {
        // 표식이 존재하면 비활성화
        if (markerPrefab.activeSelf == true)
        {
            markerPrefab.SetActive(false);
        }
    }
}
