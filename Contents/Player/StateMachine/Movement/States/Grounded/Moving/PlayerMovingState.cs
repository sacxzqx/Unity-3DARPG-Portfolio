/// <summary>
/// �ȱ�(Walking), �޸���(Running), ����(Sprinting) ������ ���� ������ �����ϴ� �θ� Ŭ����
/// �� Ŭ������ ���� ���·� ������ ������, �ڽ� ���µ鿡�� Moving �ִϸ����� �Ķ���͸�
/// �Ѱ� ���� ���� ����� �����Ͽ� �ڵ� �ߺ��� ����
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
