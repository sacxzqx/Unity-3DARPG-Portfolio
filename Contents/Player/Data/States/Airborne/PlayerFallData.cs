using UnityEngine;
using System;

/// <summary>
/// 플레이어 낙하 상태에 대한 설정 데이터를 정의하는 클래스
/// 낙하 속도 제한 및 하드 랜딩 판정 기준 등을 포함
/// </summary>
[Serializable]
public class PlayerFallData
{
    [field: SerializeField][field: Range(1f, 30f)] public float FallSpeedLimit { get; private set; } = 15f;
    [field: SerializeField][field: Range(0f, 100f)] public float MinimumDistanceToBeConsideredHardFall { get; private set; } = 3f;
}
