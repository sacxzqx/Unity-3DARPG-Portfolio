using System;
using UnityEngine;

/// <summary>
/// �÷��̾��� ��ǥ ȸ�� ���� �ð� �������� ��� ������ Ŭ����
/// </summary>
[Serializable]
public class PlayerRotationData
{
    [field: SerializeField] public Vector3 TargetRotationReachTime { get; private set; }
}
