using UnityEngine.InputSystem;

/// <summary>
/// 플레이어가 무기를 넣은 비전투 상태를 관리
/// 이 상태에서는 공격, 스킬 등의 전투 행동이 비활성화되며
/// 무기 꺼내기 입력을 통해 전투 상태로 전환할 수 있음
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
