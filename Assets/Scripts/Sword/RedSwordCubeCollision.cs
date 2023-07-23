using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSwordCubeCollision : MonoBehaviour
{
    public OVRInput.Controller controllerType; // 컨트롤러 종류 선택
    public float vibrationDuration = 1.0f; // 햅틱 지속 시간

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("redCube"))
        {
            // 충돌한 오브젝트가 blueCube 태그를 가지고 있다면 해당 오브젝트를 삭제합니다.
            Destroy(collision.gameObject);

            // Oculus 컨트롤러에서 햅틱 반응을 발생시킵니다.
            OVRInput.SetControllerVibration(1.0f, vibrationDuration,controllerType);
        }
    }

}

