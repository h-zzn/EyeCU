using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyHp = 1000; // Enemy의 초기 hp

    // 이 함수는 WeakPoint가 EYE RAY로 hover 되었을 때 호출됩니다.
    public void OnWeakPointHit(bool isPrimaryController)
    {
        if (isPrimaryController)
        {
            // Primary controller trigger를 눌렀을 때
            enemyHp -= 100;
        }
        else
        {
            // Secondary controller trigger를 눌렀을 때
            enemyHp -= 100;
        }

        // Enemy의 hp가 0 이하로 떨어지면 파괴
        if (enemyHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
