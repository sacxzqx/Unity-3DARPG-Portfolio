/// <summary>
/// 플레이어가 걷기 상태(WalkingState)에서 멈출 때 사용하는 가벼운 정지(LightStopping) 상태 클래스
/// 수평 속도를 줄이고, 애니메이션 전이 후 Idle 상태로 전환
/// </summary>
public class PlayerLightStoppingState : PlayerStoppingState
{
    public PlayerLightStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        stateMachine.ReusableData.MovementDecelerationForce = movementData.StopData.LightDecelerationForce;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;
    }
}
