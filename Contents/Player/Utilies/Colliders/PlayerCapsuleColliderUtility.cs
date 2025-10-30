using UnityEngine;
using System;

/// <summary>
/// 플레이어 전용 캡슐 콜라이더 유틸리티
/// 트리거 콜라이더 데이터를 포함하여 플레이어의 충돌 관련 정보를 보조
/// </summary>
[Serializable]
public class PlayerCapsuleColliderUtility : CapsuleColliderUtility
{
    [field: SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }
}
