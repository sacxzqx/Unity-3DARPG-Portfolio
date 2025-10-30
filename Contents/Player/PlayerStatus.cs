using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private PlayerContext playerContext;

    public PlayerStat playerStat { get; private set; }

    private Coroutine manaRegenCoroutine;

    private void Awake()
    {
        playerStat = new PlayerStat();

        SaveManager.Instance.RegisterSavable(playerStat);
    }

    private void OnDestroy()
    {
        SaveManager.Instance.UnregisterSavable(playerStat);
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.ItemEvents.OnItemUsed += HandleItemUsed;
        GameEventsManager.Instance.ItemEvents.OnUnequipItem += HandleItemUnequipped;

        GameEventsManager.Instance.EnemyEvents.OnCalculateSkillDamage += AddStrengthDamage;

        GameEventsManager.Instance.PlayerEvents.OnStatPointUsed += playerStat.UseStatPoint;
        GameEventsManager.Instance.PlayerEvents.OnStatIncreased += playerStat.IncreaseStat;
        GameEventsManager.Instance.PlayerEvents.OnApplyBuff += HandleBuff;

        GameEventsManager.Instance.PlayerEvents.OnExperienceGained += playerStat.GainExperience;
        GameEventsManager.Instance.EnemyEvents.OnEnemyDie += playerStat.OnEnemyDie;

        if (manaRegenCoroutine == null)
        {
            manaRegenCoroutine = StartCoroutine(ManaRegenerationCoroutine());
        }
    }

    void OnDisable()
    {
        GameEventsManager.Instance.ItemEvents.OnItemUsed -= HandleItemUsed;
        GameEventsManager.Instance.ItemEvents.OnUnequipItem -= HandleItemUnequipped;

        GameEventsManager.Instance.EnemyEvents.OnCalculateSkillDamage -= AddStrengthDamage;

        GameEventsManager.Instance.PlayerEvents.OnStatPointUsed -= playerStat.UseStatPoint;
        GameEventsManager.Instance.PlayerEvents.OnStatIncreased -= playerStat.IncreaseStat;
        GameEventsManager.Instance.PlayerEvents.OnApplyBuff -= HandleBuff;

        GameEventsManager.Instance.PlayerEvents.OnExperienceGained -= playerStat.GainExperience;
        GameEventsManager.Instance.EnemyEvents.OnEnemyDie -= playerStat.OnEnemyDie;

        if (manaRegenCoroutine != null)
        {
            StopCoroutine(manaRegenCoroutine);
            manaRegenCoroutine = null;
        }
    }

    private void HandleItemUsed(ItemSO item)
    {
        if (item is ConsumableSO consumable)
        {
            consumable.Use(playerStat);
        }
        else if (item is EquipmentSO equipment)
        {
            if (playerStat.EquippedItems.ContainsKey(equipment.EquipType))
            {
                Debug.LogError($"[HandleItemUsed] {equipment.EquipType} 부위의 아이템이 해제되지 않았는데 장착 요청이 들어왔습니다. InventoryHandler의 Equip 로직을 확인하세요.");
            }

            equipment.Equip(playerStat);
            playerStat.EquippedItems[equipment.EquipType] = equipment;
            if (!SaveManager.Instance.IsLoading)
            {
                AudioManager.Instance.PlaySFX("Equip");
            }
        }
    }

    private void HandleItemUnequipped(EquipmentSO equipment)
    {
        // 장착된 아이템 목록에 해당 아이템이 있는지 다시 한번 확인
        if (playerStat.EquippedItems.ContainsKey(equipment.EquipType))
        {
            equipment.UnEquip(playerStat);

            playerStat.EquippedItems.Remove(equipment.EquipType);

            if (!SaveManager.Instance.IsLoading)
            {
                AudioManager.Instance.PlaySFX("UnEquip");
            }
        }
        else
        {
            Debug.LogWarning($"[HandleItemUnequipped] 장착 목록에 없는 아이템({equipment.ItemName})의 해제 요청이 들어왔습니다.");
        }
    }

    private bool HasEquipped(EquipType type)
    {
        return playerStat.EquippedItems.ContainsKey(type);
    }

    private void HandleBuff(SkillSO buffSkill)
    {
        if (buffSkill.Type != SkillType.Buff) return;

        Stat targetStat = null;

        switch (buffSkill.TargetStat)
        {
            case StatType.Health:
                targetStat = playerStat.Stats.Health;
                break;
            case StatType.Mana:
                targetStat = playerStat.Stats.Mana;
                break;
            case StatType.Strength:
                targetStat = playerStat.Stats.Strength;
                break;
            case StatType.Defense:
                targetStat = playerStat.Stats.Defense;
                break;
        }

        if (targetStat != null)
        {
            StartCoroutine(ApplyBuffCoroutine(targetStat, buffSkill.SkillPower, buffSkill.BuffDuration));
        }
    }

    private IEnumerator ApplyBuffCoroutine(Stat targetStat, float modifierValue, float duration)
    {
        targetStat.AddModifier(modifierValue);

        yield return new WaitForSeconds(duration);

        targetStat.RemoveModifier(modifierValue);
    }

    private int AddStrengthDamage(SkillSO _, int currentDamage)
    {
        int strengthValue = Mathf.RoundToInt(playerStat.Stats.Strength.CurrentValue);

        return currentDamage + strengthValue;
    }

    private void RecoveryAllStatus()
    {
        playerStat.Stats.Health.CurrentValue = playerStat.Stats.Health.MaxValue;
        playerStat.Stats.Mana.CurrentValue = playerStat.Stats.Mana.MaxValue;
    }

    /// <summary>
    /// 1초마다 마나 재생량만큼 현재 마나를 회복시키는 코루틴
    /// </summary>
    private IEnumerator ManaRegenerationCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            if (playerStat.Stats.Mana.CurrentValue < playerStat.Stats.Mana.MaxValue)
            {
                float regenAmount = playerStat.Stats.ManaRegen.CurrentValue;

                playerStat.Stats.Mana.CurrentValue += regenAmount;
            }
        }
    }
}
