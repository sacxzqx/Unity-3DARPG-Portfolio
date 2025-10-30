using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// ���� �� �÷��̾� �ֺ��� ��/��/�ڷ� ���� �ð� Ⱦ�̵��ϴ� AI Action ���
/// ����� ������ ���� Ȯ�� ���� 0.2 ����
/// </summary>
public class CombatStrafeMove : Action
{
    public enum Direction { Left, Right, Back}

    public SharedFloat StrafeMoveChance; // �ش� ����� �߻� Ȯ�� ������

    public Direction MoveDirection = Direction.Left;
    private float duration = 2.0f;
    private Animator anim;
    private float timer;

    public override void OnStart()
    {
        anim = GetComponent<Animator>();
        timer = 0f;

        MoveDirection = (Direction)Random.Range(0, 3);

        switch (MoveDirection)
        {
            case Direction.Left:
                anim.SetFloat("MoveX", -1f);
                break;
            case Direction.Right:
                anim.SetFloat("MoveX", 1f);
                break;
            case Direction.Back:
                anim.SetFloat("MoveY", -1f);
                break;
        }

        // �� ��尡 ���������� ����Ǵ� ���� �����ϱ� ���� Ȯ���� ���ҽ�Ŵ
        StrafeMoveChance.Value -= 0.2f;
    }

    public override TaskStatus OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer > duration)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        timer = 0f;
        anim.SetFloat("MoveX", 0f);
        anim.SetFloat("MoveY", 0f);
    }
}
