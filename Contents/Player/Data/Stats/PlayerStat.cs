using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : ISavable
{
    private CharacterStats stats = new CharacterStats();
    public CharacterStats Stats => stats;

    public Dictionary<EquipType, EquipmentSO> EquippedItems = new();

    public int Level { get; private set; } = 1;
    public int Experience { get; private set; } = 0;
    public int ExperienceToNextLevel { get; private set; } = 100;

    public int StatPoint { get; private set; } = 0;

    public void TakeDamage(float damage)
    {
        float actualDamage = damage - Stats.Defense.CurrentValue;
        if (actualDamage < 0) actualDamage = 0;

        Stats.Health.CurrentValue -= actualDamage;
    }

    public void OnEnemyDie(IEnemy enemy)
    {
        GameEventsManager.Instance.PlayerEvents.ExperienceGained(enemy.EnemyData.Experience);
    }

    public void GainExperience(int amount)
    {
        Experience += amount;

        while (Experience >= ExperienceToNextLevel)
        {
            Experience -= ExperienceToNextLevel;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        Level++;
        ExperienceToNextLevel += 50;
        StatPoint += 3;
        GameEventsManager.Instance.PlayerEvents.StatPointChange(StatPoint);

        GameEventsManager.Instance.PlayerEvents.PlayerLevelUp(Level);

        Debug.Log($"레벨업! 현재 레벨: {Level}, 남은 경험치: {Experience}");
    }

    public void UseStatPoint(int amount)
    {
        StatPoint -= amount;
        GameEventsManager.Instance.PlayerEvents.StatPointChange(StatPoint);
    }

    /// <summary>
    /// 스테이터스 UI에서 스탯을 상승시켰을 때 호출되는 함수
    /// </summary>
    public void IncreaseStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.Health:
                Stats.Health.MaxValue += 10;
                Stats.Health.BaseValue += 10;
                break;
            case StatType.Mana:
                Stats.Mana.MaxValue += 10;
                Stats.Mana.BaseValue += 10;
                break;
            case StatType.ManaRegen:
                Stats.ManaRegen.MaxValue += 2;
                Stats.ManaRegen.BaseValue += 2;
                break;
            case StatType.Strength:
                Stats.Strength.BaseValue += 5;
                break;
            case StatType.Defense:
                Stats.Defense.BaseValue += 5;
                break;
        }
    }

    public void EquipItem(Dictionary<StatType, float> statModifiers)
    {
        foreach (var modifier in statModifiers)
        {
            switch (modifier.Key)
            {
                case StatType.Health:
                    Stats.Health.AddModifier(modifier.Value);
                    break;
                case StatType.Mana:
                    Stats.Mana.AddModifier(modifier.Value);
                    break;
                case StatType.ManaRegen:
                    Stats.ManaRegen.AddModifier(modifier.Value);
                    break;
                case StatType.Strength:
                    Stats.Strength.AddModifier(modifier.Value);
                    break;
                case StatType.Defense:
                    Stats.Defense.AddModifier(modifier.Value);
                    break;
            }
        }
    }

    public void UnEquipItem(Dictionary<StatType, float> statModifiers)
    {
        foreach (var modifier in statModifiers)
        {
            switch (modifier.Key)
            {
                case StatType.Health:
                    Stats.Health.RemoveModifier(modifier.Value);
                    break;
                case StatType.Mana:
                    Stats.Mana.RemoveModifier(modifier.Value);
                    break;
                case StatType.ManaRegen:
                    Stats.ManaRegen.RemoveModifier(modifier.Value);
                    break;
                case StatType.Strength:
                    Stats.Strength.RemoveModifier(modifier.Value);
                    break;
                case StatType.Defense:
                    Stats.Defense.RemoveModifier(modifier.Value);
                    break;
            }
        }
    }

    public void UnequipAllItems()
    {
        List<EquipmentSO> currentlyEquipped = new List<EquipmentSO>(EquippedItems.Values);

        foreach (EquipmentSO equipment in currentlyEquipped)
        {
            UnEquipItem(equipment.GetStatModifiers());
        }

        Debug.Log("인벤토리 클리어");
        EquippedItems.Clear();
    }

    public void RestoreStat(PotionType potionType, float recoveryAmount)
    {
        switch(potionType)
        {
            case PotionType.Health:
                Stats.Health.CurrentValue += recoveryAmount;
                if (Stats.Health.CurrentValue > Stats.Health.BaseValue)
                    Stats.Health.CurrentValue = Stats.Health.BaseValue;
                break;

            case PotionType.Mana:
                Stats.Mana.CurrentValue += recoveryAmount;
                if (Stats.Mana.CurrentValue > Stats.Mana.BaseValue)
                    Stats.Mana.CurrentValue = Stats.Mana.BaseValue;
                break;
        }
    }

    public void ApplyStrengthBuff(float buffAmount)
    {
        Stats.Strength.AddModifier(buffAmount);
    }

    public void SaveData(GameSaveData data)
    {
        data.PlayerData.Level = Level;
        data.PlayerData.Experience = Experience;
        data.PlayerData.StatPoint = StatPoint;

        data.PlayerData.Stats.Add(Stats.Health.ToSaveData());
        data.PlayerData.Stats.Add(Stats.Mana.ToSaveData());
        data.PlayerData.Stats.Add(Stats.ManaRegen.ToSaveData());
        data.PlayerData.Stats.Add(Stats.Strength.ToSaveData());
        data.PlayerData.Stats.Add(Stats.Defense.ToSaveData());
    }

    public void LoadData(GameSaveData data)
    {
        Level = data.PlayerData.Level;
        Experience = data.PlayerData.Experience;
        StatPoint = data.PlayerData.StatPoint;

        GameEventsManager.Instance.PlayerEvents.StatPointChange(data.PlayerData.StatPoint);
        GameEventsManager.Instance.PlayerEvents.PlayerLevelChange(data.PlayerData.Level);

        foreach (var statData in data.PlayerData.Stats)
        {
            switch (statData.Type)
            {
                case StatType.Health:
                    Stats.Health.LoadFromData(statData);
                    GameEventsManager.Instance.PlayerEvents.PlayerHealthChange(statData.CurrentValue);
                    break;
                case StatType.Mana:
                    Stats.Mana.LoadFromData(statData);
                    GameEventsManager.Instance.PlayerEvents.PlayerManaChange(statData.CurrentValue);
                    break;
                case StatType.ManaRegen:
                    Stats.ManaRegen.LoadFromData(statData);
                    GameEventsManager.Instance.PlayerEvents.PlayerManaRegenChange(statData.CurrentValue);
                    break;
                case StatType.Strength:
                    Stats.Strength.LoadFromData(statData);
                    GameEventsManager.Instance.PlayerEvents.PlayerStrengthChange(statData.CurrentValue);
                    break;
                case StatType.Defense:
                    Stats.Defense.LoadFromData(statData);
                    GameEventsManager.Instance.PlayerEvents.PlayerDefenseChange(statData.CurrentValue);
                    break;
            }
        }
    }
}
