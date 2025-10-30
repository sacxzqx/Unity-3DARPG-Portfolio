using System;
using UnityEngine;

/// <summary>
/// �÷��̾� ���� ���̾� �����͸� �����ϰ�, ���̾� �Ǻ� ����� �����ϴ� ��ƿ��Ƽ Ŭ����
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
