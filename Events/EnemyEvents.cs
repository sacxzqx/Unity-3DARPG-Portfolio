using System;
using UnityEngine;

/// <summary>
/// 적 관련 게임 이벤트를 관리하는 클래스
/// 적의 사망, 공격, 체간 상태 변화 등의 이벤트를 정의
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
    /// 적이 스킬을 사용할 때 데미지를 수정하는 이벤트
    /// 여러 리스너가 baseDamage를 가공해서 반환 가능
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
    /// 체간이 완전히 무너졌을 때 (패링 가능 상태 등)
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
    /// 적이 플레이어를 인식하고 전투에 들어갔을 때
    /// 음악 등의 변경에 사용
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
