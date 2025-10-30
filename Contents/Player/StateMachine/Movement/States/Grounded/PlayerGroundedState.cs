using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾ �� ���� ���� ��(Grounded ����) ����ϴ� �⺻ �̵� ���� Ŭ����
/// ����, ���, �̵� �Է��� ó���ϰ� ���� �̵� ������ ����
/// </summary>
public class PlayerGroundedState : PlayerMovementState
{
    private SlopeData slopeData;

    public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        slopeData = stateMachine.player.ColliderUtility.SlopeData;
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.GroundedParameterHash);

        UpdateShouldSprintState();
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.GroundedParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Float();
    }

    /// <summary>
    /// �÷��̾ ���鿡 �ε巴�� �پ� �ֵ��� �Ʒ� �������� ���� ���ϰ�
    /// �̸� ���� ���� ���̳� ���鿡�� ���� �ߴ� ������ ����
    /// </summary>
    private void Float()
    {
        Vector3 capsuleColliderCenterInWorldSpace = stateMachine.player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, slopeData.FloatRayDistacne,stateMachine.player.LayerData.GroundLayer,QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

            if (slopeSpeedModifier == 0f)
            {
                return;
            }

            float distanceToFloatingPoint = stateMachine.player.ColliderUtility.CapsuleColliderData.ColliderCenterInLocalSpace.y * stateMachine.player.transform.localScale.y - hit.distance; 

            if(distanceToFloatingPoint == 0f)
            {
                return;
            }

            // ��ǥ ����(Floating Point)�� ���� ������ ���̸� �������, ��ǥ�� �����ϱ� ���� �ʿ��� ���� ���� ���
            float amountToLift = distanceToFloatingPoint * slopeData.StepReachForce - GetPlayerVerticalVelocity().y;

            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

            stateMachine.player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// ���� ���� ������ ���� �̵� �ӵ� ���� ����� ����
    /// </summary>
    /// <param name="angle">���� �������� ����</param>
    /// <returns>������ �ӵ� ���� ���</returns>
    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

        stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    /// <summary>
    /// �̵� �Է��� ���߸�, ���� �̵� �� �ڵ����� �����ϴ� ����(ShouldSprint)�� ��Ȱ��ȭ
    /// </summary>
    private void UpdateShouldSprintState()
    {
        if (!stateMachine.ReusableData.ShouldSprint)
        {
            return;
        }

        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        stateMachine.ReusableData.ShouldSprint = false;
    }

    /// <summary>
    /// �÷��̾� �ٷ� �Ʒ��� ������ �����ϴ��� Ȯ��
    /// ���������� ������ ���� �� Fall ���·� ��ȯ���� �ʵ��� �ϱ� ���� �Լ�
    /// </summary>
    private bool IsThereGroundUnderneath()
    {
        BoxCollider groundCheckCollider = stateMachine.player.ColliderUtility.TriggerColliderData.GroundCheckCollider;

        Vector3 groundColliderCenterInWorldSpace = groundCheckCollider.bounds.center;
        
        Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, groundCheckCollider.bounds.extents, groundCheckCollider.transform.rotation, stateMachine.player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore);

        return overlappedGroundColliders.Length > 0;
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        stateMachine.player.Input.MovementActions.Movement.canceled += OnMovementCanceled;

        stateMachine.player.Input.MovementActions.Dash.started += OnDashStarted;

        stateMachine.player.Input.MovementActions.Jump.started += OnJumpStarted;
    }

    protected override void RemoveinputActionsCallback()
    {
        base.RemoveinputActionsCallback();

        stateMachine.player.Input.MovementActions.Movement.canceled -= OnMovementCanceled;

        stateMachine.player.Input.MovementActions.Dash.started -= OnDashStarted;

        stateMachine.player.Input.MovementActions.Jump.started -= OnJumpStarted;
    }

    /// <summary>
    /// �̵� �Է��� ������ �� ȣ��. ���� ������Ʈ/�ȱ� ���ο� ���� ���¸� ����
    /// </summary>
    protected virtual void OnMove()
    {
        if (stateMachine.ReusableData.ShouldSprint)
        {
            stateMachine.SetState(stateMachine.SprintingState);
            return;
        }

        if (stateMachine.ReusableData.ShouldWalk)
        {
            stateMachine.SetState(stateMachine.WalkingState);

            return;
        }

        stateMachine.SetState(stateMachine.RunningState);
    }

    /// <summary>
    /// �ݶ��̴��� ����(Ground Layer)�� ����� �� ȣ��
    /// �߰��� �ֺ��� �ٸ� ������ �����ϴ��� �����Ͽ�,
    /// ������ ���߿� �� �ִ� ��� �÷��̾ Falling ���·� ����
    /// </summary>
    /// <param name="collider">���鿡�� ��� Ʈ���� �ݶ��̴�</param>
    protected override void OnContactWithGroundExited(Collider collider)
    {
        base.OnContactWithGroundExited(collider);

        if (IsThereGroundUnderneath())
        {
            return;
        }

        Vector3 capsuleColliderCenterInWorldSpace = stateMachine.player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleBottom = new Ray(capsuleColliderCenterInWorldSpace - stateMachine.player.ColliderUtility.CapsuleColliderData.ColliderVerticalExtents, Vector3.down);

        if (!Physics.Raycast(downwardsRayFromCapsuleBottom, out _, movementData.GroundToFallRayDistance, stateMachine.player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            OnFall();
        }
    }

    protected virtual void OnFall()
    {
        stateMachine.SetState(stateMachine.FallingState);
    }

    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.SetState(stateMachine.IdlingState);
    }

    protected virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        stateMachine.SetState(stateMachine.DashingState);
    }

    protected virtual void OnJumpStarted(InputAction.CallbackContext context)
    {
        stateMachine.SetState(stateMachine.JumpingState);
    }
}
