using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾��� ����(Landing) ���¸� �����ϴ� Ŭ����
/// ���� �ִϸ��̼��� ����ϸ�, �ٸ� �Է� ������ ó������ ����
/// </summary>
public class PlayerLandingState : PlayerGroundedState
{
    public PlayerLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.LandingParameterHash);
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.LandingParameterHash);
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        // ���� ���¿����� ������ �̵� ��� ó�� ���� ����
    }
}
