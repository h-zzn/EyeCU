using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpecialOrbSpawner : MonoBehaviour
{
    public bool isSpawnStop = true;
    [SerializeField] private GameObject[] SpecialOrbPrefab;
    
    public Transform[] points;

    private float coolTime = 0;

    public float SpecialOrbSpeed = 1;
    public float SpecialOrbInterval = 7.5f;

    [SerializeField] private int maxNumofSpecialOrb = 3;
    private List<GameObject> Orbs = new List<GameObject>();


    // Magic Circles
    [SerializeField] private GameObject magicCircle;
    [SerializeField] private bool isMagicCircleActive = false;

    private ParticleSystem magicCircleParticleSystem;

    // Start is called before the first frame update
    void Awake()
    {
        coolTime = SpecialOrbInterval;
    }

    private void Start()
    {
        // Get Particle System of Magic Circle
        magicCircleParticleSystem = magicCircle.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawnStop)    
        {
            if (Orbs.Count == maxNumofSpecialOrb)
            {
                if (Orbs.All(orb => orb == null))
                {
                    isSpawnStop = true;
                }
            }

            if (!isMagicCircleActive)
            {
                activateMagicCircle();
            }

            spawnSpecialOrb();
        }
        else      
        {
            if (isMagicCircleActive)
            {
                resetAllValue();
                deactivateMagicCircle();
            }
        }
    }

    public void spawnSpecialOrb()
    {
        if (coolTime > SpecialOrbInterval && Orbs.Count < maxNumofSpecialOrb) 
        {
            GameObject Orb = Instantiate(SpecialOrbPrefab[Random.Range(0,SpecialOrbPrefab.Length)], points[Random.Range(0, points.Length)]);
            Orb.transform.localPosition = Vector3.zero;
            Orb.transform.Rotate(transform.forward);
            Orb.transform.GetChild(0).GetComponent<Tracing>().movingTime /= SpecialOrbSpeed; 
            Orbs.Add(Orb);

            coolTime = 0;
        }
        coolTime += Time.deltaTime;
    }

    public void resetAllValue()
    {
        coolTime = SpecialOrbInterval;
        Orbs.Clear();
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
