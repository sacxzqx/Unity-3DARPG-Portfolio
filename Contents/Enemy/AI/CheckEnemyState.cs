using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CheckEnemyState : Conditional
{
    public SharedInt TargetState;
    public Enemy Enemy;

    public override void OnAwake()
    {
        Enemy = GetDefaultGameObject(gameObject).GetComponent<Enemy>();
    }

    public override TaskStatus OnUpdate()
    {
        if ((int)Enemy.CurrentState == TargetState.Value) return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}
