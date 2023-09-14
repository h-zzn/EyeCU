using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool isSpawnStop = true;

    public GameObject[] cubes; 
    public Transform[] points; 
    public float Interval = (60/130)*2; 
    private float coolTime = 0;  

    public float OrbSpeed = 1;  

    public int numOfBasicOrb = 0;
    
    // Magic Circles
    [SerializeField] private List<GameObject> regMagicCircles;
    private bool isRegMagicCircleActive = false;
    [SerializeField] private List<GameObject> stoneMagicCircles;
    private bool isStoneMagicCircleActive = false;

    private void Start()
    {
        foreach (GameObject spawnObject in cubes) 
        {
            spawnObject.GetComponent<Cube>().moveSpeed *= OrbSpeed; 
        }   
    }

    void Update()
    {
        if (!isSpawnStop)
        {
            if(!isRegMagicCircleActive){
                activateRegMagicCircle();
            }
            spawnOrb();
        }
        else 
        {
            if(isRegMagicCircleActive){
                deactivateRegMagicCircle();
            }
        }
    }

    public void spawnOrb()
    {
        if(coolTime > Interval) 
        {
            GameObject cube = Instantiate(cubes[Random.Range(0,cubes.Length)],points[Random.Range(0,points.Length)]);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.Rotate(transform.forward);
            coolTime = 0;
            numOfBasicOrb += 1;
        }       
        coolTime += Time.deltaTime;
    }

    // Activate Regular Magic Cirle
    public void activateRegMagicCircle()
    {
        isRegMagicCircleActive = true;
        foreach (GameObject magicCircle in regMagicCircles)
        {
            if (magicCircle != null)
            {
                magicCircle.SetActive(true);
            }
        }
    }

    // Deactivate Regular Magic Cirle
    public void deactivateRegMagicCircle()
    {
        isRegMagicCircleActive = false;

        foreach (GameObject magicCircle in regMagicCircles)
        {
            if (magicCircle != null)
            {
                magicCircle.SetActive(false);
            }
        }
    }

    // // Activate Stone Magic Cirle
    // public void activateStoneMagicCircle()
    // {
    //     if (stoneMagicCircles != null)
    //     {
    //         isStoneMagicCircleActive = true;
    //         stoneMagicCircle.SetActive(true);
    //     }
    // }

    // // Deactivate Stone Magic Cirle
    // public void deactivateStoneMagicCircle()
    // {
    //     if (stoneMagicCircles != null)
    //     {
    //         isStoneMagicCircleActive = false;
    //         stoneMagicCircle.SetActive(false);
    //     }
    // }
}
