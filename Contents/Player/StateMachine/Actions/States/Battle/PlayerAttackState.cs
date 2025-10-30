using UnityEngine;

/// <summary>
/// 플레이어의 공격 상태를 관리하는 클래스
/// 공격 중에는 이동이 제한되고 루트 모션을 통해 움직이도록 함
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
        // 공격 애니메이션이 재생되는 동안, 다음 공격이 나갈 방향을
        // 플레이어의 현재 이동 입력에 맞춰 계속 갱신
        ReadMovementInput();
    }

    public override void PhysicsUpdate()
    {
        RotatePlayer(); // 방향키를 입력한 방향으로 캐릭터를 회전
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

        forward.y = 0; // 카메라의 수평 방향만 고려
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