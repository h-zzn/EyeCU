using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DamagedArea : MonoBehaviour
{
    public int stageHP = 2000; 
    // Assign Post Processing Volume from the Scene
    [SerializeField] private PostProcessVolume postProcessingVolumeObject;
    private Vignette vignette;

    private float normalizedHP;

    [SerializeField] private float hitShakeTime = 0.2f;
    [SerializeField] private float hitShakeSpeed = 2.0f;
    [SerializeField] private float hitShakeAmount = 1.5f;

    [SerializeField] private Transform cam;

    void Awake()
    {
        // Scene에서 OVRInPlayMode를 찾아 cam에 assign
        cam = GameObject.Find("OVRInPlayMode").transform;

        // Scene에서 PostProcessVolume을 가져옴
        postProcessingVolumeObject = GameObject.Find("Post Processing").GetComponent<PostProcessVolume>(); 
        // PostProcessVolume에서 Vignette 설정 값을 가져옴
        postProcessingVolumeObject.profile.TryGetSettings(out vignette);
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("blueCube") || other.gameObject.CompareTag("redCube") || other.gameObject.CompareTag("LavaStone") || other.gameObject.CompareTag("IceStone"))
        {
            stageHP -= 100;
            // Shake the camera
            StartCoroutine(Shake());
            Destroy(other.gameObject); 
        }
        else if(other.gameObject.CompareTag("MovingOrb"))
        {
            if (other.transform.parent != null)
            {
                stageHP -= 500;
                // Shake the camera
                StartCoroutine(Shake());
                Destroy(other.transform.parent.gameObject); 
            }
        }

        // Calculate the normalized HP
        normalizedHP = 1 - stageHP / 2000f;

        // Set the intensity of the Vignette of the Post Processing Volume to the current stageHP
        vignette.intensity.Override(normalizedHP * normalizedHP);

        // Print the current stageHP and the intensity of the Vignette
        Debug.Log("StageHP: " + stageHP);
        Debug.Log("Intensity: " + vignette.intensity.value);
    }

    IEnumerator Shake()
    {
        Debug.Log("Hit Shake!!!!!!!!!!!");
        Vector3 originPosition = cam.localPosition;
        float elapsedTime = 0.0f;

        while (elapsedTime < hitShakeTime)
        {
            Vector3 randomPoint = originPosition + Random.insideUnitSphere * hitShakeAmount;
            cam.localPosition = Vector3.Lerp(cam.localPosition, randomPoint, Time.deltaTime * hitShakeSpeed);

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        cam.localPosition = originPosition;
    }
}
