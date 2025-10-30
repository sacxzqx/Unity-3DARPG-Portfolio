using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어가 피격당했을 때의 상태를 관리
/// 이 상태에 진입하면 짧은 무적 시간이 부여되며,
/// 무적 시간이 끝난 후에는 애니메이션이 끝나기 전이라도 다시 피격될 수 있음
/// 피격 애니메이션은 무기 장착 여부에 따라 다르게 재생
/// </summary>
public class PlayerHitState : PlayerActionState
{
    private bool isInvincible = false;

    public PlayerHitState(PlayerActionStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void EnterState()
    {
        // 무기 장착 여부에 따라 다른 피격 애니메이션 재생
        if (stateMachine.ReusableData.IsWeaponEquipped)
        {
            StartAnimation(stateMachine.player.AnimationsData.HitOnBattleParameterHash);
        }
        else
        {
            StartAnimation(stateMachine.player.AnimationsData.HitOnSheathedParameterHash);
        }

        stateMachine.ReusableData.CanMove = false;

        stateMachine.player.Rigidbody.velocity = Vector3.zero;

        isInvincible = true;
    }

    public override void ExitState()
    {
        isInvincible = false;

        stateMachine.ReusableData.CanMove = true;
    }

    /// <summary>
    /// 피격 애니메이션의 '무적' 구간이 끝나는 시점에 애니메이션 이벤트를 통해 호출
    /// 이 시점부터 플레이어는 애니메이션이 완전히 끝나기 전이라도 다시 피격될 수 있음
    /// </summary>
    public override void OnAnimationTransitionEvent()
    {
        isInvincible = false;
    }

    /// <summary>
    /// 피격 애니메이션이 완전히 끝났을 때 애니메이션 이벤트를 통해 호출
    /// 이전의 무기 상태에 따라 적절한 기본 상태로 복귀
    /// </summary>
    public override void OnAnimationExitEvent()
    {
        if (stateMachine.ReusableData.IsWeaponEquipped)
        {
            stateMachine.SetState(stateMachine.WeaponDrawnState);
        }
        else
        {
            stateMachine.SetState(stateMachine.SheathedState);
        }
    }

    protected override void OnHitEnter(Collider collider)
    {
        if (isInvincible)
        {
            return;
        }

        base.OnHitEnter(collider);
    }

    public override void OnAttackPerformed(InputAction.CallbackContext context)
    {
    }

    public override void OnDefendPerformed(InputAction.CallbackContext context)
    {
    }

    public override void OnDefendCanceled(InputAction.CallbackContext context)
    {
    }

    public override void OnSkillPerformed(InputAction.CallbackContext context)
    {
    }
}
