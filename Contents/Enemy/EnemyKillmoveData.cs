using UnityEngine;

/// <summary>
/// ų���� ���⿡ �ʿ��� �̵�, ȸ��, �ִϸ��̼� Ʈ���� ������ ��� ScriptableObject
/// ������ �پ��� ų���� ������ ������ �� �ֵ��� �и��� ������ �ڻ�
/// </summary>
[CreateAssetMenu(fileName = "NewKillMoveData", menuName = "KillMove/KillMoveData")]
public class KillMoveData : ScriptableObject
{
    [Header("ų���� �̵� ����")]
    [Tooltip("�÷��̾�� �� ������ �Ÿ�")]
    public float Distance = 1.5f;

    [Tooltip("ų���� ���� ��ġ�� �̵��ϴ� �ð�")]
    public float Duration = 0.3f;

    [Header("ȸ�� ����")]
    [Tooltip("���� �÷��̾ �ٶ󺸵��� 180�� ȸ������ ����")]
    public bool ReverseRotation = true;

    [Header("�ִϸ��̼� ����")]
    [Tooltip("�÷��̾� �� �ִϸ��̼� Ʈ���� �̸�")]
    public string PlayerAnimationTrigger;

    [Tooltip("�� �� �ִϸ��̼� Ʈ���� �̸�")]
    public string EnemyAnimationTrigger;
}
