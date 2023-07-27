using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool isSpawnStop = true;

    public GameObject[] cubes;
    public Transform[] points;
    public float beat = (60/130)*2;
    private float coolTime = 0;

    private float Timer = 0;
    [SerializeField] private float startDelayTime;

    // Update is called once per frame
    void Update()
    {
        if(!isSpawnStop)
        {
            if(Timer > startDelayTime)
                spawnOrb();

            if(Timer <= startDelayTime + 0.1f)
                Timer += Time.deltaTime;
        }
    }

    public void spawnOrb()
    {
        if(coolTime > beat) 
        {
            GameObject cube = Instantiate(cubes[Random.Range(0,2)],points[Random.Range(0,16)]);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.Rotate(transform.forward);
            coolTime = 0;
        }       
        coolTime += Time.deltaTime;
    }
}
