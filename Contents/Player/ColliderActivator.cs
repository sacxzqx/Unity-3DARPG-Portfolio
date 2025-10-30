using UnityEngine;

/// <summary>
/// 애니메이션 이벤트를 통해 무기 및 바디 콜라이더를 활성화/비활성화하는 컴포넌트
/// </summary>
public class ColliderActivator : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private Collider bodyCollider; // 킬무브시 무적상태가 되기 위해 사라질 콜라이더

    // 공격 타이밍에 활성화
    public void ActivateWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    // 공격이 끝나는 타이밍에 활성화
    public void DeactivateWeaponCollider()
    {
        weaponCollider.enabled = false;
    }

    public void ActivateBodyCollider()
    {
        bodyCollider.enabled = true;
    }

    public void DeactivateBodyCollider()
    {
        bodyCollider.enabled = false;
    }
}
