using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
    public bool magicRedOrbMission = false; 
    public bool magicBlueOrbMission = false; 

    public bool specialOrbMission = false; 

    public bool lavaSwordMission = false; 
    public bool iceSwordMission = false;

    public bool lavaStoneMission = false; 
    public bool iceStoneMission = false; 


    [SerializeField] public GameObject magicRedOrb; 
    [SerializeField] public GameObject magicBlueOrb; 

    [SerializeField] public GameObject specialOrb; 

    [SerializeField] public GameObject lavaStone; 
    [SerializeField] public GameObject iceStone; 


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

        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            lavaSwordMission = true;
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            iceSwordMission = true;
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
