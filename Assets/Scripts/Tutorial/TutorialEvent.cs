using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
    public bool magicRedOrbMission = false; 
    public bool magicBlueOrbMission = false;
    public bool magicFailMission = false;

    public bool specialOrbMission = false; 

    public bool lavaSwordMission = false; 
    public bool iceSwordMission = false;

    public bool lavaStoneMission = false; 
    public bool iceStoneMission = false; 


    public GameObject magicRedOrb; 
    public GameObject magicBlueOrb; 

    public GameObject specialOrb; 

    public GameObject lavaStone; 
    public GameObject iceStone; 

    private ControllerManager controllerManager;

    void Awake()
    {
        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();
    }

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
        if(controllerManager.blueMagicActive == false || controllerManager.redMagicActive == false)
        { 
            magicFailMission = true;
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
