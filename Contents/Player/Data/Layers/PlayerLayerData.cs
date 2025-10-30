using System;
using UnityEngine;

/// <summary>
/// 플레이어 관련 레이어 데이터를 관리하고, 레이어 판별 기능을 제공하는 유틸리티 클래스
/// </summary>
[Serializable]
public class PlayerLayerData
{
    [field: SerializeField] public LayerMask GroundLayer { get; private set; }
    [field: SerializeField] public LayerMask EnemyWeapon { get; private set; }

    public bool ContainerLayer(LayerMask layerMask, int layer)
    {
        return (1 << layer & layerMask) != 0;
    }

    public bool IsGroundLayer(int layer)
    {
        return ContainerLayer(GroundLayer, layer);
    }

    public bool IsEnemyLayer(int layer)
    {
        return ContainerLayer(EnemyWeapon, layer);
    }
}
