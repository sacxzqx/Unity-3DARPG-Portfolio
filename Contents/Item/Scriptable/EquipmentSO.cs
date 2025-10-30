using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 아이템 데이터 SO 클래스  
/// 장착 부위 및 적용 스탯 보정치를 포함하며, 장착/해제 시 플레이어 스탯에 영향을 준다
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
    /// 장비의 효과 설명 텍스트  
    /// UI에 표시될 수 있으며, 보정 스탯을 자동으로 설명 문자열로 구성
    /// </summary>
    public override string Description
    {
        get
        {
            if (StatModifiers.Count == 0)
                return "효과가 없는 장비";

            List<string> statDescriptions = new();
            foreach (var mod in StatModifiers)
            {
                string statName = mod.StatType switch
                {
                    StatType.Health => "최대 체력",
                    StatType.Mana => "최대 마나",
                    StatType.ManaRegen => "최대 기력",
                    StatType.Strength => "공격력",
                    StatType.Defense => "방어력",
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