using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
    public bool magicRedOrbMission = false;
    public bool magicBlueOrbMission = false;

    public bool specialOrbMission = false;

    public bool stoneSwordMission = false;
    public bool lavaStoneMission = false;
    public bool iceStoneMission = false;


    [SerializeField] private GameObject magicRedOrb; 
    [SerializeField] private GameObject magicBlueOrb; 

    [SerializeField] private GameObject specialOrb; 

    [SerializeField] private GameObject lavaStone; 
    [SerializeField] private GameObject iceStone; 


    void Update()
    {
        if(magicRedOrb == null)
        { 
            magicRedOrbMission = true;
        } 
        if(magicBlueOrb == null)
        { 
            magicBlueOrbMission = true;
        } 


        if(specialOrb == null)
        { 
            specialOrbMission = true;
        }


        if(lavaStone == null)
        { 
            lavaStoneMission = true;
        }
        if(iceStone == null)
        { 
            iceStoneMission = true;
        }
    }
}
