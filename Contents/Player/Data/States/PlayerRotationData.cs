using System;
using UnityEngine;

/// <summary>
/// 플레이어의 목표 회전 도달 시간 설정값을 담는 데이터 클래스
/// </summary>
[Serializable]
public class PlayerRotationData
{
    [field: SerializeField] public Vector3 TargetRotationReachTime { get; private set; }
}
