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

    [SerializeField] private float SpecialOrbInterval = 5;

    [SerializeField] private int maxNumofSpecialOrb = 3;
    private List<GameObject> Orbs = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        coolTime = SpecialOrbInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawnStop)
        {   
            spawnSpecialOrb();
            
            if(Orbs.Count == maxNumofSpecialOrb) 
            {
                if(Orbs.All(orb => orb == null)) 
                {
                    isSpawnStop = true;
                    resetAllValue();
                }
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
}
