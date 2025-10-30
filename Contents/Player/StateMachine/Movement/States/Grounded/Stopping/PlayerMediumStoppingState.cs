/// <summary>
/// 플레이어가 달리기 상태(RunningState)에서 멈출 때 사용하는 중간 정지(MediumStopping) 상태 클래스
/// 수평 속도를 줄이고, 애니메이션 전이 후 Idle 상태로 전환
/// </summary>
public class PlayerMediumStoppingState : PlayerStoppingState
{
    public PlayerMediumStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.MediumStopParameterHash);

        stateMachine.ReusableData.MovementDecelerationForce = movementData.StopData.MediumDecelerationForce;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.MediumStopParameterHash);
    }
}
