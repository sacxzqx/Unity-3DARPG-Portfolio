using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾ ���⸦ ���� ������ ���¸� ����
/// �� ���¿����� ����, ��ų ���� ���� �ൿ�� ��Ȱ��ȭ�Ǹ�
/// ���� ������ �Է��� ���� ���� ���·� ��ȯ�� �� ����
/// </summary>
public class PlayerWeaponSheathedState : PlayerActionState
{
    public PlayerWeaponSheathedState(PlayerActionStateMachine playerActionStateMachine) : base(playerActionStateMachine)
    {
    }

    public override void EnterState()
    {
        stateMachine.player.Input.PlayerActions.Enable();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void OnAttackPerformed(InputAction.CallbackContext context)
    {
    }

    public override void OnDefendCanceled(InputAction.CallbackContext context)
    {
    }

    public override void OnSkillPerformed(InputAction.CallbackContext context)
    {
    }
}
