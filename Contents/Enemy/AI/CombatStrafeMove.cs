using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// 전투 중 플레이어 주변을 좌/우/뒤로 일정 시간 횡이동하는 AI Action 노드
/// 실행될 때마다 내부 확률 값을 0.2 감소
/// </summary>
public class CombatStrafeMove : Action
{
    public enum Direction { Left, Right, Back}

    public SharedFloat StrafeMoveChance; // 해당 노드의 발생 확률 조정값

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

        // 이 노드가 연속적으로 실행되는 것을 방지하기 위해 확률을 감소시킴
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
