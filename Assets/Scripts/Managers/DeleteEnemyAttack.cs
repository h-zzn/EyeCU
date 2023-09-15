using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DeleteEnemyAttack : MonoBehaviour
{
    private Vector3 OriginPosition;


    void Start()
    {
        OriginPosition = this.transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("blueCube") || other.gameObject.CompareTag("redCube") || other.gameObject.CompareTag("LavaStone") || other.gameObject.CompareTag("IceStone"))
        {
            Destroy(other.gameObject);  
        }
        else if (other.gameObject.CompareTag("MovingOrb"))
        {
            if (other.transform.parent != null)
            {
                Destroy(other.transform.parent.gameObject);  
            }
        }
    }

    private IEnumerator DeleteAll()
    {
        Vector3 targetPosition = OriginPosition + new Vector3(0f, 0f, 100f); // 이동할 목표 위치

        float startTime = Time.time;
        float journeyLength = Vector3.Distance(transform.localPosition, targetPosition);
        float journeyTime = 10f; // 이동하는 데 걸리는 시간 (초)

        while (Time.time - startTime < journeyTime)
        {
            float distanceCovered = (Time.time - startTime) * journeyLength / journeyTime;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, fractionOfJourney);
            yield return null;
        }

        // 이동이 완료되면 원래 위치로 돌아감
        transform.localPosition = OriginPosition;
    }

}
