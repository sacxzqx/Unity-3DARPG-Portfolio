using UnityEngine;

/// <summary>
/// ��ų ���� �� ���¸� �����ϴ� Ŭ����
/// �θ��� �⺻ ���۰� �޸�, �ǰ� ����(HitState)�� ��ȯ���� �ʰ� �������� ����(���� �Ƹ�)
/// �̸� ���� �÷��̾�� ���� ������ ��Ƽ�鼭 ��ų�� ������ ����� �� ����
/// </summary>
public class PlayerSkillState : PlayerActionState
{
    public PlayerSkillState(PlayerActionStateMachine playerActionStateMachine) : base(playerActionStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if (stateMachine.player.CurrentSkill == null)
        {
            stateMachine.SetState(stateMachine.WeaponDrawnState);
            return;
        }

        StopAnimation(stateMachine.player.AnimationsData.MovingParameterHash); // �ٸ� �̵� �ִϸ��̼��� ��ų �ִϸ��̼��� �������� �ʵ��� ���� ��Ȱ��ȭ
        StartAnimation(stateMachine.player.AnimationsData.SkillParameterHash);
        stateMachine.player.Anim.SetTrigger(stateMachine.player.CurrentSkill.AnimationTrigger);

        stateMachine.player.Input.DisablePlayerActions();

        stateMachine.player.Anim.applyRootMotion = true;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.SkillParameterHash);

        stateMachine.player.Input.EnablePlayerActions();
        stateMachine.player.Anim.applyRootMotion = false;
    }

    protected override void OnHitEnter(Collider collider)
    {
        int damage = CalculateDamage(collider);

        ApplyDamage(damage);
    }

    public override void OnAnimationExitEvent()
    {
        base.OnAnimationExitEvent();

        stateMachine.SetState(stateMachine.WeaponDrawnState);
    }
}
