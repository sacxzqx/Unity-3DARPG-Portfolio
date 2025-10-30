using System;
using UnityEngine;

/// <summary>
/// 플레이어의 바닥 감지를 위한 트리거 콜라이더 데이터를 보관하는 데이터 클래스
/// </summary>
[Serializable]
public class PlayerTriggerColliderData
{
    [field: SerializeField] public BoxCollider GroundCheckCollider { get; private set; }
}
