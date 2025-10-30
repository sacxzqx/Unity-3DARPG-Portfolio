using UnityEngine;

/// <summary>
/// 플레이어 점프(Jumping) 상태 클래스  
/// 점프 시작 시 힘을 가하고, 수직 속도가 음수가 되면 낙하(Falling) 상태로 전환
/// 지형의 경사도에 따라 점프 힘이 조정
/// </summary>
public class PlayerJumpingState : PlayerAirborneState
{
    private PlayerJumpData jumpData;

    private bool shouldKeepRotating;
    private bool canStartFalling;

    public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        jumpData = airborneData.JumpData;
    }

    public override void ExitState()
    {
        base.ExitState();

        SetBaseRotationData();

        canStartFalling = false;
    }

    public override void EnterState()
    {
        base.EnterState();

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        stateMachine.ReusableData.MovementDecelerationForce = jumpData.DecelerationForce;

        stateMachine.ReusableData.RotationData = jumpData.RotationData;

        shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

        Jump();
    }

    /// <summary>
    /// 수직 속도가 0 이하가 되면 Falling 상태로 전환
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (!canStartFalling && IsMovingUp(0f))
        {
            canStartFalling = true;
        }

        if (!canStartFalling || GetPlayerVerticalVelocity().y > 0)
        {
            return;
        }

        stateMachine.SetState(stateMachine.FallingState);
    }

    /// <summary>
    /// 상승 중일 때 감속하고, 방향 입력이 있으면 회전
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (shouldKeepRotating)
        {
            RotateTowardsTargetRoation();
        }

        if (IsMovingUp())
        {
            DecelerateVertically();
        }
    }

    /// <summary>
    /// 점프 상태에서는 스프린트 유지 여부를 리셋하지 않음 (상태 유지 의도)
    /// </summary>
    protected override void ResetSprintState()
    {
        // Jump 상태에서는 스프린트 플래그를 유지할 수 있음
    }

    /// <summary>
    /// 점프 시 필요한 힘을 계산하고 플레이어에게 적용
    /// 지형의 경사도에 따라 점프 방향 및 세기를 조절
    /// </summary>
    private void Jump()
    {
        Vector3 jumpForce = stateMachine.ReusableData.CurrentJumpForce;

        Vector3 jumpDirection = stateMachine.player.transform.forward;

        if (shouldKeepRotating)
        {
            UpdateTargetRotation(GetMovementInputDirection());

            jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
        }

        jumpForce.x *= jumpDirection.x;
        jumpForce.z *= jumpDirection.z;

        jumpForce = GetJumpForceOnSlope(jumpForce);

        ResetVelocity();

        stateMachine.player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
    }

    /// <summary>
    /// 경사면에서 점프할 때 자연스러운 느낌을 주기 위해 점프 힘을 보정
    /// 플레이어 아래로 레이를 발사하여 경사각을 감지하고,
    /// 오르막에서는 수평 점프 힘을, 내리막에서는 수직 점프 힘을 AnimationCurve 데이터에 따라 조절
    /// </summary>
    /// <param name="jumpForce">보정하기 전의 기본 점프 힘 벡터</param>
    /// <returns>경사면이 적용된 최종 점프 힘 벡터</returns>
    private Vector3 GetJumpForceOnSlope(Vector3 jumpForce)
    {
        Vector3 capsuleColliderCenterInWorldSpace = stateMachine.player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        // 1. 캡슐 콜라이더 중앙에서 아래로 레이를 쏘아 현재 발밑의 경사면 정보를 가져옴
        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, airborneData.JumpData.JumpToGroundRayDistance, stateMachine.player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            // 2. 오르막길을 오르는 중이었다면, 수평 점프 힘을 조절
            // 이를 통해 경사면에 부딪혀 '달라붙는' 느낌을 줄이고, 더 멀리 부드럽게 점프하도록 만듦
            if (IsMovingUp())
            {
                float forceModifier = airborneData.JumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);

                jumpForce.x *= forceModifier;
                jumpForce.z *= forceModifier;
            }

            // 3. 내리막길을 내려가는 중이었다면, 수직 점프 힘을 조절
            // 이를 통해 내리막에서 점프할 때 튀어 오르는 부자연스러운 느낌을 줄여줌
            if (IsMovingDown())
            {
                float forceModifier = airborneData.JumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);

                jumpForce.y *= forceModifier;
            }
        }

        return jumpForce;
    }
}
