using System;
using System.Diagnostics;

/// <summary>
/// UI 입력과 관련된 이벤트들을 처리하는 클래스
/// 각 UI 창의 토글, 상태 변경 등을 중앙 집중식으로 관리
/// </summary>
public class InputEvents
{
    /// <summary>
    /// 현재 열려있는 토글형 UI의 닫기/열기 액션을 저장
    /// 다른 UI를 열려고 할 때 이 액션을 호출하여 기존 UI를 닫는 데 사용
    /// null이면 열려있는 토글형 UI가 없음을 의미
    /// </summary>
    private Action lastToggleAction = null;

    /// <summary>
    /// 현재 열려 있는 UI가 있다면 강제로 닫음
    /// </summary>
    public void ForceCloseActiveUI()
    {
        if (lastToggleAction != null)
        {
            lastToggleAction.Invoke();
            lastToggleAction = null;
            UIStateChange(UIType.None, false); // UI가 전부 닫혔다고 알림
        }
    }

    public event Action OnSubmitPressed;
    public void SubmitPressed()
    {
        OnSubmitPressed?.Invoke();
    }

    public event Action<bool> OnSetPlayerInput;
    public void SetPlayerInput(bool isEnabled)
    {
        OnSetPlayerInput!.Invoke(isEnabled);
    }

    public event Action OnSaveMenuRequested;
    public void OpenSaveMenu()
    {
        HandleToggle(UIType.None, OnSaveMenuRequested);
    }

    public event Action OnLoadMenuRequested;
    public void OpenLoadMenu()
    {
        HandleToggle(UIType.None, OnLoadMenuRequested);
    }

    public event Action OnPreferenceRequested;
    public void OpenPreference()
    {
        HandleToggle(UIType.None, OnPreferenceRequested);
    }

    public event Action OnQuestLogTogglePressed;
    public void QuestLogTogglePressed()
    {
        HandleToggle(UIType.None, OnQuestLogTogglePressed);
    }

    public event Action OnMenuTogglePressed;
    public void MenuTogglePressed()
    {
        HandleToggle(UIType.None, OnMenuTogglePressed);
    }

    public event Action OnInventoryTogglePressed;
    public void InventoryTogglePressed()
    {
        HandleToggle(UIType.Inventory, OnInventoryTogglePressed);
    }

    public event Action OnSkillTreeTogglePressed;
    public void SkillTreeTogglePressed()
    {
        HandleToggle(UIType.None, OnSkillTreeTogglePressed);
    }

    public event Action OnStatusTogglePressed;
    public void StatusTogglePressed()
    {
        HandleToggle(UIType.None, OnStatusTogglePressed);
    }

    public event Action OnShopTogglePressed;
    public void ShopTogglePressed()
    {
        HandleToggle(UIType.None, OnShopTogglePressed);
    }

    public event Action OnMapTogglePressed;
    public void MapTogglePressed()
    {
        HandleToggle(UIType.WorldMap, OnMapTogglePressed);
    }

    /// <summary>
    /// 공통 UI 상태 변경 이벤트
    /// 열림/닫힘 여부를 각 UI에 전달
    /// HUD, Map UI의 설정에도 사용 
    /// </summary>
    public event Action<UIType, bool> OnUIStateChange;

    public void UIStateChange(UIType type, bool isOpen)
    {
        OnUIStateChange?.Invoke(type, isOpen);
    }

    /// <summary>
    /// UI 토글 요청을 처리하는 중앙 함수
    /// 한 번에 하나의 토글형 UI만 열려 있도록 보장
    /// </summary>
    /// <param name="type">열거나 닫으려는 UI의 타입 (<see cref="UIType"/> enum)</param>
    /// <param name="toggleAction">해당 UI를 실제로 열고 닫는 로직이 담긴 액션</param>
    public void HandleToggle(UIType type, Action toggleAction)
    {
        if (lastToggleAction != null && lastToggleAction != toggleAction)
        {
            lastToggleAction.Invoke();
        }

        toggleAction?.Invoke();

        lastToggleAction = lastToggleAction == toggleAction ? null : toggleAction;

        UIStateChange(type, lastToggleAction != null);
    }

    /// <summary>
    /// UI가 자체적으로 닫혔을 때(예: UI 내의 'Exit' 버튼 클릭) 호출되어,
    /// InputEvents 시스템의 상태를 동기화
    /// 이 메서드를 호출하지 않으면, 시스템은 UI가 여전히 열려있다고 착각할 수 있음
    /// </summary>
    public void NotifyUIClosed()
    {
        // 강제 닫기 외의 방법(예: UI 내 Exit 버튼)으로 UI가 닫혔을 때 호출
        lastToggleAction = null;
        UIStateChange(UIType.None, false);
    }
}