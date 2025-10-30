using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾� ������(Roll) ���¸� �����ϴ� Ŭ����
/// ���� ���� �����⸦ �����ϸ�, ������ �ִϸ��̼��� ���� �� �̵� �Ǵ� ���� ���·� ��ȯ
/// </summary>
public class PlayerRollingState : PlayerLandingState
{
    private PlayerRollData rollData;

    public PlayerRollingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        rollData = movementData.RollData;
    }
    public override void EnterState()
    {
        base.EnterState();

        stateMachine.ReusableData.MovementSpeedModifier = rollData.SpeedModifier;

        StartAnimation(stateMachine.player.AnimationsData.RollParameterHash);

        // ������ �Ŀ��� �ٷ� ���ַ� �̾����� �ʰ�, �켱 �Ϲ� �޸���� ����ǵ��� �Ͽ� �ڿ������� �������� ����
        stateMachine.ReusableData.ShouldSprint = false;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.RollParameterHash);
    }

    /// <summary>
    /// �̵� �Է��� ������ ��ǥ �������� ȸ��
    /// �̵� �Է��� �ִ� ���: �Է� �������� �ڿ������� ȸ���ϸ� ����
    /// �̵� �Է��� ���� ���: ĳ���Ͱ� ������ ���ϵ��� ȸ��
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        RotateTowardsTargetRoation();
    }

    public override void OnAnimationTransitionEvent()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.MediumStoppingState);
            return;
        }

        OnMove();
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        // ������ �� ���� �Է��� ����
    }
}
