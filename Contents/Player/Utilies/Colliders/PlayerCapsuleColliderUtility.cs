using UnityEngine;
using System;

/// <summary>
/// �÷��̾� ���� ĸ�� �ݶ��̴� ��ƿ��Ƽ
/// Ʈ���� �ݶ��̴� �����͸� �����Ͽ� �÷��̾��� �浹 ���� ������ ����
/// </summary>
[Serializable]
public class PlayerCapsuleColliderUtility : CapsuleColliderUtility
{
    [field: SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }
}
