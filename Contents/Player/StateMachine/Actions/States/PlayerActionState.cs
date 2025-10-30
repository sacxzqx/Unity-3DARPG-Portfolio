using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 액션 관련 공통 로직을 담당하는 베이스 액션 상태 클래스
/// 공격, 방어, 피격등 기본 액션 기능을 정의
/// </summary>
public class PlayerActionState : IState
{
    protected PlayerActionStateMachine stateMachine;

    protected PlayerActionData actionData;

    public PlayerActionState(PlayerActionStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

        actionData = stateMachine.player.Data.ActionData;
    }

    public virtual void EnterState()
    {
    }

    public virtual void ExitState()
    {
    }

    public virtual void HandleInput()
    {
    }

    public virtual void OnAnimationEnterEvent()
    {
    }

    public virtual void OnAnimationExitEvent()
    {
    }

    public virtual void OnAnimationTransitionEvent()
    {
    }

    public virtual void OnTriggerEnter(Collider collider)
    {
        if (stateMachine.player.LayerData.IsEnemyLayer(collider.gameObject.layer))
        {
            if (stateMachine.CurrentState == stateMachine.DeathState)
            {
                return;
            }

            OnHitEnter(collider);

            if (stateMachine.player.Stat.Stats.Health.CurrentValue <= 0)
            {
                GameEventsManager.Instance.PlayerEvents.PlayerDie();
                stateMachine.SetState(stateMachine.DeathState);
            }

            return;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void Update()
    {
    }

    protected void StartAnimation(int animationHash)
    {
        stateMachine.player.Anim.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        stateMachine.player.Anim.SetBool(animationHash, false);
    }

    public virtual void OnAttackPerformed(InputAction.CallbackContext context)
    {
        // 공중에서는 공격이 불가능
        if (!stateMachine.player.Anim.GetBool("Grounded"))
        {
            return;
        }

        Enemy targetEnemy = FindFrontEnemyWithPostureBreak();
        if (targetEnemy != null)
        {
            stateMachine.KillMoveState.SetTarget(targetEnemy);
            stateMachine.SetState(stateMachine.KillMoveState);
        }
        else
        {
            if (stateMachine.CurrentState != stateMachine.player.ActionStateMachine.AttackState)
            {
                stateMachine.SetState(stateMachine.AttackState);
            }
        }
    }

    public virtual void OnDefendPerformed(InputAction.CallbackContext context)
    {
        if (stateMachine.CurrentState != stateMachine.SheathedState)
            stateMachine.SetState(stateMachine.DefenseState);
    }

    public virtual void OnDefendCanceled(InputAction.CallbackContext context)
    {
        if (stateMachine.CurrentState == stateMachine.DefenseState || stateMachine.CurrentState == stateMachine.ParryState)
            stateMachine.SetState(stateMachine.WeaponDrawnState);
    }

    public virtual void OnSkillPerformed(InputAction.CallbackContext context)
    {
        // 공중에서는 스킬 사용이 불가능
        if (!stateMachine.player.Anim.GetBool("Grounded"))
        {
            return;
        }

        string key = context.control.path;

        SkillUseResult result = stateMachine.player.SkillManager.UseSkill(key);

        if (result == SkillUseResult.Success)
        {
            stateMachine.SetState(stateMachine.SkillState);
        }
        else
        {
            GameEventsManager.Instance.UIEvents.SkillFailureDisplay(result);
        }
    }

    /// <summary>
    /// 피격이 발생했을 때 실행되는 공통 처리 로직
    /// 상태를 피격 상태로 전환하고, 데미지를 계산하여 플레이어 체력에 적용
    /// </summary>
    /// <param name="collider">충돌을 일으킨 객체의 콜라이더</param>
    protected virtual void OnHitEnter(Collider collider)
    {
        stateMachine.SetState(stateMachine.HitState);

        int damage = CalculateDamage(collider);

        ApplyDamage(damage);
    }

    protected virtual void OnHitExit(Collider collider)
    {
    }

    protected virtual int CalculateDamage(Collider collider)
    {
        IDamageProvider damageProvider = collider.GetComponentInParent<IDamageProvider>();

        if (damageProvider != null)
        {
            return damageProvider.GetDamageAmount();
        }
        return 0;
    }

    protected void ApplyDamage(int damge)
    {
        stateMachine.player.Stat.TakeDamage(damge);
    }

    public virtual void DrawWeapon(InputAction.CallbackContext context)
    {
        // 공중에서는 불가능
        if (!stateMachine.player.Anim.GetBool("Grounded"))
        {
            return;
        }

        if (stateMachine.CurrentState == stateMachine.WeaponDrawnState ||
            stateMachine.CurrentState == stateMachine.SheathedState)
        {
            stateMachine.SetState(stateMachine.WeaponToggleState);
        }
        else return;
    }

    public void SetWeaponEquippedStatus(bool isEquipped)
    {
        stateMachine.ReusableData.IsWeaponEquipped = isEquipped;
    }

    /// <summary>
    /// 플레이어 전방 직선상에 체간게이지가 가득 찬 적이 있는지 찾고
    /// 여러 명이 감지될 경우, 가장 가까운 적을 우선하여 반환
    /// </summary>
    /// <returns>처형 가능한 가장 가까운 적. 없으면 null.</returns>
    private Enemy FindFrontEnemyWithPostureBreak()
    {
        float detectionRange = 3f;
        Vector3 origin = stateMachine.player.transform.position + Vector3.up * 1.5f;
        Vector3 direction = stateMachine.player.transform.forward;

        // 1. RaycastAll을 사용하여 광선에 맞는 모든 객체를 가져옴
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, detectionRange);

        // 2. 맞은 모든 객체들을 가까운 순서대로 확인
        foreach (RaycastHit hit in hits)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();

            // 3. 적을 찾으면 해당 적이 처형 가능한 상태인지 확인
            if (enemy != null && enemy.IsParryGuageFull)
            {
                return enemy;
            }
        }

        return null;
    }
}
