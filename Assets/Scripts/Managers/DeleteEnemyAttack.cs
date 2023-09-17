using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteEnemyAttack : MonoBehaviour
{
    private Vector3 OriginPosition;

    private ControllerManager controllerManager;

    void Start()
    {
        OriginPosition = this.transform.localPosition;

        StartCoroutine("DeleteAll");

        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();

    }

    /*    private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("blueCube") || other.gameObject.CompareTag("redCube") || other.gameObject.CompareTag("LavaStone") || other.gameObject.CompareTag("IceStone"))
            {
                Destroy(other.gameObject);  
            }
            else if (other.gameObject.CompareTag("MovingOrb"))
            {
                if (other.transform.parent != null)
                {
                    hitEffectPosition = other.gameObject.transform.parent.position;
                    GameObject redMagicHitInstance = Instantiate(RedMagicHitEffectPrefab, hitEffectPosition, Quaternion.identity);
                    redMagicHitInstance.SetActive(true);
                    Destroy(redMagicHitInstance, 3f);
                    Destroy(other.transform.parent.gameObject);  
                }
            }
        }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("blueCube") || other.gameObject.CompareTag("IceStone"))
        {
            DisplayHitEffect(other.transform.position, controllerManager.BlueMagicHitEffectPrefab);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("redCube") || other.gameObject.CompareTag("LavaStone"))
        {
            DisplayHitEffect(other.transform.position, controllerManager.RedMagicHitEffectPrefab);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("MovingOrb"))
        {
            if (other.transform.parent != null)
            {
                DisplayHitEffect(other.transform.parent.position, controllerManager.RedMagicHitEffectPrefab); //MovingOrbHitEffect 어디갔을까요 ~
                Destroy(other.transform.parent.gameObject);
            }
        }
    }

    //Eraser 지나갈 때 공격체 없어지는 Effect Diplay Function
    private void DisplayHitEffect(Vector3 position, GameObject hitEffectPrefab)
    {
        GameObject hitEffectInstance = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        hitEffectInstance.SetActive(true);
        Destroy(hitEffectInstance, 3f);
    }

    private IEnumerator DeleteAll()
    {
        Vector3 targetPosition = OriginPosition + new Vector3(-250f, 0f, 0f); // �̵��� ��ǥ ��ġ

        float startTime = Time.time;
        float journeyLength = Vector3.Distance(transform.localPosition, targetPosition);
        float journeyTime = 8f; // �̵��ϴ� �� �ɸ��� �ð� (��)

        while (Time.time - startTime < journeyTime)
        {
            float distanceCovered = (Time.time - startTime) * journeyLength / journeyTime;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, fractionOfJourney);
            yield return null;
        }

        // �̵��� �Ϸ�Ǹ� ���� ��ġ�� ���ư�
        this.transform.localPosition = OriginPosition;
    }

}
