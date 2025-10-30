/// <summary>
/// �÷��̾ �ȱ� ����(WalkingState)���� ���� �� ����ϴ� ������ ����(LightStopping) ���� Ŭ����
/// ���� �ӵ��� ���̰�, �ִϸ��̼� ���� �� Idle ���·� ��ȯ
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
