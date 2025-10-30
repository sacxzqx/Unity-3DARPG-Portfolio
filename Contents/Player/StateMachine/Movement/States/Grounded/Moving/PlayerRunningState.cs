using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 달리기(Running) 상태를 관리하는 클래스
/// 이 상태에서는 중간 점프(Medium Jump)가 설정되며,
/// 걷기 전환이나 멈춤(Stop) 전환을 처리
/// 또한, 공격 상태 여부에 따라 다른 정지 상태로 전환되는 로직을 포함
/// </summary>
public class PlayerRunningState : PlayerMovingState
{
    private PlayerSprintData sprintData;

    private float startTime;

    public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        sprintData = movementData.SprintData;
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.RunParameterHash);

        stateMachine.ReusableData.MovementSpeedModifier = movementData.RunData.SpeedModifier;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;

        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.RunParameterHash);
    }

    /// <summary>
    /// 일정 시간이 지나고 걷기 토글이 켜져 있으면 걷기 상태로 전환
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (!stateMachine.ReusableData.ShouldWalk)
        {
            return;
        }

        if(Time.time < startTime + sprintData.RunToWalkTime)
        {
            return;
        }

        StopRunning();
    }

    /// <summary>
    /// 달리기를 중단하고 정지 또는 걷기 상태로 전환
    /// </summary>
    private void StopRunning()
    {
        if(stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.IdlingState);

            return;
        }

        stateMachine.SetState(stateMachine.WalkingState);
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        stateMachine.player.Input.MovementActions.Movement.canceled += OnMovementCanceled;
    }

    protected override void RemoveinputActionsCallback()
    {
        base.RemoveinputActionsCallback();
        stateMachine.player.Input.MovementActions.Movement.canceled -= OnMovementCanceled;
    }

    /// <summary>
    /// 이동 입력을 멈췄을 때 호출
    /// 플레이어의 액션 상태에 따라 다른 정지 상태로 전환
    /// </summary>
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        // 공격 중이 아닐 때는 관성이 적용된 MediumStoppingState로 전환하여 자연스러운 멈춤을 유도
        if (stateMachine.player.ActionStateMachine.CurrentState != stateMachine.player.ActionStateMachine.AttackState)
        {
            stateMachine.SetState(stateMachine.MediumStoppingState);

            return;
        }

        // 공격 중이었다면, 즉시 다음 행동으로 연계할 수 있도록 IdlingState로 전환
        stateMachine.SetState(stateMachine.IdlingState);
    }

    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);

        stateMachine.SetState(stateMachine.WalkingState);
    }
}
