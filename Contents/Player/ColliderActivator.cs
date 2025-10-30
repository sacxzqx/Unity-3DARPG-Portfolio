using UnityEngine;

/// <summary>
/// �ִϸ��̼� �̺�Ʈ�� ���� ���� �� �ٵ� �ݶ��̴��� Ȱ��ȭ/��Ȱ��ȭ�ϴ� ������Ʈ
/// </summary>
public class ColliderActivator : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private Collider bodyCollider; // ų����� �������°� �Ǳ� ���� ����� �ݶ��̴�

    // ���� Ÿ�ֿ̹� Ȱ��ȭ
    public void ActivateWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    // ������ ������ Ÿ�ֿ̹� Ȱ��ȭ
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
