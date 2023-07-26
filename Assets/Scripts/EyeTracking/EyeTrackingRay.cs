using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    [SerializeField]
    private Transform markerPrefab; 
    


    private Transform currentMarker;
    
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

    private List<Cube> Cubes = new List<Cube>();
    private List<Tracing> Tracings = new List<Tracing>();

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
            var Cube = hit.transform.GetComponent<Cube>();
            if (Cube != null) // Add a null check here
            {
                Cubes.Add(Cube);
                Cube.IsHovered = true;
            }

            var Tracing = hit.transform.GetComponent<Tracing>();
            if (Tracing != null) // Add a null check here
            {
                Tracings.Add(Tracing);
                Tracing.IsHovered = true;
            }

            if (currentMarker == null)
                {
                    // 표식이 없는 경우 새로 생성
                    currentMarker = SpawnMarker(hit.point, hit.normal);
                }
                else
                {
                    // 표식이 이미 있는 경우 위치를 업데이트 
                    currentMarker.position = hit.point;
                }
        }
        else 
        {
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            UnSelect(true);

            // Ray가 아무것도 맞지 않은 경우 표식 삭제
            DestroyMarker();
        }

        if(hit.transform != null && 
        ((hit.transform != null && (hit.transform.gameObject.CompareTag("redCube") || hit.transform.gameObject.CompareTag("blueCube"))))) // Add a null check here
            HoveredCube = hit.transform.gameObject;
    }


    void UnSelect(bool clear = false)
    {
        foreach (var interactable in Cubes)
        {
            interactable.IsHovered = false;
        }
        if (clear)
        {
            Cubes.Clear();
        }

        foreach (var interactable in Tracings)
        {
            interactable.IsHovered = false;
        }
        if (clear)
        {
            Tracings.Clear();
        }  
    }

    Transform SpawnMarker(Vector3 position, Vector3 normal)
    {
        // 표식 프리팹을 생성 위치에 생성하고 Rotation 값을 유지
        Transform marker = Instantiate(markerPrefab, position, Quaternion.FromToRotation(Vector3.up, normal));
        return marker;
    }

    void DestroyMarker()
    {
        // 표식이 존재하면 삭제
        if (currentMarker != null)
        {
            Destroy(currentMarker.gameObject);
            currentMarker = null;
        }
    }
}
