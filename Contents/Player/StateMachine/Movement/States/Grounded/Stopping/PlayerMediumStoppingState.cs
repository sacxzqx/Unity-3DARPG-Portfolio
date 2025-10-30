/// <summary>
/// �÷��̾ �޸��� ����(RunningState)���� ���� �� ����ϴ� �߰� ����(MediumStopping) ���� Ŭ����
/// ���� �ӵ��� ���̰�, �ִϸ��̼� ���� �� Idle ���·� ��ȯ
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
