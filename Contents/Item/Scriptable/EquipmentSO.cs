using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ������ ������ SO Ŭ����  
/// ���� ���� �� ���� ���� ����ġ�� �����ϸ�, ����/���� �� �÷��̾� ���ȿ� ������ �ش�
/// </summary>
[CreateAssetMenu(fileName = "New Equipment", menuName = "Item/Equipment")]
public class EquipmentSO : ItemSO
{
    public EquipType EquipType;
   
    public List<StatModifier> StatModifiers = new();

    public Dictionary<StatType, float> GetStatModifiers()
    {
        Dictionary<StatType, float> statDict = new();
        foreach (var mod in StatModifiers)
        {
            statDict[mod.StatType] = mod.Value;
        }
        return statDict;
    }

    /// <summary>
    /// ����� ȿ�� ���� �ؽ�Ʈ  
    /// UI�� ǥ�õ� �� ������, ���� ������ �ڵ����� ���� ���ڿ��� ����
    /// </summary>
    public override string Description
    {
        get
        {
            if (StatModifiers.Count == 0)
                return "ȿ���� ���� ���";

            List<string> statDescriptions = new();
            foreach (var mod in StatModifiers)
            {
                string statName = mod.StatType switch
                {
                    StatType.Health => "�ִ� ü��",
                    StatType.Mana => "�ִ� ����",
                    StatType.ManaRegen => "�ִ� ���",
                    StatType.Strength => "���ݷ�",
                    StatType.Defense => "����",
                    _ => mod.StatType.ToString()
                };
                statDescriptions.Add($"{statName} +{mod.Value}");
            }
            return string.Join(", ", statDescriptions);
        }
    }

    public void Equip(PlayerStat playerStat)
    {
        playerStat.EquipItem(GetStatModifiers());
    }

    public void UnEquip(PlayerStat playerStat)
    {
        playerStat.UnEquipItem(GetStatModifiers());
    }
}