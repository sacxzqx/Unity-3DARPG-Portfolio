using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 전력질주(Sprint) 상태를 관리하는 클래스
/// 이 상태에서는 가장 강한 점프(Strong Jump)를 준비하며, 
/// 점프나 낙하 시에도 질주 상태를 기억하여 착지 후 자연스럽게 달리기를 이어갈 수 있음
/// 스프린트 입력이 중단되면, 짧은 지연 후 일반 달리기(Running) 상태로 전환
/// </summary>
public class PlayerSprintState : PlayerMovingState
{
    private PlayerSprintData sprintData;

    private float startTime;

    private bool keepSprinting;
    private bool shouldResetSprintingState;

    public PlayerSprintState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        sprintData = movementData.SprintData;
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.SprintParameterHash);

        stateMachine.ReusableData.MovementSpeedModifier = sprintData.SpeedModifier;
        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

        shouldResetSprintingState = true;

        if (!stateMachine.ReusableData.ShouldSprint)
        {
            keepSprinting = false;
        }

        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.SprintParameterHash);

        if (shouldResetSprintingState)
        {
            keepSprinting = false;

            stateMachine.ReusableData.ShouldSprint = false;
        }
    }

    /// <summary>
    /// 매 프레임 스프린트 입력 유지 여부를 검사하여 런(Run) 상태로 전환
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // 스프린트 키를 계속 누르고 있는 경우 상태를 유지
        if (keepSprinting)
        {
            return;
        }

        // 스프린트 키를 뗀 경우, 바로 Run상태(혹은 Idle상태)로 전환하지 않고 약간의 유예 시간을 줌
        if (Time.time < startTime + sprintData.SpeedToRunTime)
        {
            return;
        }

        StopSprinting();
    }

    /// <summary>
    /// 스프린트를 중단하고 정지하거나 런(Run) 상태로 전환
    /// </summary>
    private void StopSprinting()
    {
        if(stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.IdlingState);

            return;
        }

        stateMachine.SetState(stateMachine.RunningState);
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        stateMachine.player.Input.MovementActions.Sprint.performed += OnSprintPerformed;
    }

    protected override void RemoveinputActionsCallback()
    {
        base.RemoveinputActionsCallback();

        stateMachine.player.Input.MovementActions.Sprint.performed -= OnSprintPerformed;
    }

    /// <summary>
    /// 낙하 전환 시 스프린트 상태를 초기화하지 않도록 설정
    /// </summary>
    protected override void OnFall()
    {
        shouldResetSprintingState = false;

        base.OnFall();
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        if(stateMachine.player.ActionStateMachine.CurrentState != stateMachine.player.ActionStateMachine.AttackState)
        {
            stateMachine.SetState(stateMachine.HardStoppingState);

            return;
        }

        stateMachine.SetState(stateMachine.IdlingState);
    }

    /// <summary>
    /// 질주 중 점프를 할 때, 착지 후에도 질주 상태를 유지할 수 있도록 
    /// 현재 질주 의도를 기억하도록 함
    /// </summary>
    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        shouldResetSprintingState = false;

        base.OnJumpStarted(context);
    }

    /// <summary>
    /// 스프린트 입력을 새로 수행할 경우 keepSprinting 활성화
    /// </summary>
    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        keepSprinting = true;

        stateMachine.ReusableData.ShouldSprint = true;
    }
}
