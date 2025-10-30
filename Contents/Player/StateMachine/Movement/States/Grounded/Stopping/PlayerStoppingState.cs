using UnityEngine.InputSystem;

/// <summary>
/// Light, Medium, Hard Stopping 상태의 공통 로직을 관리하는 부모 클래스
/// 이 클래스는 직접 상태로 사용되지 않으며, 자식 상태들에게 점진적인 감속 로직과
/// Stopping 애니메이터 파라미터 관리 기능을 제공
/// </summary>
public class PlayerStoppingState : PlayerGroundedState
{
    public PlayerStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.StoppingParameterHash);

        stateMachine.ReusableData.MovementSpeedModifier = 0f;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.StoppingParameterHash);
    }

    /// <summary>
    /// 방향을 목표 각도로 회전시키고 수평 감속을 적용,
    /// 완전히 멈출 때까지 감속을 반복
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        RotateTowardsTargetRoation();

        if (!IsMovingHorizontally())
        {
            return;
        }

        DecelerateHorizontally();
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

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
    }

    private void OnMovementStarted(InputAction.CallbackContext context)
    {
        OnMove();
    }
}
