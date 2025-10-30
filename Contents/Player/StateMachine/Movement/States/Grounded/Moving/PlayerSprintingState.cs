using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾��� ��������(Sprint) ���¸� �����ϴ� Ŭ����
/// �� ���¿����� ���� ���� ����(Strong Jump)�� �غ��ϸ�, 
/// ������ ���� �ÿ��� ���� ���¸� ����Ͽ� ���� �� �ڿ������� �޸��⸦ �̾ �� ����
/// ������Ʈ �Է��� �ߴܵǸ�, ª�� ���� �� �Ϲ� �޸���(Running) ���·� ��ȯ
/// </summary>
public class PlayerSprintState : PlayerMovingState
{
    private PlayerSprintData sprintData;

    private float startTime;

    private bool keepSprinting;
    private bool shouldResetSprintingState;

    public PlayerSprintState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        sprintData = movementData.SprintData;
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.SprintParameterHash);

        stateMachine.ReusableData.MovementSpeedModifier = sprintData.SpeedModifier;
        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

        shouldResetSprintingState = true;

        if (!stateMachine.ReusableData.ShouldSprint)
        {
            keepSprinting = false;
        }

        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.SprintParameterHash);

        if (shouldResetSprintingState)
        {
            keepSprinting = false;

            stateMachine.ReusableData.ShouldSprint = false;
        }
    }

    /// <summary>
    /// �� ������ ������Ʈ �Է� ���� ���θ� �˻��Ͽ� ��(Run) ���·� ��ȯ
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // ������Ʈ Ű�� ��� ������ �ִ� ��� ���¸� ����
        if (keepSprinting)
        {
            return;
        }

        // ������Ʈ Ű�� �� ���, �ٷ� Run����(Ȥ�� Idle����)�� ��ȯ���� �ʰ� �ణ�� ���� �ð��� ��
        if (Time.time < startTime + sprintData.SpeedToRunTime)
        {
            return;
        }

        StopSprinting();
    }

    /// <summary>
    /// ������Ʈ�� �ߴ��ϰ� �����ϰų� ��(Run) ���·� ��ȯ
    /// </summary>
    private void StopSprinting()
    {
        if(stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.SetState(stateMachine.IdlingState);

            return;
        }

        stateMachine.SetState(stateMachine.RunningState);
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        stateMachine.player.Input.MovementActions.Sprint.performed += OnSprintPerformed;
    }

    protected override void RemoveinputActionsCallback()
    {
        base.RemoveinputActionsCallback();

        stateMachine.player.Input.MovementActions.Sprint.performed -= OnSprintPerformed;
    }

    /// <summary>
    /// ���� ��ȯ �� ������Ʈ ���¸� �ʱ�ȭ���� �ʵ��� ����
    /// </summary>
    protected override void OnFall()
    {
        shouldResetSprintingState = false;

        base.OnFall();
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        if(stateMachine.player.ActionStateMachine.CurrentState != stateMachine.player.ActionStateMachine.AttackState)
        {
            stateMachine.SetState(stateMachine.HardStoppingState);

            return;
        }

        stateMachine.SetState(stateMachine.IdlingState);
    }

    /// <summary>
    /// ���� �� ������ �� ��, ���� �Ŀ��� ���� ���¸� ������ �� �ֵ��� 
    /// ���� ���� �ǵ��� ����ϵ��� ��
    /// </summary>
    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        shouldResetSprintingState = false;

        base.OnJumpStarted(context);
    }

    /// <summary>
    /// ������Ʈ �Է��� ���� ������ ��� keepSprinting Ȱ��ȭ
    /// </summary>
    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        keepSprinting = true;

        stateMachine.ReusableData.ShouldSprint = true;
    }
}
