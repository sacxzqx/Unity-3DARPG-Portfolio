using System;
using UnityEngine;

/// <summary>
/// �� ���� ���� �̺�Ʈ�� �����ϴ� Ŭ����
/// ���� ���, ����, ü�� ���� ��ȭ ���� �̺�Ʈ�� ����
/// </summary>
public class EnemyEvents
{
    public event Action<IEnemy> OnEnemyDie;
    public void EnemyDie(IEnemy enemy)
    {
        if (OnEnemyDie != null)
        {
            OnEnemyDie(enemy);
        }
    }

    public event Action<IEnemy, GameObject, Transform> OnEnemyDiedForRespawn;
    public void EnemyDiedForRespawn(IEnemy enemy, GameObject prefab, Transform waypointRoot)
    {
        OnEnemyDiedForRespawn?.Invoke(enemy, prefab, waypointRoot);
    }

    public event Action<IEnemy> OnEnemySpawned;
    public void EnemySpawned(IEnemy enemy)
    {
        OnEnemySpawned?.Invoke(enemy);
    }

    public event Action<IEnemy> OnEnemyDespawned;
    public void EnemyDespawned(IEnemy enemy)
    {
        OnEnemyDespawned?.Invoke(enemy);
    }

    /// <summary>
    /// ���� ��ų�� ����� �� �������� �����ϴ� �̺�Ʈ
    /// ���� �����ʰ� baseDamage�� �����ؼ� ��ȯ ����
    /// </summary>
    public event Func<SkillSO, int, int> OnCalculateSkillDamage;
    public int EnemySkillHit(SkillSO skill, int baseDamage = 0)
    {
        if (OnCalculateSkillDamage != null)
        {
            foreach (Func<SkillSO, int, int> del in OnCalculateSkillDamage.GetInvocationList())
            {
                baseDamage = del(skill, baseDamage);
            }
        }

        return baseDamage;
    }

    /// <summary>
    /// ü���� ������ �������� �� (�и� ���� ���� ��)
    /// </summary>
    public event Action<Enemy> OnPostureBreak;
    public void PostureBreak(Enemy enemy)
    {
        if (OnPostureBreak != null)
        {
            OnPostureBreak(enemy);
        }
    }

    public event Action OnRecoveryPosture;
    public void PostureRecovery()
    {
        if (OnRecoveryPosture != null)
        {
            OnRecoveryPosture();
        }
    }

    public event Action OnPostureGaugeChanged;
    public void PostureGaugeChanged()
    {
        if (OnPostureGaugeChanged != null)
        {
            OnPostureGaugeChanged();
        }
    }

    /// <summary>
    /// ���� �÷��̾ �ν��ϰ� ������ ���� ��
    /// ���� ���� ���濡 ���
    /// </summary>
    public event Action OnEnemyEnterBattle;
    public void EnterBattleMusic()
    {
        if (OnEnemyEnterBattle != null)
        {
            OnEnemyEnterBattle();
        }
    }

    public event Action OnEnemyExitBattle;
    public void ExitBattleMusic()
    {
        if (OnEnemyExitBattle != null)
        {
            OnEnemyExitBattle();
        }
    }
}
