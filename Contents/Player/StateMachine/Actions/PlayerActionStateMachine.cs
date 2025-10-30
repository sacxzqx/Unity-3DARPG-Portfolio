using UnityEngine.InputSystem;

public class PlayerActionStateMachine : StateMachine
{
    public PlayerMovementStateMachine movementStateMachine;
    public PlayerContext player { get; }
    public PlayerStateReusableData ReusableData { get; }
    public PlayerWeaponDrawnState WeaponDrawnState { get; }
    public PlayerWeaponSheathedState SheathedState { get; }
    public PlayerWeaponToggleState WeaponToggleState { get; }
    public PlayerConversationState ConversationState { get; }
    public PlayerAttackState AttackState { get; }  
    public PlayerDefenseState DefenseState { get; }
    public PlayerParryState ParryState { get; }
    public PlayerSkillState SkillState { get; }
    public PlayerKillMoveState KillMoveState { get; }
    public PlayerHitState HitState { get; }
    public PlayerDeathState DeathState { get; }

    public PlayerActionStateMachine(PlayerContext player, PlayerMovementStateMachine movementStateMachine)
    {
        this.player = player;
        this.movementStateMachine = movementStateMachine;

        ReusableData = movementStateMachine.ReusableData;

        WeaponDrawnState = new PlayerWeaponDrawnState(this);

        SheathedState = new PlayerWeaponSheathedState(this);

        WeaponToggleState = new PlayerWeaponToggleState(this);

        ConversationState = new PlayerConversationState(this);

        AttackState = new PlayerAttackState(this);
        DefenseState = new PlayerDefenseState(this);
        ParryState = new PlayerParryState(this);
        SkillState = new PlayerSkillState(this);
        KillMoveState = new PlayerKillMoveState(this);
        HitState = new PlayerHitState(this);
        DeathState = new PlayerDeathState(this);
    }

    /// <summary>
    /// 모든 전투 관련 입력을 구독
    /// </summary>
    public void SubscribeInputs()
    {
        player.Input.PlayerActions.Attack.performed += HandleAttackInput;
        player.Input.PlayerActions.WeaponDraw.performed += HandleDrawWeaponInput;
        player.Input.PlayerActions.Parry.performed += HandleParryPerformedInput;
        player.Input.PlayerActions.Parry.canceled += HandleParryCanceledInput;
        player.Input.PlayerActions.Skill1.performed += HandleSkillInput;
        player.Input.PlayerActions.Skill2.performed += HandleSkillInput;
        player.Input.PlayerActions.Skill3.performed += HandleSkillInput;
        player.Input.PlayerActions.Skill4.performed += HandleSkillInput;
    }

    /// <summary>
    /// 구독했던 모든 전투 관련 입력을 해제
    /// </summary>
    public void UnsubscribeInputs()
    {
        player.Input.PlayerActions.Attack.performed -= HandleAttackInput;
        player.Input.PlayerActions.WeaponDraw.performed -= HandleDrawWeaponInput;
        player.Input.PlayerActions.Parry.performed -= HandleParryPerformedInput;
        player.Input.PlayerActions.Parry.canceled -= HandleParryCanceledInput;
        player.Input.PlayerActions.Skill1.performed -= HandleSkillInput;
        player.Input.PlayerActions.Skill2.performed -= HandleSkillInput;
        player.Input.PlayerActions.Skill3.performed -= HandleSkillInput;
        player.Input.PlayerActions.Skill4.performed -= HandleSkillInput;
    }

    // --- 입력 분배기 메서드들 ---
    // 입력이 들어오면, 현재 활성화된 상태(currentState)에게 처리를 위임

    private void HandleAttackInput(InputAction.CallbackContext context)
    {
        (currentState as PlayerActionState)?.OnAttackPerformed(context);
    }

    private void HandleDrawWeaponInput(InputAction.CallbackContext context)
    {
        (currentState as PlayerActionState)?.DrawWeapon(context);
    }

    private void HandleParryPerformedInput(InputAction.CallbackContext context)
    {
        (currentState as PlayerActionState)?.OnDefendPerformed(context);
    }

    private void HandleParryCanceledInput(InputAction.CallbackContext context)
    {
        (currentState as PlayerActionState)?.OnDefendCanceled(context);
    }

    private void HandleSkillInput(InputAction.CallbackContext context)
    {
        (currentState as PlayerActionState)?.OnSkillPerformed(context);
    }
}
