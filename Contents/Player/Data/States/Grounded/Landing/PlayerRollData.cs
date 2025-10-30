using UnityEngine;
using System;

/// <summary>
/// �÷��̾� ������(Roll) ���� ���� �ӵ� �������� �����ϴ� ������ Ŭ����
/// </summary>
[Serializable]
public class PlayerRollData
{
    [field: SerializeField][field: Range(0f, 3f)] public float SpeedModifier { get; private set; } = 1f;
}
