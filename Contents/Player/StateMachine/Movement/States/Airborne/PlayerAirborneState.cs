using UnityEngine;

/// <summary>
/// �÷��̾ ����(Airborne) ������ �� ����ϴ� ���� Ŭ����
/// ����, ���� �� ������ ���� ��Ȳ�� ó���ϸ�, ���� �� ���� ���·� ��ȯ�ȴ�
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
    /// ����� �������� �� ȣ��
    /// ������ ���� ����(LightLandingState)�� ��ȯ�Ѵ�
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
