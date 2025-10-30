using UnityEngine;

/// <summary>
/// �÷��̾� ����(Falling) ���� Ŭ����
/// ���� ���� �� ���ϸ� ����ϸ�, ���� �Ÿ� �� �Է� ���¿� ���� ���� �� ���¸� �б�
/// </summary>
public class PlayerFallingState : PlayerAirborneState
{
    private PlayerFallData fallData;
    private Vector3 playerPositionOnEnter;

    public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        fallData = airborneData.FallData;
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.FallParameterHash);

        playerPositionOnEnter = stateMachine.player.transform.position;

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        ResetVerticalVelocity();
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.FallParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        LimitVerticalVelocity();
    }

    protected override void ResetSprintState()
    {
    }

    /// <summary>
    /// ������Ʈ ���� ������ �������̵��Ͽ� ����
    /// ���� �߿��� ������Ʈ �÷��׸� ������ �ʱ�ȭ���� ����
    /// </summary>
    protected override void OnContactWithGround(Collider collider)
    {
        float fallDisatance = Mathf.Abs(playerPositionOnEnter.y - stateMachine.player.transform.position.y);

        if (fallDisatance < fallData.MinimumDistanceToBeConsideredHardFall)
        {
            stateMachine.SetState(stateMachine.LightLandingState); // ª�� ���ϴ� ������ ������

            return;
        }

        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.HardLandingState); // �� ���� + �̵� ���� = �ϵ巣��

            return;
        }

        stateMachine.SetState(stateMachine.RollingState); // �� ���� + �̵� ���� = ������� ����
    }

    /// <summary>
    /// ����� �������� �� ȣ��
    /// ���� �Ÿ��� ���� ���� ���¸� LightLanding, HardLanding, Rolling���� �б�
    /// </summary>
    private void LimitVerticalVelocity()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

        if (playerVerticalVelocity.y >= -fallData.FallSpeedLimit)
        {
            return;
        }

        Vector3 limitedVelocity = new Vector3(0f, -fallData.FallSpeedLimit - playerVerticalVelocity.y, 0f);

        stateMachine.player.Rigidbody.AddForce(limitedVelocity, ForceMode.VelocityChange);
    }
}
