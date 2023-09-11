using UnityEngine;

public class WeakPoint : MonoBehaviour
{
    public bool isPrimaryController = false; // Primary controller인지 여부
    public Enemy enemy; // 상호작용할 Enemy 스크립트 참조

    private void OnTriggerEnter(Collider other)
    {
       
        
            // PlayerController와 충돌했을 때, OnWeakPointHit 함수를 호출하여 enemy의 hp를 감소시킴
            enemy.OnWeakPointHit(isPrimaryController);
        
    }
}
