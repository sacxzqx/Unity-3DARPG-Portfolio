using UnityEngine;

/// <summary>
/// �÷��̾��� ���� ���¸� �����ϴ� Ŭ����
/// ���� �߿��� �̵��� ���ѵǰ� ��Ʈ ����� ���� �����̵��� ��
/// </summary>
public class PlayerAttackState : PlayerActionState
{
    public PlayerAttackState(PlayerActionStateMachine playerActionStateMachine) : base(playerActionStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.AttackParameterHash);

        stateMachine.ReusableData.CanMove = false;
        stateMachine.player.Rigidbody.velocity = Vector3.zero;
        stateMachine.player.InputBuffer.ClearLastInput();
        stateMachine.player.Anim.applyRootMotion = true;
    }

    public override void ExitState()
    {
        base.ExitState();

        stateMachine.player.Anim.ResetTrigger(stateMachine.player.AnimationsData.AttackParameterHash);

        stateMachine.ReusableData.CanMove = true;
        stateMachine.ReusableData.Direction = Vector3.zero;
        stateMachine.player.Anim.applyRootMotion = false;
        stateMachine.player.WeaponCollider.enabled = false;
    }

    public override void Update()
    {
        // ���� �ִϸ��̼��� ����Ǵ� ����, ���� ������ ���� ������
        // �÷��̾��� ���� �̵� �Է¿� ���� ��� ����
        ReadMovementInput();
    }

    public override void PhysicsUpdate()
    {
        RotatePlayer(); // ����Ű�� �Է��� �������� ĳ���͸� ȸ��
    }

    private void RotatePlayer()
    {
        Vector3 movementDirection = GetMovementInputDirection();

        stateMachine.ReusableData.Direction = movementDirection;
    }

    protected Vector3 GetMovementInputDirection()
    {
        Vector2 input = stateMachine.ReusableData.MovementInput;
        Vector3 forward = stateMachine.player.MainCameraTransform.forward;
        Vector3 right = stateMachine.player.MainCameraTransform.right;

        forward.y = 0; // ī�޶��� ���� ���⸸ ���
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * input.y + right * input.x;
    }

    void ReadMovementInput()
    {
        stateMachine.ReusableData.MovementInput = stateMachine.player.Input.MovementActions.Movement.ReadValue<Vector2>();
    }
}