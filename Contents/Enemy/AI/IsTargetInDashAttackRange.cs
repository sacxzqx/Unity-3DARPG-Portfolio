using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsTargetInDashAttackRange : Conditional
{
    public SharedTransform Target;
    public SharedFloat AttackRange = 2f;
    private Enemy enemy;

    public override void OnStart()
    {
        enemy = GetDefaultGameObject(gameObject).GetComponent<Enemy>();
        Target.Value = enemy.TargetPlayer;
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy.TargetPlayer == null) 
            return TaskStatus.Failure;

        float distance = Vector3.Distance(transform.position, enemy.TargetPlayer.position);
        return distance <= AttackRange.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
}
