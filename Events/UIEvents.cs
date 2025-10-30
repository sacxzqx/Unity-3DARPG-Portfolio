using System;

/// <summary>
/// 게임 플레이 중 HUD에 반영되어야 할 이벤트들을 관리하는 클래스
/// 스킬 핫키 등록, 쿨다운 시작, 실패 메시지 출력, 아이템 획득 표시 등 HUD 요소 갱신에 특화
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
    /// 아이템이 발견되었거나, 감춰졌을 때 호출됨.
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
