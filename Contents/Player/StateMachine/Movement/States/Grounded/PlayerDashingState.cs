using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 대시 상태를 관리하는 클래스
/// 대시 중 속도 및 회전 변경, 연속 대시 횟수 제한 및 쿨다운을 관리
/// 또한, 대시 시작 시의 입력에 따라 방향 전환 가능 여부를 결정하고,
/// 대시 종료 후에는 질주(Sprinting) 또는 급정지(HardStopping) 상태로 자연스럽게 전환
/// </summary>
public class PlayerDashingState : PlayerGroundedState
{
    private PlayerDashData dashData;
    private float startTime;
    private int consecutiveDashesUsed;
    private bool shouldKeepRotating; // 대시 중에 캐릭터가 방향을 틀 수 있는지, 아니면 고정 방향으로만 밀고 나갈지 결정하는 변수

    public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        dashData = new PlayerDashData();
    }

    public override void EnterState()
    {
        base.EnterState();
        
        StartAnimation(stateMachine.player.AnimationsData.DashParameterHash);

        stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;

        stateMachine.ReusableData.RotationData = dashData.RotationData;

        AddForceOnTransitionFromStationaryState();

        // 대시 시작 시점에 이동 입력이 있으면 대시 중 방향 전환을 허용
        shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

        UpdateConsecutiveDashes();

        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.DashParameterHash);

        SetBaseRotationData();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!shouldKeepRotating)
        {
            return;
        }

        RotateTowardsTargetRoation();
    }

    /// <summary>
    /// 대시 동작이 끝날 때, 이동 입력을 유지하면 질주 상태로, 아니면 급정지 상태로 전환
    /// </summary>
    public override void OnAnimationTransitionEvent()
    {
        if(stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.HardStoppingState);

            return;
        }

        stateMachine.SetState(stateMachine.SprintingState);
    }

    /// <summary>
    /// 정지 상태에서 대시할 경우, 캐릭터 정면 방향으로 초기 힘을 가함
    /// </summary>
    private void AddForceOnTransitionFromStationaryState()
    {
        if(stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        Vector3 characterRotationDirection = stateMachine.player.transform.forward;

        characterRotationDirection.y = 0;

        UpdateTargetRotation(characterRotationDirection, false);

        stateMachine.player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed();
    }

    /// <summary>
    /// 연속 대시 사용 횟수를 갱신하고, 제한 횟수 초과 시 쿨다운 부여
    /// </summary>
    private void UpdateConsecutiveDashes()
    {
        if (!IsConsecutive())
        {
            consecutiveDashesUsed = 0;
        }
        
        ++consecutiveDashesUsed;

        if(consecutiveDashesUsed == dashData.ConsecutiveDashesLimitAmount)
        {
            consecutiveDashesUsed = 0;

            stateMachine.player.Input.DisableActionFor(stateMachine.player.Input.MovementActions.Dash, dashData.DashLimitReachedCooldown);
        }
    }

    /// <summary>
    /// 직전 대시와의 시간 차를 기준으로 연속 대시 여부 판별
    /// </summary>
    private bool IsConsecutive()
    {
        return Time.time < startTime + dashData.TimeToBeConsideredConsecutive;
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        stateMachine.player.Input.MovementActions.Movement.performed += OnMovementPerformed;
    }

    protected override void RemoveinputActionsCallback()
    {
        base.RemoveinputActionsCallback();

        stateMachine.player.Input.MovementActions.Movement.performed -= OnMovementPerformed;
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
    }

    protected override void OnDashStarted(InputAction.CallbackContext context)
    {
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        shouldKeepRotating = true;
    }
}
