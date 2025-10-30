using UnityEngine;

/// <summary>
/// �÷��̾� ����(Jumping) ���� Ŭ����  
/// ���� ���� �� ���� ���ϰ�, ���� �ӵ��� ������ �Ǹ� ����(Falling) ���·� ��ȯ
/// ������ ��絵�� ���� ���� ���� ����
/// </summary>
public class PlayerJumpingState : PlayerAirborneState
{
    private PlayerJumpData jumpData;

    private bool shouldKeepRotating;
    private bool canStartFalling;

    public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        jumpData = airborneData.JumpData;
    }

    public override void ExitState()
    {
        base.ExitState();

        SetBaseRotationData();

        canStartFalling = false;
    }

    public override void EnterState()
    {
        base.EnterState();

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        stateMachine.ReusableData.MovementDecelerationForce = jumpData.DecelerationForce;

        stateMachine.ReusableData.RotationData = jumpData.RotationData;

        shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

        Jump();
    }

    /// <summary>
    /// ���� �ӵ��� 0 ���ϰ� �Ǹ� Falling ���·� ��ȯ
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (!canStartFalling && IsMovingUp(0f))
        {
            canStartFalling = true;
        }

        if (!canStartFalling || GetPlayerVerticalVelocity().y > 0)
        {
            return;
        }

        stateMachine.SetState(stateMachine.FallingState);
    }

    /// <summary>
    /// ��� ���� �� �����ϰ�, ���� �Է��� ������ ȸ��
    /// </summary>
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (shouldKeepRotating)
        {
            RotateTowardsTargetRoation();
        }

        if (IsMovingUp())
        {
            DecelerateVertically();
        }
    }

    /// <summary>
    /// ���� ���¿����� ������Ʈ ���� ���θ� �������� ���� (���� ���� �ǵ�)
    /// </summary>
    protected override void ResetSprintState()
    {
        // Jump ���¿����� ������Ʈ �÷��׸� ������ �� ����
    }

    /// <summary>
    /// ���� �� �ʿ��� ���� ����ϰ� �÷��̾�� ����
    /// ������ ��絵�� ���� ���� ���� �� ���⸦ ����
    /// </summary>
    private void Jump()
    {
        Vector3 jumpForce = stateMachine.ReusableData.CurrentJumpForce;

        Vector3 jumpDirection = stateMachine.player.transform.forward;

        if (shouldKeepRotating)
        {
            UpdateTargetRotation(GetMovementInputDirection());

            jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
        }

        jumpForce.x *= jumpDirection.x;
        jumpForce.z *= jumpDirection.z;

        jumpForce = GetJumpForceOnSlope(jumpForce);

        ResetVelocity();

        stateMachine.player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
    }

    /// <summary>
    /// ���鿡�� ������ �� �ڿ������� ������ �ֱ� ���� ���� ���� ����
    /// �÷��̾� �Ʒ��� ���̸� �߻��Ͽ� ��簢�� �����ϰ�,
    /// ������������ ���� ���� ����, ������������ ���� ���� ���� AnimationCurve �����Ϳ� ���� ����
    /// </summary>
    /// <param name="jumpForce">�����ϱ� ���� �⺻ ���� �� ����</param>
    /// <returns>������ ����� ���� ���� �� ����</returns>
    private Vector3 GetJumpForceOnSlope(Vector3 jumpForce)
    {
        Vector3 capsuleColliderCenterInWorldSpace = stateMachine.player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        // 1. ĸ�� �ݶ��̴� �߾ӿ��� �Ʒ��� ���̸� ��� ���� �߹��� ���� ������ ������
        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, airborneData.JumpData.JumpToGroundRayDistance, stateMachine.player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            // 2. ���������� ������ ���̾��ٸ�, ���� ���� ���� ����
            // �̸� ���� ���鿡 �ε��� '�޶�ٴ�' ������ ���̰�, �� �ָ� �ε巴�� �����ϵ��� ����
            if (IsMovingUp())
            {
                float forceModifier = airborneData.JumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);

                jumpForce.x *= forceModifier;
                jumpForce.z *= forceModifier;
            }

            // 3. ���������� �������� ���̾��ٸ�, ���� ���� ���� ����
            // �̸� ���� ���������� ������ �� Ƣ�� ������ ���ڿ������� ������ �ٿ���
            if (IsMovingDown())
            {
                float forceModifier = airborneData.JumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);

                jumpForce.y *= forceModifier;
            }
        }

        return jumpForce;
    }
}
