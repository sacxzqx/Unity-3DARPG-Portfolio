using UnityEngine;

/// <summary>
/// 플레이어가 공중(Airborne) 상태일 때 사용하는 상태 클래스
/// 점프, 낙하 등 지면을 떠난 상황을 처리하며, 착지 시 착지 상태로 전환된다
/// </summary>
public class PlayerAirborneState : PlayerMovementState
{
    public PlayerAirborneState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.AirborneParameterHash);

        ResetSprintState();
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.AirborneParameterHash);
    }

    /// <summary>
    /// 지면과 접촉했을 때 호출
    /// 가벼운 착지 상태(LightLandingState)로 전환한다
    /// </summary>
    protected override void OnContactWithGround(Collider collider)
    {
        stateMachine.SetState(stateMachine.LightLandingState);
    }

    protected virtual void ResetSprintState()
    {
        stateMachine.ReusableData.ShouldSprint = false;
    }
}
