using System;

/// <summary>
/// ���� ���� UI �̺�Ʈ�� �����ϴ� Ŭ����
/// ���� �ʱ�ȭ, ����/�ݱ�, �Ǹ� ��� ��ȯ ���� �̺�Ʈ�� ������
/// </summary>
public class ShopEvents
{
    /// <summary>
    /// ������ �ʱ�ȭ�� �� ȣ��Ǵ� �̺�Ʈ
    /// �ʱ�ȭ �� ������ ǥ�õ� ������ ����� ������
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
    /// ������ ���� ��, Inventory Handler���� �Ǹ� ��带 Ȱ��ȭ�ϱ� ���� ȣ��Ǵ� �̺�Ʈ
    /// �÷��̾ �������� �Ǹ��� �� �ִ� ���·� ��ȯ��
    /// </summary>
    public event Action OnSellMode;
    public void EnableSellMode()
    {
        OnSellMode?.Invoke();
    }
}
