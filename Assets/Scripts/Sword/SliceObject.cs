using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;

public class SliceObject : MonoBehaviour
{

    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator velocityEstimator;
    public LayerMask sliceableLayer;

    public Material crossSectionMaterial;
    public float cutForce = 2000f ;
    // Start is called before the first frame update
    void Start()
    {
        
    }
     
    // Update is called once per frame
    void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if (hasHit)
        {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }


    }
    public void Slice(GameObject target)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();
        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if(hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull);
            ChangeLayer(upperHull);
            

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull);
            ChangeLayer(lowerHull);


            Destroy(target);
        }
    }
    public void ChangeLayer(GameObject target)
    {
        int layer = LayerMask.NameToLayer("Sliceable");
        target.layer = layer;
       
    }

    public void SetupSlicedComponent(GameObject sliceObject)
    {
        Rigidbody rb = sliceObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = 5;
        MeshCollider collider = sliceObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(cutForce, sliceObject.transform.position, 1);

    }
}