using System;
using UnityEngine;

/// <summary>
/// �÷��̾��� �ٴ� ������ ���� Ʈ���� �ݶ��̴� �����͸� �����ϴ� ������ Ŭ����
/// </summary>
[Serializable]
public class PlayerTriggerColliderData
{
    [field: SerializeField] public BoxCollider GroundCheckCollider { get; private set; }
}
