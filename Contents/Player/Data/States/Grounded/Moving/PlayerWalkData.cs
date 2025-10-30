using System;
using UnityEngine;

/// <summary>
/// 플레이어의 걷기(Walk) 상태에서 이동 속도 보정을 정의하는 데이터 클래스
/// </summary>
[Serializable]
public class PlayerWalkData
{
    [field: SerializeField][field: Range(0f, 1f)] public float SpeedModifier { get; private set; } = 0.225f;
}
