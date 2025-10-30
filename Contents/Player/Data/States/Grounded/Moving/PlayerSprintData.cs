using System;
using UnityEngine;

/// <summary>
/// 플레이어의 스프린트(Sprint) 상태에서 속도 보정 및 상태 전환 타이밍을 정의하는 데이터 클래스
/// </summary>
[Serializable]
public class PlayerSprintData
{
    [field: SerializeField][field: Range(1f, 3f)] public float SpeedModifier { get; private set; } = 1.7f;
    [field: SerializeField][field: Range(0f, 5f)] public float SpeedToRunTime { get; private set; } = 1f;
    [field: SerializeField][field: Range(0f, 2f)] public float RunToWalkTime { get; private set; } = 0.5f;
}
