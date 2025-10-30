using UnityEngine;

/// <summary>
/// �÷��̾ ������ �ִ� Idle ���¸� ����
/// �̵��� ���߰�, ���ڸ� ����(Stationary Jump)�� ���� ���� �̸� ����
/// </summary>

public class PlayerIdlingState : PlayerGroundedState
{
    public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.IdleParameterHash);

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;

        ResetVelocity();
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            return;
        }

        OnMove();
    }
}
