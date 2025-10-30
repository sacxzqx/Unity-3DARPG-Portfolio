using System;
using System.Diagnostics;

/// <summary>
/// UI �Է°� ���õ� �̺�Ʈ���� ó���ϴ� Ŭ����
/// �� UI â�� ���, ���� ���� ���� �߾� ���߽����� ����
/// </summary>
public class InputEvents
{
    /// <summary>
    /// ���� �����ִ� ����� UI�� �ݱ�/���� �׼��� ����
    /// �ٸ� UI�� ������ �� �� �� �׼��� ȣ���Ͽ� ���� UI�� �ݴ� �� ���
    /// null�̸� �����ִ� ����� UI�� ������ �ǹ�
    /// </summary>
    private Action lastToggleAction = null;

    /// <summary>
    /// ���� ���� �ִ� UI�� �ִٸ� ������ ����
    /// </summary>
    public void ForceCloseActiveUI()
    {
        if (lastToggleAction != null)
        {
            lastToggleAction.Invoke();
            lastToggleAction = null;
            UIStateChange(UIType.None, false); // UI�� ���� �����ٰ� �˸�
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
    /// ���� UI ���� ���� �̺�Ʈ
    /// ����/���� ���θ� �� UI�� ����
    /// HUD, Map UI�� �������� ��� 
    /// </summary>
    public event Action<UIType, bool> OnUIStateChange;

    public void UIStateChange(UIType type, bool isOpen)
    {
        OnUIStateChange?.Invoke(type, isOpen);
    }

    /// <summary>
    /// UI ��� ��û�� ó���ϴ� �߾� �Լ�
    /// �� ���� �ϳ��� ����� UI�� ���� �ֵ��� ����
    /// </summary>
    /// <param name="type">���ų� �������� UI�� Ÿ�� (<see cref="UIType"/> enum)</param>
    /// <param name="toggleAction">�ش� UI�� ������ ���� �ݴ� ������ ��� �׼�</param>
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
    /// UI�� ��ü������ ������ ��(��: UI ���� 'Exit' ��ư Ŭ��) ȣ��Ǿ�,
    /// InputEvents �ý����� ���¸� ����ȭ
    /// �� �޼��带 ȣ������ ������, �ý����� UI�� ������ �����ִٰ� ������ �� ����
    /// </summary>
    public void NotifyUIClosed()
    {
        // ���� �ݱ� ���� ���(��: UI �� Exit ��ư)���� UI�� ������ �� ȣ��
        lastToggleAction = null;
        UIStateChange(UIType.None, false);
    }
}