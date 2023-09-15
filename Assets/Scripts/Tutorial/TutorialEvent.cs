using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
    public bool magicRedOrbMission = false; 
    public bool magicBlueOrbMission = false;
    public bool magicTaegukOrbMission = false;

    public bool specialOrbMission = false; 

    public bool lavaSwordMission = false; 
    public bool iceSwordMission = false;

    public bool lavaStoneMission = false;  
    public bool iceStoneMission = false;

    public bool HPMission = false;
    public bool MPMission = false;
    public bool magicFailMission = false;
    public bool skillActivateMission = false;

    public GameObject magicRedOrb;  
    public GameObject magicBlueOrb;
    public GameObject magicTaegukOrb;

    public GameObject specialOrb;  

    public GameObject lavaStone;   
    public GameObject iceStone;  

    private ControllerManager controllerManager; 
    private DamagedArea damagedArea;

    void Awake()
    {
        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();
        damagedArea = GameObject.Find("StageCore").transform.GetComponent<DamagedArea>();
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
        if (magicTaegukOrb == null)
        {
            magicTaegukOrbMission = true;
        }
        

        if(specialOrb == null)
        { 
            specialOrbMission = true;
        }

        if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            lavaSwordMission = true;
        }
        else
        {
            lavaSwordMission = false;
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            iceSwordMission = true;
        }
        else
        {
            iceSwordMission = false;
        }


        if (lavaStone == null)
        { 
            lavaStoneMission = true;
        }
        if(iceStone == null)
        { 
            iceStoneMission = true;
        }


        if(damagedArea.stageHP <= 1900)
        {
            HPMission = true; 
        }
        if(controllerManager.skillEnergyPoint >= 200)
        {
            MPMission = true; 
        }
        else
        {
            MPMission = false;
        }
        if (controllerManager.blueMagicActive == false || controllerManager.redMagicActive == false)
        {
            magicFailMission = true; 
        }
        if( controllerManager.handEffectCollision.canUseSkill == true)
        {
            skillActivateMission = true;
        }
    }
}
