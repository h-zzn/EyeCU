using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpecialOrbSpawner : MonoBehaviour
{
    public bool isSpawnStop = true;
    public GameObject[] basicSpawner;
    [SerializeField] private GameObject SpecialOrbPrefab;
    
    public Transform[] points;

    private float coolTime = 0;
    private float Timer = 0;

    [SerializeField] private float startDelayTime = 17;
    [SerializeField] private float SpecialOrbInterval = 5;

    [SerializeField] private int maxNumofSpecialOrb = 3;
    public List<GameObject> Orbs = new List<GameObject>();

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
            if (Timer > startDelayTime)
               spawnSpecialOrb();
            Timer += Time.deltaTime;
            
            if(Orbs.Count == maxNumofSpecialOrb) 
            {
                if(Orbs.All(orb => orb == null)) //정해진 오브들이 다 사라지면 스폰 중지
                {
                    BasicSpawnStop(false);

                    resetAllValue();
                }
            }
        }    
    }

    //특별한 물체를 소환
    public void spawnSpecialOrb()
    {
        if (coolTime > SpecialOrbInterval && Orbs.Count < maxNumofSpecialOrb) 
        {
            GameObject Orb = Instantiate(SpecialOrbPrefab, points[Random.Range(0, points.Length)]);
            Orb.transform.localPosition = Vector3.zero;
            Orb.transform.Rotate(transform.forward);
            Orbs.Add(Orb);

            // Stop spawning for each basicSpawner
            BasicSpawnStop(true);
            coolTime = 0;
        }
        coolTime += Time.deltaTime;
    }

    //다른 스포너 멈춤 여부 
    public void BasicSpawnStop(bool stop)
    {
        foreach (GameObject spawner in basicSpawner)
        {
            spawner.GetComponent<Spawner>().isSpawnStop = stop;
        }
    }

     //변수 리셋
    public void resetAllValue()
    {
        isSpawnStop = true;
        Timer = 0;
        coolTime = SpecialOrbInterval;
        Orbs.Clear();
    }
}
