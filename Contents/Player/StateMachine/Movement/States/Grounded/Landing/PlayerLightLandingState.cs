using UnityEngine;

/// <summary>
/// ������ ����(Light Landing) ���¸� �����ϴ� Ŭ����
/// ���� �� ������ �����ϴ� ��Ȳ�� ó���ϸ�, ���� �ִϸ��̼� �� Idle ���·� ��ȯ
/// </summary>
public class PlayerLightLandingState : PlayerLandingState
{
    public PlayerLightLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;

        ResetVelocity();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            return;
        }

        OnMove();
    }

    /// <summary>
    /// ���� �� �������� ���� �̲������� ������ �����ϱ� ���� ���� �ӵ��� ������ 0���� ����
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!IsMovingHorizontally())
        {
            return;
        }

        ResetVelocity();
    }

    public override void OnAnimationTransitionEvent()
    {
        stateMachine.SetState(stateMachine.IdlingState);
    }
}
