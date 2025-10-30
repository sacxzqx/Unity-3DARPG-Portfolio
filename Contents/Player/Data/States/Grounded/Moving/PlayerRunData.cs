using System;
using UnityEngine;

/// <summary>
/// 플레이어의 달리기(Run) 상태에서 이동 속도 보정을 위한 설정 데이터
/// </summary>
[Serializable]
public class PlayerRunData
{
    [field: SerializeField][field: Range(1f, 2f)] public float SpeedModifier { get; private set; } = 1f;
}
