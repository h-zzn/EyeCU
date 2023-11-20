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
    [SerializeField] private GameObject magicCircle;
    [SerializeField] private bool isMagicCircleActive = false;

    private ParticleSystem magicCircleParticleSystem;

    private void Start()
    {
        // Get Particle System of Magic Circle
        magicCircleParticleSystem = magicCircle.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!isSpawnStop)
        {
            if(!isMagicCircleActive){
                activateMagicCircle();
            }
            spawnOrb();
        }
        else 
        {
            if(isMagicCircleActive){
                deactivateMagicCircle();
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
            cube.GetComponent<Cube>().moveSpeed *= OrbSpeed; 
            coolTime = 0;
            numOfBasicOrb += 1;
        }       
        coolTime += Time.deltaTime;
    }

    // Activate Regular Magic Cirle
    public void activateMagicCircle()
    {
        isMagicCircleActive = true;
        
        if (magicCircle != null)
        {
            // Start Particle System
            magicCircleParticleSystem.Play();
        }
    }

    // Deactivate Regular Magic Cirle
    public void deactivateMagicCircle()
    {
        isMagicCircleActive = false;

        if (magicCircle != null)
        {
            // Stop Particle System
            magicCircleParticleSystem.Stop();
        }
    }
}
