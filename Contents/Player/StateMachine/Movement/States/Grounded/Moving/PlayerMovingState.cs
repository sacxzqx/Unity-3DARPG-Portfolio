/// <summary>
/// 걷기(Walking), 달리기(Running), 질주(Sprinting) 상태의 공통 로직을 관리하는 부모 클래스
/// 이 클래스는 직접 상태로 사용되지 않으며, 자식 상태들에게 Moving 애니메이터 파라미터를
/// 켜고 끄는 공통 기능을 제공하여 코드 중복을 방지
/// </summary>
public class PlayerMovingState : PlayerGroundedState
{
    public PlayerMovingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.MovingParameterHash);
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.MovingParameterHash);
    }
}
