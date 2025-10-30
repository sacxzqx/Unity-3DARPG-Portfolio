using System;

/// <summary>
/// 상점 관련 UI 이벤트를 관리하는 클래스
/// 상점 초기화, 열기/닫기, 판매 모드 전환 등의 이벤트를 포함함
/// </summary>
public class ShopEvents
{
    /// <summary>
    /// 상점이 초기화될 때 호출되는 이벤트
    /// 초기화 시 상점에 표시될 아이템 목록을 전달함
    /// </summary>
    public event Action<ItemSO[]> OnShopInitialize;
    public void InitializeShop(ItemSO[] items)
    {
        OnShopInitialize?.Invoke(items);
    }

    public event Action OnShopOpen;
    public void OpenShop()
    {
        OnShopOpen?.Invoke();
    }

    public event Action OnShopClose;
    public void CloseShop()
    {
        OnShopClose?.Invoke();
    }

    /// <summary>
    /// 상점이 열릴 때, Inventory Handler에서 판매 모드를 활성화하기 위해 호출되는 이벤트
    /// 플레이어가 아이템을 판매할 수 있는 상태로 전환됨
    /// </summary>
    public event Action OnSellMode;
    public void EnableSellMode()
    {
        OnSellMode?.Invoke();
    }
}
