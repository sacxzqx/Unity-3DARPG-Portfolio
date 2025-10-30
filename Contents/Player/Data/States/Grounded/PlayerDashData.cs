using System;
using UnityEngine;

/// <summary>
/// 플레이어의 대시(Dash) 동작과 관련된 속도, 회전, 연속 대시 제한 및 쿨타임 설정을 담은 데이터 클래스
/// </summary>
[Serializable]
public class PlayerDashData
{
    [field: SerializeField][field: Range(1f, 3f)] public float SpeedModifier { get; private set; } = 2f;
    [field: SerializeField] public PlayerRotationData RotationData { get; private set; }
    [field: SerializeField][field: Range(0f, 2f)] public float TimeToBeConsideredConsecutive { get; private set; } = 1f;
    [field: SerializeField][field: Range(1, 10)] public int ConsecutiveDashesLimitAmount { get; private set; } = 2;
    [field: SerializeField][field: Range(0f, 5f)] public float DashLimitReachedCooldown { get; private set; } = 1.75f;
}
