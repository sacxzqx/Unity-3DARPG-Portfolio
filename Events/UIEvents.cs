using System;

/// <summary>
/// ���� �÷��� �� HUD�� �ݿ��Ǿ�� �� �̺�Ʈ���� �����ϴ� Ŭ����
/// ��ų ��Ű ���, ��ٿ� ����, ���� �޽��� ���, ������ ȹ�� ǥ�� �� HUD ��� ���ſ� Ưȭ
/// </summary>
public class UIEvents
{
    public event Action<string, SkillSO> OnSkillAssignedToKey;
    public void SkillAssigned(string key, SkillSO skill)
    {
        OnSkillAssignedToKey?.Invoke(key, skill);
    }

    public event Action<SkillSO, float> OnSkillCooldownStarted;
    public void SkillCooldownStarted(SkillSO skill, float duration)
    {
        OnSkillCooldownStarted?.Invoke(skill, duration);
    }

    public event Action<SkillUseResult> OnSkillFailureDisplay;
    public void SkillFailureDisplay(SkillUseResult result)
    {
        OnSkillFailureDisplay?.Invoke(result);
    }

    /// <summary>
    /// �������� �߰ߵǾ��ų�, �������� �� ȣ���.
    /// </summary>
    public event Action<ItemSO, bool> OnItemDiscoverd;
    public void DiscoverItem(ItemSO item, bool isActive)
    {
        if (OnItemDiscoverd != null)
        {
            OnItemDiscoverd(item, isActive);
        }
    }

    public event Action OnUIClosed;
    public void CloseUI()
    {
        OnUIClosed?.Invoke();
    }

    public event Action OnSkillDataReloaded;
    public void ReloadSkillData()
    {
        OnSkillDataReloaded?.Invoke();
    }
}
