using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 습득한 스킬을 관리하고, 스킬 포인트, 스킬 할당, 쿨다운 등의 로직을 담당하는 매니저 클래스
/// </summary>
public class SkillManager : MonoBehaviour, ISavable
{
    [Header("Database")]
    [Tooltip("게임에 존재하는 모든 스킬 SO 에셋을 여기에 등록합니다.")]
    [SerializeField] private List<SkillSO> allSkillSOs = new List<SkillSO>();
    private Dictionary<string, SkillSO> skillDatabase = new Dictionary<string, SkillSO>();

    private PlayerContext playerContext;

    [SerializeField] private int skillPoint = 2;

    private Dictionary<string, SkillSO> learnedSkills = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, SkillSO> skillBindings = new Dictionary<string, SkillSO>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<SkillSO, float> cooldownTracker = new Dictionary<SkillSO, float>();

    public void Initialize(PlayerStatus playerStatus, PlayerContext playerContext)
    {
        this.playerContext = playerContext;
    }

    private void Awake()
    {
        foreach (SkillSO skill in allSkillSOs)
        {
            if (!skillDatabase.ContainsKey(skill.name))
            {
                skillDatabase.Add(skill.name, skill);
            }
        }
    }

    private void OnEnable()
    {
        SaveManager.Instance.RegisterSavable(this);

        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelUp += GetSkillPoint;
        GameEventsManager.Instance.EnemyEvents.OnCalculateSkillDamage += AddSkillDamage;
    }

    private void OnDisable()
    {
        SaveManager.Instance.UnregisterSavable(this);

        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelUp -= GetSkillPoint;
        GameEventsManager.Instance.EnemyEvents.OnCalculateSkillDamage -= AddSkillDamage;
    }

    public void AssignSkillToKey(SkillSO skill, string key)
    {
        skillBindings[key] = skill;
    }

    public void UnassignSkillFromKey(string key)
    {
        if (skillBindings.ContainsKey(key))
        {
            skillBindings.Remove(key);
        }
    }

    public SkillUseResult UseSkill(string key)
    {
        key = key.Replace("/Keyboard/", "");

        if (!skillBindings.TryGetValue(key, out SkillSO skill))
        {
            return SkillUseResult.NotAssigned;
        }

        if (IsSkillOnCooldown(skill))
        {
            return SkillUseResult.OnCooldown;
        }

        if (playerContext.Stat.Stats.Mana.CurrentValue < skill.RequiredMana)
        {
            return SkillUseResult.NotEnoughMana;
        }

        if (skillBindings.ContainsKey(key))
        {
            playerContext.SetCurrentSkill(skill);
            playerContext.Stat.Stats.Mana.CurrentValue -= skill.RequiredMana;
            StartCooldown(skill);

            GameEventsManager.Instance.UIEvents.SkillCooldownStarted(skill, skill.CooldownTime);

            return SkillUseResult.Success;
        }

        return SkillUseResult.NotAssigned;
    }

    public void LearnSkill(SkillSO skill)
    {
        if (!learnedSkills.ContainsKey(skill.name))
        {
            learnedSkills[skill.name] = skill;
            skillPoint -= 1;
        }
    }

    public void LevelUpSkill(SkillSO skill)
    {
        // TODO: 스킬 레벨업 기능 구현 예정
    }

    public bool HasLearnedSkill(SkillSO skill)
    {
        return learnedSkills.ContainsKey(skill.name);
    }

    public bool GetAvailableSkillPoints()
    {
        return skillPoint > 0;
    }

    public bool CanLearnSkill(SkillSO skill)
    {
        return skill.CanLearn(this);
    }

    public void GetSkillPoint(int _)
    {
        skillPoint += 1;
    }

    public int GetSkillPoint()
    {
        return skillPoint;
    }

    public bool IsSkillOnCooldown(SkillSO skill)
    {
        if (cooldownTracker.TryGetValue(skill, out float endTime))
        {
            return Time.time < endTime;
        }
        return false;
    }

    public void StartCooldown(SkillSO skill)
    {
        if (skill.CooldownTime > 0)
        {
            cooldownTracker[skill] = Time.time + skill.CooldownTime;
        }
    }

    private int AddSkillDamage(SkillSO skill, int currentDamage)
    {
        if (learnedSkills.TryGetValue(skill.name, out SkillSO learnedskill))
        {
            return currentDamage + learnedskill.SkillPower;
        }

        return currentDamage;
    }

    public void SaveData(GameSaveData data)
    {
        if (data.PlayerData.SkillData == null)
        {
            data.PlayerData.SkillData = new SkillSaveData();
        }

        data.PlayerData.SkillData.SkillPoint = this.skillPoint;

        data.PlayerData.SkillData.LearnedSkillIDs.Clear();
        foreach (var skill in learnedSkills.Values)
        {
            data.PlayerData.SkillData.LearnedSkillIDs.Add(skill.name);
        }

        data.PlayerData.SkillData.SkillBindings.Clear();
        foreach (var binding in skillBindings)
        {
            data.PlayerData.SkillData.SkillBindings.Add(new SkillBindingData
            {
                Key = binding.Key,
                SkillId = binding.Value.name
            });
        }
    }

    public void LoadData(GameSaveData data)
    {
        var skillData = data.PlayerData.SkillData;
        if (skillData == null) return;

        this.skillPoint = skillData.SkillPoint;

        learnedSkills.Clear();
        foreach (string skillId in skillData.LearnedSkillIDs)
        {
            SkillSO skill = GetSkill(skillId);
            if (skill != null)
            {
                learnedSkills[skillId] = skill;
            }
        }

        skillBindings.Clear();
        foreach (var bindingData in skillData.SkillBindings)
        {
            SkillSO skill = GetSkill(bindingData.SkillId);
            if (skill != null)
            {
                skillBindings[bindingData.Key] = skill;
            }
        }

        GameEventsManager.Instance.UIEvents.ReloadSkillData();
    }

    public SkillSO GetSkill(string skillId)
    {
        if (skillDatabase.TryGetValue(skillId, out SkillSO skill))
        {
            return skill;
        }
        Debug.LogWarning($"데이터베이스에서 스킬을 찾을 수 없습니다: {skillId}");
        return null;
    }

    public SkillSO GetSkillForKey(string key)
    {
        if (skillBindings.TryGetValue(key, out SkillSO skill))
        {
            return skill;
        }
        else return null;
    }
}