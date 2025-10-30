/// <summary>
/// �÷��̾ ������Ʈ ����(SprintingState)���� ���� �� ����ϴ� HardStopping ���� Ŭ����
/// ���� �ӵ��� ���̰�, �ִϸ��̼� ���� �� Idle ���·� ��ȯ
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
    /// HardStopping �ִϸ��̼� ��� �߿� �̵� �Է��� ������ ȣ��
    /// �ȱ�(Walk)��尡 �ƴ϶��, ���� ���¸� ��� �ߴ��ϰ� �޸�(Running)���·� ��ȯ�Ͽ�
    /// �÷��̾��� �Է¿� ������ �����ϵ��� ��
    /// </summary>
    protected override void OnMove()
    {
        stateMachine.SetState(stateMachine.RunningState);
    }
}
