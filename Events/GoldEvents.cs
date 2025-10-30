using System;

/// <summary>
/// ��� ȹ�� �� ��ȭ�� ���� �̺�Ʈ�� �����ϴ� Ŭ����
/// </summary>
public class GoldEvents
{
    public event Action<int> OnGoldGained;
    public void GoldGained(int gold)
    {
        if (OnGoldGained != null)
        {
            OnGoldGained?.Invoke(gold);
            GoldChange(GoldManager.Instance.CurrentGold);
        }
    }

    public event Action<int> OnGoldChange;
    public void GoldChange(int gold)
    {
        if (OnGoldChange != null)
        {
            OnGoldChange?.Invoke(gold);
        }
    }
}
