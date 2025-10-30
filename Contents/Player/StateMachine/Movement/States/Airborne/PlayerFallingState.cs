using UnityEngine;

/// <summary>
/// 플레이어 낙하(Falling) 상태 클래스
/// 공중 상태 중 낙하를 담당하며, 낙하 거리 및 입력 상태에 따라 착지 후 상태를 분기
/// </summary>
public class PlayerFallingState : PlayerAirborneState
{
    private PlayerFallData fallData;
    private Vector3 playerPositionOnEnter;

    public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        fallData = airborneData.FallData;
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.FallParameterHash);

        playerPositionOnEnter = stateMachine.player.transform.position;

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        ResetVerticalVelocity();
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.FallParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        LimitVerticalVelocity();
    }

    protected override void ResetSprintState()
    {
    }

    /// <summary>
    /// 스프린트 상태 리셋을 오버라이드하여 무시
    /// 낙하 중에는 스프린트 플래그를 별도로 초기화하지 않음
    /// </summary>
    protected override void OnContactWithGround(Collider collider)
    {
        float fallDisatance = Mathf.Abs(playerPositionOnEnter.y - stateMachine.player.transform.position.y);

        if (fallDisatance < fallData.MinimumDistanceToBeConsideredHardFall)
        {
            stateMachine.SetState(stateMachine.LightLandingState); // 짧은 낙하는 가벼운 착지로

            return;
        }

        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.HardLandingState); // 긴 낙하 + 이동 없음 = 하드랜딩

            return;
        }

        stateMachine.SetState(stateMachine.RollingState); // 긴 낙하 + 이동 있음 = 구르기로 착지
    }

    /// <summary>
    /// 지면과 접촉했을 때 호출
    /// 낙하 거리에 따라 착지 상태를 LightLanding, HardLanding, Rolling으로 분기
    /// </summary>
    private void LimitVerticalVelocity()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

        if (playerVerticalVelocity.y >= -fallData.FallSpeedLimit)
        {
            return;
        }

        Vector3 limitedVelocity = new Vector3(0f, -fallData.FallSpeedLimit - playerVerticalVelocity.y, 0f);

        stateMachine.player.Rigidbody.AddForce(limitedVelocity, ForceMode.VelocityChange);
    }
}
