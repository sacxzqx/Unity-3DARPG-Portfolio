using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾��� �ȱ�(Walk) ���¸� �����ϴ� Ŭ����
/// �� ���¿����� ���� ���� ����(Weak Jump)�� �����Ǹ�,
/// �ȱ� ����� �����ϸ� �޸���(Running) ���·� ��ȯ
/// </summary>
public class PlayerWalkingState : PlayerMovingState
{
    public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.WalkParameterHash);

        stateMachine.ReusableData.MovementSpeedModifier = movementData.WalkData.SpeedModifier;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.WalkParameterHash);
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        if (stateMachine.player.ActionStateMachine.CurrentState != stateMachine.player.ActionStateMachine.AttackState)
        {
            stateMachine.SetState(stateMachine.LightStoppingState);
        }
    }
    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);

        stateMachine.SetState(stateMachine.RunningState);
    }
}
