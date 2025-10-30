using System;

/// <summary>
/// 골드 획득 및 변화에 대한 이벤트를 관리하는 클래스
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
