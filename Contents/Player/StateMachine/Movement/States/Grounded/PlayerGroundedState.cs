using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어가 땅 위에 있을 때(Grounded 상태) 사용하는 기본 이동 상태 클래스
/// 점프, 대시, 이동 입력을 처리하고 경사면 이동 보정을 수행
/// </summary>
public class PlayerGroundedState : PlayerMovementState
{
    private SlopeData slopeData;

    public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        slopeData = stateMachine.player.ColliderUtility.SlopeData;
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.GroundedParameterHash);

        UpdateShouldSprintState();
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.GroundedParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Float();
    }

    /// <summary>
    /// 플레이어가 지면에 부드럽게 붙어 있도록 아래 방향으로 힘을 가하고
    /// 이를 통해 작은 턱이나 경사면에서 발이 뜨는 현상을 방지
    /// </summary>
    private void Float()
    {
        Vector3 capsuleColliderCenterInWorldSpace = stateMachine.player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, slopeData.FloatRayDistacne,stateMachine.player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

            if (slopeSpeedModifier == 0f)
            {
                return;
            }

            float distanceToFloatingPoint = stateMachine.player.ColliderUtility.CapsuleColliderData.ColliderCenterInLocalSpace.y * stateMachine.player.transform.localScale.y - hit.distance; 

            if(distanceToFloatingPoint == 0f)
            {
                return;
            }

            // 목표 높이(Floating Point)와 현재 높이의 차이를 기반으로, 목표에 도달하기 위해 필요한 수직 힘을 계산
            float amountToLift = distanceToFloatingPoint * slopeData.StepReachForce - GetPlayerVerticalVelocity().y;

            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

            stateMachine.player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// 현재 경사면 각도에 따라 이동 속도 보정 계수를 설정
    /// </summary>
    /// <param name="angle">지면 법선과의 각도</param>
    /// <returns>적용할 속도 보정 계수</returns>
    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

        stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    /// <summary>
    /// 이동 입력이 멈추면, 다음 이동 시 자동으로 질주하는 상태(ShouldSprint)를 비활성화
    /// </summary>
    private void UpdateShouldSprintState()
    {
        if (!stateMachine.ReusableData.ShouldSprint)
        {
            return;
        }

        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        stateMachine.ReusableData.ShouldSprint = false;
    }

    /// <summary>
    /// 플레이어 바로 아래에 지면이 존재하는지 확인
    /// 울퉁불퉁한 지형을 걸을 때 Fall 상태로 전환되지 않도록 하기 위한 함수
    /// </summary>
    private bool IsThereGroundUnderneath()
    {
        BoxCollider groundCheckCollider = stateMachine.player.ColliderUtility.TriggerColliderData.GroundCheckCollider;

        Vector3 groundColliderCenterInWorldSpace = groundCheckCollider.bounds.center;
        
        Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, groundCheckCollider.bounds.extents, groundCheckCollider.transform.rotation, stateMachine.player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore);

        return overlappedGroundColliders.Length > 0;
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        stateMachine.player.Input.MovementActions.Movement.canceled += OnMovementCanceled;

        stateMachine.player.Input.MovementActions.Dash.started += OnDashStarted;

        stateMachine.player.Input.MovementActions.Jump.started += OnJumpStarted;
    }

    protected override void RemoveinputActionsCallback()
    {
        base.RemoveinputActionsCallback();

        stateMachine.player.Input.MovementActions.Movement.canceled -= OnMovementCanceled;

        stateMachine.player.Input.MovementActions.Dash.started -= OnDashStarted;

        stateMachine.player.Input.MovementActions.Jump.started -= OnJumpStarted;
    }

    /// <summary>
    /// 이동 입력이 들어왔을 때 호출. 현재 스프린트/걷기 여부에 따라 상태를 전이
    /// </summary>
    protected virtual void OnMove()
    {
        if (stateMachine.ReusableData.ShouldSprint)
        {
            stateMachine.SetState(stateMachine.SprintingState);
            return;
        }

        if (stateMachine.ReusableData.ShouldWalk)
        {
            stateMachine.SetState(stateMachine.WalkingState);

            return;
        }

        stateMachine.SetState(stateMachine.RunningState);
    }

    /// <summary>
    /// 콜라이더가 지면(Ground Layer)을 벗어났을 때 호출
    /// 추가로 주변에 다른 지면이 존재하는지 검출하여,
    /// 실제로 공중에 떠 있는 경우 플레이어를 Falling 상태로 전이
    /// </summary>
    /// <param name="collider">지면에서 벗어난 트리거 콜라이더</param>
    protected override void OnContactWithGroundExited(Collider collider)
    {
        base.OnContactWithGroundExited(collider);

        if (IsThereGroundUnderneath())
        {
            return;
        }

        Vector3 capsuleColliderCenterInWorldSpace = stateMachine.player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleBottom = new Ray(capsuleColliderCenterInWorldSpace - stateMachine.player.ColliderUtility.CapsuleColliderData.ColliderVerticalExtents, Vector3.down);

        if (!Physics.Raycast(downwardsRayFromCapsuleBottom, out _, movementData.GroundToFallRayDistance, stateMachine.player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            OnFall();
        }
    }

    protected virtual void OnFall()
    {
        stateMachine.SetState(stateMachine.FallingState);
    }

    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.SetState(stateMachine.IdlingState);
    }

    protected virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        stateMachine.SetState(stateMachine.DashingState);
    }

    protected virtual void OnJumpStarted(InputAction.CallbackContext context)
    {
        stateMachine.SetState(stateMachine.JumpingState);
    }
}
