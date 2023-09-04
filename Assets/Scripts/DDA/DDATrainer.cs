using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DDATrainer : Agent
{

    void Start()
    {
        
    }

    public override void OnEpisodeBegin()
    {
    	
    }

    public override void CollectObservations(VectorSensor sensor)  
    {
        // 정보 수집
        sensor.AddObservation(1);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {    
    	// Agent가 Target쪽으로 이동하기 위해 X, Z축으로의 Force를 정의
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        // rBody.AddForce(controlSignal * forceMultiplier);

        // Agent와 Target사이의 거리를 측정
        float distanceToTarget = 0.1f;

        // Target에 도달하는 경우 (거리가 1.42보다 작은 경우) Episode 종료
        if (distanceToTarget < 1.42)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // 플랫폼 밖으로 나가면 Episode 종료
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}