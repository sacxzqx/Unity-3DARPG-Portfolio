using System;
using UnityEngine;

/// <summary>
/// 플레이어 관련 이벤트들을 관리하는 클래스
/// 레벨, 경험치, 체력, 스탯 등의 상태 변경 알림을 처리
/// </summary>
public class PlayerEvents
{
    public event Action<int> OnPlayerLevelUp;
    public void PlayerLevelUp(int level)
    {
        OnPlayerLevelUp?.Invoke(level);
    }

    public event Action<int> OnPlayerLevelChanged;
    public void PlayerLevelChange(int level)
    {
        OnPlayerLevelChanged?.Invoke(level);
    }

    public event Action<int> OnExperienceGained;
    public void ExperienceGained(int experienceGained)
    {
        OnExperienceGained?.Invoke(experienceGained);
    }

    public event Action<float> OnHealthChanged;
    public void PlayerHealthChange(float changeAmount)
    {
        OnHealthChanged?.Invoke(changeAmount);
    }

    public event Action<float> OnManaChanged;
    public void PlayerManaChange(float changeAmount)
    {
        OnManaChanged?.Invoke(changeAmount);
    }

    public event Action<float> OnManaRegenChanged;
    public void PlayerManaRegenChange(float changeAmount)
    {
        OnManaRegenChanged?.Invoke(changeAmount);
    }

    public event Action<float> OnStrengthChanged;
    public void PlayerStrengthChange(float changeAmount)
    {
        OnStrengthChanged?.Invoke(changeAmount);
    }

    public event Action<float> OnDefenseChanged;
    public void PlayerDefenseChange(float changeAmount)
    {
        OnDefenseChanged?.Invoke(changeAmount);
    }

    public event Action<float> OnMaxHealthChanged;
    public void PlayerMaxHealthChange(float maxHealth)
    {
        OnMaxHealthChanged?.Invoke(maxHealth);
    }

    public event Action<float> OnMaxManaChanged;
    public void PlayerMaxManaChange(float maxMana)
    {
        OnMaxManaChanged?.Invoke(maxMana);
    }

    public event Action<SkillSO> OnApplyBuff;
    public void ApplyBuff(SkillSO skill)
    {
        OnApplyBuff?.Invoke(skill);
    }

    public event Action<StatType> OnStatIncreased;
    public void IncreasPlayerStat(StatType stat)
    {
        OnStatIncreased?.Invoke(stat);
    }

    public event Action<int> OnStatPointUsed;
    public void UseStatPoint(int amount)
    {
        OnStatPointUsed?.Invoke(amount);
    }

    public event Action<int> OnStatPointChanged;
    public void StatPointChange(int currentSkillPoint)
    {
        OnStatPointChanged?.Invoke(currentSkillPoint);
    }

    public event Action OnPlayerDied;
    public void PlayerDie()
    {
        OnPlayerDied?.Invoke();
    }

    public event Action OnKillMoveStarted;
    public void StartKillMove()
    {
        OnKillMoveStarted?.Invoke();
    }
}
