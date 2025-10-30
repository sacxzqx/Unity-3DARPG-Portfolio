using System;
using UnityEngine;

/// <summary>
/// 플레이어의 점프 동작에 관련된 물리값 및 보간 곡선을 포함한 설정 데이터
/// 회전, 힘, 경사도 보정, 감속값 등을 정의
/// </summary>
[Serializable]
public class PlayerJumpData
{
    [field: SerializeField] public PlayerRotationData RotationData { get; private set; }
    [field: SerializeField][field: Range(0f, 5f)] public float JumpToGroundRayDistance { get; private set; } = 2f;
    [field: SerializeField] public AnimationCurve JumpForceModifierOnSlopeUpwards { get; private set; }
    [field: SerializeField] public AnimationCurve JumpForceModifierOnSlopeDownwards { get; private set; }
    [field: SerializeField] public Vector3 StationaryForce { get; private set; }
    [field: SerializeField] public Vector3 WeakForce { get; private set; }
    [field: SerializeField] public Vector3 MediumForce { get; private set; }
    [field: SerializeField] public Vector3 StrongForce { get; private set; }
    [field: SerializeField][field: Range(0f, 10f)] public float DecelerationForce { get; private set; } = 1.5f; 
}
