using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // URP의 네임스페이스 추가
using UnityEngine.Rendering.PostProcessing; // Post-processing에 관련된 네임스페이스 추가


public class DamagedArea : MonoBehaviour
{
    public int stageHP = 2000; 
    // Assign Post Processing Volume from the Scene
    [SerializeField] private Volume postProcessingVolumeObject;
    private UnityEngine.Rendering.Universal.Vignette vignette;

    private float normalizedHP;

    [SerializeField] private float hitShakeTime = 0.2f;
    [SerializeField] private float hitShakeSpeed = 2.0f;
    [SerializeField] private float hitShakeAmount = 1.5f;

    [SerializeField] private GameObject HPGauge;
    [SerializeField] private GameObject HPMaterialObj;

    private Renderer HPGaugeRenderer; 

    private List<Material> HPMaterials; 

    private Transform cam;

    void Awake()
    {
        // Scene에서 OVRInPlayMode를 찾아 cam에 assign
        cam = GameObject.Find("OVRInPlayMode").transform;

        // Scene에서 PostProcessVolume을 가져옴
        postProcessingVolumeObject = GameObject.Find("Post Processing").GetComponent<Volume>(); 
        // PostProcessVolume에서 Vignette 설정 값을 가져옴
        postProcessingVolumeObject.profile.TryGet(out vignette); 

        // HPGauge의 Renderer를 가져오고 Material 리스트를 설정
        HPGaugeRenderer = HPGauge.GetComponent<Renderer>(); 
        HPMaterials = new List<Material>(HPMaterialObj.GetComponent<Renderer>().materials); 

    }

    void Update()
    {
        //stageHP 에 따라 HP 게이지 조절
        chargingHPGauge();
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
    }

    IEnumerator Shake()
    {
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

    void chargingHPGauge()
    {
        if (stageHP > 1900)
        {
            HPGaugeRenderer.material = HPMaterials[0];
        }
        else if (stageHP >= 1900)
        {
            HPGaugeRenderer.material = HPMaterials[1];
        }
        else if (stageHP >= 1700)
        {
            HPGaugeRenderer.material = HPMaterials[2];
        }
        else if (stageHP >= 1500)
        {
            HPGaugeRenderer.material = HPMaterials[3];
        }
        else if (stageHP >= 1300)
        {
            HPGaugeRenderer.material = HPMaterials[4];
        }
        else if (stageHP >= 1100)
        {
            HPGaugeRenderer.material = HPMaterials[5];
        }
        else if (stageHP >= 900)
        {
            HPGaugeRenderer.material = HPMaterials[6];
        }
        else if (stageHP >= 700)
        {
            HPGaugeRenderer.material = HPMaterials[7];
        }
        else if (stageHP >= 500)
        {
            HPGaugeRenderer.material = HPMaterials[8];
        }
        else if (stageHP >= 300)
        {
            HPGaugeRenderer.material = HPMaterials[9];
        }
        else if (stageHP >= 100)
        {
            HPGaugeRenderer.material = HPMaterials[10];
        }
        else
        {
            HPGaugeRenderer.material = HPMaterials[11];
        }
    }
}
