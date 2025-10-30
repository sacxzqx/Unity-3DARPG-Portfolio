using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾��� �޸���(Running) ���¸� �����ϴ� Ŭ����
/// �� ���¿����� �߰� ����(Medium Jump)�� �����Ǹ�,
/// �ȱ� ��ȯ�̳� ����(Stop) ��ȯ�� ó��
/// ����, ���� ���� ���ο� ���� �ٸ� ���� ���·� ��ȯ�Ǵ� ������ ����
/// </summary>
public class PlayerRunningState : PlayerMovingState
{
    private PlayerSprintData sprintData;

    private float startTime;

    public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        sprintData = movementData.SprintData;
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.RunParameterHash);

        stateMachine.ReusableData.MovementSpeedModifier = movementData.RunData.SpeedModifier;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.MediumForce;

        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.RunParameterHash);
    }

    /// <summary>
    /// ���� �ð��� ������ �ȱ� ����� ���� ������ �ȱ� ���·� ��ȯ
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (!stateMachine.ReusableData.ShouldWalk)
        {
            return;
        }

        if(Time.time < startTime + sprintData.RunToWalkTime)
        {
            return;
        }

        StopRunning();
    }

    /// <summary>
    /// �޸��⸦ �ߴ��ϰ� ���� �Ǵ� �ȱ� ���·� ��ȯ
    /// </summary>
    private void StopRunning()
    {
        if(stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.IdlingState);

            return;
        }

        stateMachine.SetState(stateMachine.WalkingState);
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        stateMachine.player.Input.MovementActions.Movement.canceled += OnMovementCanceled;
    }

    protected override void RemoveinputActionsCallback()
    {
        base.RemoveinputActionsCallback();
        stateMachine.player.Input.MovementActions.Movement.canceled -= OnMovementCanceled;
    }

    /// <summary>
    /// �̵� �Է��� ������ �� ȣ��
    /// �÷��̾��� �׼� ���¿� ���� �ٸ� ���� ���·� ��ȯ
    /// </summary>
    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        // ���� ���� �ƴ� ���� ������ ����� MediumStoppingState�� ��ȯ�Ͽ� �ڿ������� ������ ����
        if (stateMachine.player.ActionStateMachine.CurrentState != stateMachine.player.ActionStateMachine.AttackState)
        {
            stateMachine.SetState(stateMachine.MediumStoppingState);

            return;
        }

        // ���� ���̾��ٸ�, ��� ���� �ൿ���� ������ �� �ֵ��� IdlingState�� ��ȯ
        stateMachine.SetState(stateMachine.IdlingState);
    }

    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);

        stateMachine.SetState(stateMachine.WalkingState);
    }
}
