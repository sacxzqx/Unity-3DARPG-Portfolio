using UnityEngine;
using System;

/// <summary>
/// �÷��̾� ���� ���¿� ���� ���� �����͸� �����ϴ� Ŭ����
/// ���� �ӵ� ���� �� �ϵ� ���� ���� ���� ���� ����
/// </summary>
[Serializable]
public class PlayerFallData
{
    [field: SerializeField][field: Range(1f, 30f)] public float FallSpeedLimit { get; private set; } = 15f;
    [field: SerializeField][field: Range(0f, 100f)] public float MinimumDistanceToBeConsideredHardFall { get; private set; } = 3f;
}
