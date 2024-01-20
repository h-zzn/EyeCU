using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeleteEnemyAttack : MonoBehaviour
{
    private Vector3 OriginPosition;  

    public GameObject target;
    private Vector3 targetPosition;  

    private ControllerManager controllerManager;  

    private Tween moveTween;

    void Awake()
    {
        OriginPosition = this.transform.position;    

        targetPosition = target.transform.position;
    }

    void Start()
    {
        StartCoroutine("DeleteAll"); 

        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();  
    }

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
                DisplayHitEffect(other.transform.parent.position, controllerManager.RedMagicHitEffectPrefab);  
                Destroy(other.transform.parent.gameObject);
            }
        }
    }

    private void DisplayHitEffect(Vector3 position, GameObject hitEffectPrefab)
    {
        GameObject hitEffectInstance = Instantiate(hitEffectPrefab, position, Quaternion.identity);  
        hitEffectInstance.SetActive(true);
        Destroy(hitEffectInstance, 3f);
    }

    IEnumerator DeleteAll()
    {
        float journeyTime = 5f; // 이동 시간 (초)

        // DoTween을 사용하여 움직임을 처리
        moveTween = transform.DOLocalMove(targetPosition, journeyTime).SetEase(Ease.Linear);

        yield return new WaitForSeconds(journeyTime); 

        // 이동이 완료되면 초기 위치로 되돌리기
        transform.position = OriginPosition;  
    }
}
