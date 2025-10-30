using System;
using UnityEngine;

/// <summary>
/// �÷��̾��� ���� ����(Jump, Fall)�� ���õ� �����͸� ���� �����ϴ� Ŭ����
/// </summary>
[Serializable]
public class PlayerAirborneData
{
    [field: SerializeField] public PlayerJumpData JumpData { get; private set; }
    [field: SerializeField] public PlayerFallData FallData { get; private set; }
}
