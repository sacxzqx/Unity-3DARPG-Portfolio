using UnityEngine;

/// <summary>
/// 플레이어가 정지해 있는 Idle 상태를 관리
/// 이동을 멈추고, 제자리 점프(Stationary Jump)에 사용될 힘을 미리 설정
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
