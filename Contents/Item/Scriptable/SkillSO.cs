using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

/// <summary>
/// ��ų Ʈ���� ���Ǵ� ���� ��ų �����͸� �����ϴ� ScriptableObject
/// �̸�, ����, ������, �ִϸ��̼�, ����Ʈ ����, �䱸 ����, ���� ��ų ���� ����
/// </summary>
[CreateAssetMenu(fileName = "newSkill", menuName = "Skill Tree/Skill")]
public class SkillSO : ScriptableObject
{
    [Header("�⺻ ����")]
    public LocalizedString SkillName;
    public LocalizedString Description;
    public SkillType Type;
    public StatType TargetStat;
    public Sprite Icon;

    [Header("�䱸 ����")]
    public int RequiredLevel;
    public int RequiredMana = 10;
    public List<SkillSO> Prerequisites = new List<SkillSO>();

    [Header("��ų ȿ��")]
    public int SkillPower;
    public float CooldownTime = 5f;
    public float BuffDuration = 20f;
    public string AnimationTrigger;
    public string ParticleName;
    public GameObject EffectPrefab;

    [Header("����Ʈ ��ġ ������")]
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