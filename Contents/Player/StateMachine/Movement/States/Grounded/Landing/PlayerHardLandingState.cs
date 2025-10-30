using System.Diagnostics;
using UnityEngine.InputSystem;
using UnityEngine;

/// <summary>
/// ���ſ� ����(Hard Landing) ���¸� �����ϴ� Ŭ����
/// ���� ������ ���� ��, �Ͻ������� �÷��̾��� �̵� ������ ��Ȱ��ȭ�Ͽ� ���� ����� ���
/// �ִϸ��̼��� ������ Idle ���·� ��ȯ
/// </summary>
public class PlayerHardLandingState : PlayerLandingState
{
    public PlayerHardLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.HardLandParameterHash);

        stateMachine.player.Input.MovementActions.Movement.Disable();

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        ResetVelocity();
    }

    public override void ExitState()
    {
        base.ExitState();
        
        StopAnimation(stateMachine.player.AnimationsData.HardLandParameterHash);

        stateMachine.player.Input.MovementActions.Movement.Enable();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // ���� �� �������� �̲������� ������ �����ϱ� ���� ���� �ӵ��� ��� 0���� ����
        if (!IsMovingHorizontally())
        {
            return;
        }

        ResetVelocity();
    }

    /// <summary>
    /// ���� ���� ��ǲ�� ������ �־��� ���,
    /// Movement.Enable()�� ȣ���Ͽ� ������ �ִ� ��ǲ�� ���� started�� �ߵ��ȴ�
    /// </summary>
    public override void OnAnimationExitEvent()
    {
        stateMachine.player.Input.MovementActions.Movement.Enable();
    }

    public override void OnAnimationTransitionEvent()
    {
        stateMachine.SetState(stateMachine.IdlingState);
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        stateMachine.player.Input.MovementActions.Movement.started += OnMovementStarted;
    }

    protected override void RemoveinputActionsCallback()
    {
        base.RemoveinputActionsCallback();

        stateMachine.player.Input.MovementActions.Movement.started -= OnMovementStarted;
    }

    protected override void OnMove()
    {
        if (stateMachine.ReusableData.ShouldWalk)
        {
            return;
        }

        stateMachine.SetState(stateMachine.RunningState);
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        // ���ſ� ���� ��(ȸ�� ��� ��)���� ���� �Ұ���
    }

    void OnMovementStarted(InputAction.CallbackContext context)
    {
        OnMove();
    }
}
