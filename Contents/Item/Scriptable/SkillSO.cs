using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

/// <summary>
/// 스킬 트리에 사용되는 개별 스킬 데이터를 정의하는 ScriptableObject
/// 이름, 설명, 아이콘, 애니메이션, 이펙트 정보, 요구 조건, 선행 스킬 등을 포함
/// </summary>
[CreateAssetMenu(fileName = "newSkill", menuName = "Skill Tree/Skill")]
public class SkillSO : ScriptableObject
{
    [Header("기본 정보")]
    public LocalizedString SkillName;
    public LocalizedString Description;
    public SkillType Type;
    public StatType TargetStat;
    public Sprite Icon;

    [Header("요구 조건")]
    public int RequiredLevel;
    public int RequiredMana = 10;
    public List<SkillSO> Prerequisites = new List<SkillSO>();

    [Header("스킬 효과")]
    public int SkillPower;
    public float CooldownTime = 5f;
    public float BuffDuration = 20f;
    public string AnimationTrigger;
    public string ParticleName;
    public GameObject EffectPrefab;

    [Header("이펙트 위치 오프셋")]
    public Vector3 OffsetPosition = Vector3.zero;
    public Vector3 OffsetRotation = Vector3.zero;

    public bool CanLearn(SkillManager skillManager)
    {
        foreach (var prerequisite in Prerequisites)
        {
            if (!skillManager.HasLearnedSkill(prerequisite))
                return false;
        }
        return true;
    }
}