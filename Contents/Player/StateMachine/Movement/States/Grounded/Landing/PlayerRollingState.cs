using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 구르기(Roll) 상태를 관리하는 클래스
/// 착지 직후 구르기를 수행하며, 구르기 애니메이션이 끝난 후 이동 또는 정지 상태로 전환
/// </summary>
public class PlayerRollingState : PlayerLandingState
{
    private PlayerRollData rollData;

    public PlayerRollingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        rollData = movementData.RollData;
    }
    public override void EnterState()
    {
        base.EnterState();

        stateMachine.ReusableData.MovementSpeedModifier = rollData.SpeedModifier;

        StartAnimation(stateMachine.player.AnimationsData.RollParameterHash);

        // 구르기 후에는 바로 질주로 이어지지 않고, 우선 일반 달리기로 연결되도록 하여 자연스러운 움직임을 유도
        stateMachine.ReusableData.ShouldSprint = false;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.RollParameterHash);
    }

    /// <summary>
    /// 이동 입력이 없으면 목표 방향으로 회전
    /// 이동 입력이 있는 경우: 입력 방향으로 자연스럽게 회전하며 구름
    /// 이동 입력이 없는 경우: 캐릭터가 정면을 향하도록 회전
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        RotateTowardsTargetRoation();
    }

    public override void OnAnimationTransitionEvent()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.MediumStoppingState);
            return;
        }

        OnMove();
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        // 구르기 중 점프 입력은 무시
    }
}
