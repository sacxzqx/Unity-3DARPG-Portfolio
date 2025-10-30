using System;
using UnityEngine;

/// <summary>
/// 플레이어의 지면(Grounded) 상태에서의 이동 속도, 회전, 경사 보정 및 하위상태(Walk, Run 등) 데이터를 포함하는 클래스
/// </summary>

[Serializable]
public class PlayerGroundedData
{
    [field: SerializeField][field: Range(0f, 25f)] public float BaseSpeed { get; private set; } = 5f;
    [field: SerializeField][field: Range(0f, 5f)] public float GroundToFallRayDistance { get; private set; } = 1f;
    [field: SerializeField] public AnimationCurve SlopeSpeedAngles { get; private set; }
    [field: SerializeField] public PlayerRotationData BaseRotationData { get; private set; }
    [field: SerializeField] public PlayerWalkData WalkData { get; private set; }
    [field: SerializeField] public PlayerRunData RunData { get; private set; }
    [field: SerializeField] public PlayerSprintData SprintData { get; private set; }
    [field: SerializeField] public PlayerDashData DashData { get; private set; }
    [field: SerializeField] public PlayerStopData StopData { get; private set; }
    [field: SerializeField] public PlayerRollData RollData { get; private set; }
}
