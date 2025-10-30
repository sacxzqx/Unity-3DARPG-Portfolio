using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// 지정된 애니메이션 트리거를 발동시키고 즉시 Success를 반환하는 간단한 액션 노드
/// </summary>
public class StartAnimation : Action
{
    public SharedString TriggerName;
    private Animator anim;

    public override void OnAwake()
    {
        anim = GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (anim == null || string.IsNullOrEmpty(TriggerName.Value))
        {
            return TaskStatus.Failure;
        }

        anim.SetTrigger(TriggerName.Value);
        return TaskStatus.Success;
    }
}