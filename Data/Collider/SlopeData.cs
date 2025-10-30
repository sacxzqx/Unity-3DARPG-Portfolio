using System;
using UnityEngine;

/// <summary>
/// ���� �� ���� ������ ���� �������� �����ϴ� ������ Ŭ����  
/// �÷��̾ ������ �ε巴�� �ö󰡰ų� ���� Ÿ���� �� ����ϴ� ���� �Ķ���͸� ����
/// </summary>
[Serializable]
public class SlopeData
{
    /// <summary>
    /// ���� �ݶ��̴� ���̿��� ����̳� ������ �ν��� �� �ִ� �ִ� ����  
    /// ��: 0.25 = Ű�� 25% ���̱����� ������ �ν��ϰ� ���� �� ����
    /// </summary>
    [field: SerializeField][field: Range(0f, 1f)] public float StepHeightPecentage { get; private set; } = 0.25f;

    /// <summary>
    /// ������ �չ������� ������ ���� ������ Ray�� ����  
    /// ���� ������ ���߿� ���ִ��� ���θ� �Ǵ��ϴ� �� ����
    /// </summary>
    [field: SerializeField][field: Range(0f, 5f)] public float FloatRayDistacne { get; private set; } = 2f;

    /// <summary>
    /// ������ �Ѿ �� ���� ������ ����� ���� ũ��  
    /// ���� �ö� �� ����Ǵ� ���޽� (ex: ���� ƨ�ܿø��� ����)
    /// </summary>
    [field: SerializeField][field: Range(0f, 50f)] public float StepReachForce { get; private set; } = 25f;
}
