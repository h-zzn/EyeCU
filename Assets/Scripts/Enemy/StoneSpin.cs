using UnityEngine;

public class StonSpin : MonoBehaviour
{
    public float rotationSpeed = 60f; // 회전 속도 (1초당 60도 회전)

    // 매 프레임마다 반복하여 오브젝트를 x축 기준으로 회전시킵니다.
    private void Update()
    {
        // x축 방향으로 회전합니다.
        transform.Rotate(Vector3.up  * rotationSpeed * Time.deltaTime);
    }
}
