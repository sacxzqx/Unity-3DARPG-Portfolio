using UnityEngine;
using System;

/// <summary>
/// 플레이어 구르기(Roll) 동작 시의 속도 보정값을 설정하는 데이터 클래스
/// </summary>
[Serializable]
public class PlayerRollData
{
    [field: SerializeField][field: Range(0f, 3f)] public float SpeedModifier { get; private set; } = 1f;
}
