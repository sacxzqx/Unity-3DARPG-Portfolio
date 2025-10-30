using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾��� ��� ���¸� �����ϴ� Ŭ����
/// ��� �� �ӵ� �� ȸ�� ����, ���� ��� Ƚ�� ���� �� ��ٿ��� ����
/// ����, ��� ���� ���� �Է¿� ���� ���� ��ȯ ���� ���θ� �����ϰ�,
/// ��� ���� �Ŀ��� ����(Sprinting) �Ǵ� ������(HardStopping) ���·� �ڿ������� ��ȯ
/// </summary>
public class PlayerDashingState : PlayerGroundedState
{
    private PlayerDashData dashData;
    private float startTime;
    private int consecutiveDashesUsed;
    private bool shouldKeepRotating; // ��� �߿� ĳ���Ͱ� ������ Ʋ �� �ִ���, �ƴϸ� ���� �������θ� �а� ������ �����ϴ� ����

    public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        dashData = new PlayerDashData();
    }

    public override void EnterState()
    {
        base.EnterState();
        
        StartAnimation(stateMachine.player.AnimationsData.DashParameterHash);

        stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;

        stateMachine.ReusableData.RotationData = dashData.RotationData;

        AddForceOnTransitionFromStationaryState();

        // ��� ���� ������ �̵� �Է��� ������ ��� �� ���� ��ȯ�� ���
        shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

        UpdateConsecutiveDashes();

        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.DashParameterHash);

        SetBaseRotationData();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!shouldKeepRotating)
        {
            return;
        }

        RotateTowardsTargetRoation();
    }

    /// <summary>
    /// ��� ������ ���� ��, �̵� �Է��� �����ϸ� ���� ���·�, �ƴϸ� ������ ���·� ��ȯ
    /// </summary>
    public override void OnAnimationTransitionEvent()
    {
        if(stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.HardStoppingState);

            return;
        }

        stateMachine.SetState(stateMachine.SprintingState);
    }

    /// <summary>
    /// ���� ���¿��� ����� ���, ĳ���� ���� �������� �ʱ� ���� ����
    /// </summary>
    private void AddForceOnTransitionFromStationaryState()
    {
        if(stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        Vector3 characterRotationDirection = stateMachine.player.transform.forward;

        characterRotationDirection.y = 0;

        UpdateTargetRotation(characterRotationDirection, false);

        stateMachine.player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed();
    }

    /// <summary>
    /// ���� ��� ��� Ƚ���� �����ϰ�, ���� Ƚ�� �ʰ� �� ��ٿ� �ο�
    /// </summary>
    private void UpdateConsecutiveDashes()
    {
        if (!IsConsecutive())
        {
            consecutiveDashesUsed = 0;
        }
        
        ++consecutiveDashesUsed;

        if(consecutiveDashesUsed == dashData.ConsecutiveDashesLimitAmount)
        {
            consecutiveDashesUsed = 0;

            stateMachine.player.Input.DisableActionFor(stateMachine.player.Input.MovementActions.Dash, dashData.DashLimitReachedCooldown);
        }
    }

    /// <summary>
    /// ���� ��ÿ��� �ð� ���� �������� ���� ��� ���� �Ǻ�
    /// </summary>
    private bool IsConsecutive()
    {
        return Time.time < startTime + dashData.TimeToBeConsideredConsecutive;
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        stateMachine.player.Input.MovementActions.Movement.performed += OnMovementPerformed;
    }

    protected override void RemoveinputActionsCallback()
    {
        base.RemoveinputActionsCallback();

        stateMachine.player.Input.MovementActions.Movement.performed -= OnMovementPerformed;
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
    }

    protected override void OnDashStarted(InputAction.CallbackContext context)
    {
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        shouldKeepRotating = true;
    }
}
