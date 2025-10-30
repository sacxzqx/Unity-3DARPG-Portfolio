using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// ������ �ִϸ��̼� Ʈ���Ÿ� �ߵ���Ű�� ��� Success�� ��ȯ�ϴ� ������ �׼� ���
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