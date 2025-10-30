using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾� �׼� ���� ���� ������ ����ϴ� ���̽� �׼� ���� Ŭ����
/// ����, ���, �ǰݵ� �⺻ �׼� ����� ����
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
        // ���߿����� ������ �Ұ���
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
        // ���߿����� ��ų ����� �Ұ���
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
    /// �ǰ��� �߻����� �� ����Ǵ� ���� ó�� ����
    /// ���¸� �ǰ� ���·� ��ȯ�ϰ�, �������� ����Ͽ� �÷��̾� ü�¿� ����
    /// </summary>
    /// <param name="collider">�浹�� ����Ų ��ü�� �ݶ��̴�</param>
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
        // ���߿����� �Ұ���
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
    /// �÷��̾� ���� ������ ü���������� ���� �� ���� �ִ��� ã��
    /// ���� ���� ������ ���, ���� ����� ���� �켱�Ͽ� ��ȯ
    /// </summary>
    /// <returns>ó�� ������ ���� ����� ��. ������ null.</returns>
    private Enemy FindFrontEnemyWithPostureBreak()
    {
        float detectionRange = 3f;
        Vector3 origin = stateMachine.player.transform.position + Vector3.up * 1.5f;
        Vector3 direction = stateMachine.player.transform.forward;

        // 1. RaycastAll�� ����Ͽ� ������ �´� ��� ��ü�� ������
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, detectionRange);

        // 2. ���� ��� ��ü���� ����� ������� Ȯ��
        foreach (RaycastHit hit in hits)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();

            // 3. ���� ã���� �ش� ���� ó�� ������ �������� Ȯ��
            if (enemy != null && enemy.IsParryGuageFull)
            {
                return enemy;
            }
        }

        return null;
    }
}
