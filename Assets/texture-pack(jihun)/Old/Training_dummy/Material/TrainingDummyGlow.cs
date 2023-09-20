using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummyGlow : MonoBehaviour
{
    // Success Material
    [SerializeField] private Material successMaterial;
    // Normal Material
    [SerializeField] private Material normalMaterial;

    [SerializeField] private GameObject good;

    // Get the skinned mesh renderer
    private SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField] private bool isGlowing = false;

    [SerializeField] private float glowTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set the glowing on and turn off after 1 second
    public void SetGlowing()
    {
        skinnedMeshRenderer.material = successMaterial;
        good.SetActive(true);
        Invoke("SetNotGlowing", glowTime);
    }

    // Set the glowing off
    private void SetNotGlowing()
    {
        skinnedMeshRenderer.material = normalMaterial;
        good.SetActive(false);
    }
}
