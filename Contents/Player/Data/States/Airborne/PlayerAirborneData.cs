using System;
using UnityEngine;

/// <summary>
/// 플레이어의 공중 상태(Jump, Fall)에 관련된 데이터를 묶어 관리하는 클래스
/// </summary>
[Serializable]
public class PlayerAirborneData
{
    [field: SerializeField] public PlayerJumpData JumpData { get; private set; }
    [field: SerializeField] public PlayerFallData FallData { get; private set; }
}
