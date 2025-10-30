using System.Collections.Generic;
using UnityEngine;

public enum StatType { Health, Mana, ManaRegen, Strength, Defense }

public class Stat
{

    private float baseValue;
    public float BaseValue
    {
        get { return baseValue;  }
        private set { baseValue = value; }
    }

    private float maxValue;
    public float MaxValue
    {
        get { return maxValue;  }
        set
        {
            float oldMaxValue = maxValue;

            maxValue = value;

            if (oldMaxValue != maxValue)
            {
                TriggerStatMaxChanged(); // �ִ� �� ���� �̺�Ʈ ȣ��
            }

            if (currentValue > maxValue)
            {
                currentValue = maxValue;
                TriggerStatChanged(); // ���� ���� �ִ� ���� ���� ���������Ƿ� ���� �� ���� �̺�Ʈ�� ȣ��
            }
        }
    }

    private float currentValue;
    public float CurrentValue
    {
        get { return currentValue; }
        set
        {
            currentValue = Mathf.Clamp(value, 0, MaxValue);
            TriggerStatChanged();
        }
    }

    private StatType type;

    private List<float> modifiers = new List<float>();

    public bool IsMaxValueModifier { get; private set; }

    public Stat(StatType type, float baseValue, float maxValue, bool isMaxValueModifier)
    {
        this.type = type;
        BaseValue = baseValue;
        MaxValue = maxValue;
        CurrentValue = BaseValue;
        IsMaxValueModifier = isMaxValueModifier;
    }

    public void AddModifier(float modifier)
    {
        modifiers.Add(modifier);
        CalculateFinalValue();
    }

    public void RemoveModifier(float modifier)
    {
        if (modifiers.Contains(modifier))
        {
            modifiers.Remove(modifier);
            CalculateFinalValue();
        }
    }

    public void IncreaseMaxValue(float addtionalValue)
    {
        MaxValue += addtionalValue;
    }

    private void CalculateFinalValue()
    {
        if (IsMaxValueModifier)
        {
            MaxValue = BaseValue;
            foreach (float modifier in modifiers)
            {
                MaxValue += modifier;
            }
        }
        else
        {
            CurrentValue = BaseValue;
            foreach (float modifier in modifiers)
            {
                CurrentValue += modifier;
            }

            CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);
        }
    }

    public StatSaveData ToSaveData()
    {
        return new StatSaveData
        {
            Type = type,
            BaseValue = BaseValue,
            CurrentValue = CurrentValue,
            MaxValue = MaxValue,
        };
    }

    public void LoadFromData(StatSaveData data)
    {
        BaseValue = data.BaseValue;
        MaxValue = data.MaxValue;
        CurrentValue = data.CurrentValue;

        CalculateFinalValue();
    }

    private void TriggerStatChanged()
    {
        switch (type)
        {
            case StatType.Health:
                GameEventsManager.Instance.PlayerEvents.PlayerHealthChange(CurrentValue);
                break;
            case StatType.Mana:
                GameEventsManager.Instance.PlayerEvents.PlayerManaChange(CurrentValue);
                break;
            case StatType.ManaRegen:
                GameEventsManager.Instance.PlayerEvents.PlayerManaRegenChange(CurrentValue);
                break;
            case StatType.Strength:
                GameEventsManager.Instance.PlayerEvents.PlayerStrengthChange(CurrentValue);
                break;
            case StatType.Defense:
                GameEventsManager.Instance.PlayerEvents.PlayerDefenseChange(CurrentValue);
                break;
        }
    }

    private void TriggerStatMaxChanged()
    {
        switch (type)
        {
            case StatType.Health:
                GameEventsManager.Instance.PlayerEvents.PlayerMaxHealthChange(MaxValue);
                break;
            case StatType.Mana:
                GameEventsManager.Instance.PlayerEvents.PlayerMaxManaChange(MaxValue);
                break;
                // Strength, Defense ���� MaxValue ���� �� UI ������ �ʿ� �����Ƿ� ����
        }
    }
}

public class CharacterStats
{
    public Stat Health;
    public Stat Mana;
    public Stat ManaRegen;
    public Stat Strength;
    public Stat Defense;

    public CharacterStats()
    {
        Health = new Stat(StatType.Health ,100, 100, true);
        Mana = new Stat(StatType.Mana ,50, 50, true);
        ManaRegen = new Stat(StatType.ManaRegen ,2, 100, true);
        Strength = new Stat(StatType.Strength ,10, 999 , false);
        Defense = new Stat(StatType.Defense ,0, 999, false);
    }
}
