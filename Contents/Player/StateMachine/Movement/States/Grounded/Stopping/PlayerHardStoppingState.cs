/// <summary>
/// 플레이어가 스프린트 상태(SprintingState)에서 멈출 때 사용하는 HardStopping 상태 클래스
/// 수평 속도를 줄이고, 애니메이션 전이 후 Idle 상태로 전환
/// </summary>
public class PlayerHardStoppingState : PlayerStoppingState
{
    public PlayerHardStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.HardStopParameterHash);

        stateMachine.ReusableData.MovementDecelerationForce = movementData.StopData.HardDecelerationForce;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.HardStopParameterHash);
    }

    /// <summary>
    /// HardStopping 애니메이션 재생 중에 이동 입력을 받으면 호출
    /// 걷기(Walk)모드가 아니라면, 정지 상태를 즉시 중단하고 달리(Running)상태로 전환하여
    /// 플레이어의 입력에 빠르게 반응하도록 함
    /// </summary>
    protected override void OnMove()
    {
        stateMachine.SetState(stateMachine.RunningState);
    }
}
