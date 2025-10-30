using System.Diagnostics;
using UnityEngine.InputSystem;
using UnityEngine;

/// <summary>
/// 무거운 착지(Hard Landing) 상태를 관리하는 클래스
/// 높은 곳에서 착지 시, 일시적으로 플레이어의 이동 조작을 비활성화하여 복구 모션을 재생
/// 애니메이션이 끝나면 Idle 상태로 전환
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

        // 착지 후 관성으로 미끄러지는 현상을 방지하기 위해 수평 속도를 계속 0으로 만듦
        if (!IsMovingHorizontally())
        {
            return;
        }

        ResetVelocity();
    }

    /// <summary>
    /// 착지 도중 인풋을 누르고 있었던 경우,
    /// Movement.Enable()을 호출하여 누르고 있던 인풋에 대한 started가 발동된다
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
        // 무거운 착지 중(회복 모션 중)에는 점프 불가능
    }

    void OnMovementStarted(InputAction.CallbackContext context)
    {
        OnMove();
    }
}
