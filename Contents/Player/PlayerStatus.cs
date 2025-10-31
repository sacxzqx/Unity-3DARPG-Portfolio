using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어의 기본 능력치(PlayerStat)와 상태를 관리하는 컴포넌트
/// 장비 장착/해제, 아이템 사용, 버프 적용, 경험치 획득, 마나 재생 등
/// 게임 이벤트에 반응하여 플레이어의 영구적인 상태를 업데이트하고 동기화
/// </summary>
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

    /// <summary>
    /// 장비 아이템 해제 이벤트 처리. 해당 장비의 능력치 변화를 반영하고 장착 목록에서 제거
    /// </summary>
    /// <param name="equipment">해제할 장비 아이템 SO</param>
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

    /// <summary>
    /// 버프 스킬 적용 이벤트 처리. 대상 스탯에 모디파이어를 추가하고, 지속 시간 후 제거하는 코루틴을 시작
    /// </summary>
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

    /// <summary>
    /// 적에게 주는 스킬 데미지에 플레이어의 힘(Strength) 스탯을 추가로 반영
    /// </summary>
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
    /// 10초마다 마나 재생량만큼 현재 마나를 회복시키는 코루틴
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
