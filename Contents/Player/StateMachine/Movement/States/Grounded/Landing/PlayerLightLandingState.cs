using UnityEngine;

/// <summary>
/// 가벼운 착지(Light Landing) 상태를 관리하는 클래스
/// 점프 후 가볍게 착지하는 상황을 처리하며, 착지 애니메이션 후 Idle 상태로 전환
/// </summary>
public class PlayerLightLandingState : PlayerLandingState
{
    public PlayerLightLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;

        ResetVelocity();
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

    /// <summary>
    /// 착지 후 관성으로 인해 미끄러지는 현상을 방지하기 위해 수평 속도를 강제로 0으로 만듦
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!IsMovingHorizontally())
        {
            return;
        }

        ResetVelocity();
    }

    public override void OnAnimationTransitionEvent()
    {
        stateMachine.SetState(stateMachine.IdlingState);
    }
}
