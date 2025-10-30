using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾ �ǰݴ����� ���� ���¸� ����
/// �� ���¿� �����ϸ� ª�� ���� �ð��� �ο��Ǹ�,
/// ���� �ð��� ���� �Ŀ��� �ִϸ��̼��� ������ ���̶� �ٽ� �ǰݵ� �� ����
/// �ǰ� �ִϸ��̼��� ���� ���� ���ο� ���� �ٸ��� ���
/// </summary>
public class PlayerHitState : PlayerActionState
{
    private bool isInvincible = false;

    public PlayerHitState(PlayerActionStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void EnterState()
    {
        // ���� ���� ���ο� ���� �ٸ� �ǰ� �ִϸ��̼� ���
        if (stateMachine.ReusableData.IsWeaponEquipped)
        {
            StartAnimation(stateMachine.player.AnimationsData.HitOnBattleParameterHash);
        }
        else
        {
            StartAnimation(stateMachine.player.AnimationsData.HitOnSheathedParameterHash);
        }

        stateMachine.ReusableData.CanMove = false;

        stateMachine.player.Rigidbody.velocity = Vector3.zero;

        isInvincible = true;
    }

    public override void ExitState()
    {
        isInvincible = false;

        stateMachine.ReusableData.CanMove = true;
    }

    /// <summary>
    /// �ǰ� �ִϸ��̼��� '����' ������ ������ ������ �ִϸ��̼� �̺�Ʈ�� ���� ȣ��
    /// �� �������� �÷��̾�� �ִϸ��̼��� ������ ������ ���̶� �ٽ� �ǰݵ� �� ����
    /// </summary>
    public override void OnAnimationTransitionEvent()
    {
        isInvincible = false;
    }

    /// <summary>
    /// �ǰ� �ִϸ��̼��� ������ ������ �� �ִϸ��̼� �̺�Ʈ�� ���� ȣ��
    /// ������ ���� ���¿� ���� ������ �⺻ ���·� ����
    /// </summary>
    public override void OnAnimationExitEvent()
    {
        if (stateMachine.ReusableData.IsWeaponEquipped)
        {
            stateMachine.SetState(stateMachine.WeaponDrawnState);
        }
        else
        {
            stateMachine.SetState(stateMachine.SheathedState);
        }
    }

    protected override void OnHitEnter(Collider collider)
    {
        if (isInvincible)
        {
            return;
        }

        base.OnHitEnter(collider);
    }

    public override void OnAttackPerformed(InputAction.CallbackContext context)
    {
    }

    public override void OnDefendPerformed(InputAction.CallbackContext context)
    {
    }

    public override void OnDefendCanceled(InputAction.CallbackContext context)
    {
    }

    public override void OnSkillPerformed(InputAction.CallbackContext context)
    {
    }
}
