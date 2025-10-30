using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 걷기(Walk) 상태를 관리하는 클래스
/// 이 상태에서는 가장 약한 점프(Weak Jump)가 설정되며,
/// 걷기 토글을 해제하면 달리기(Running) 상태로 전환
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
