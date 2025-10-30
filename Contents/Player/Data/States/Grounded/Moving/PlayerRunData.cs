using System;
using UnityEngine;

/// <summary>
/// �÷��̾��� �޸���(Run) ���¿��� �̵� �ӵ� ������ ���� ���� ������
/// </summary>
[Serializable]
public class PlayerRunData
{
    [field: SerializeField][field: Range(1f, 2f)] public float SpeedModifier { get; private set; } = 1f;
}
