using System;
using UnityEngine;

/// <summary>
/// 아이템 관련 이벤트를 관리하는 클래스
/// 인벤토리 추가, 제거, 사용, 판매 등의 액션을 처리
/// </summary>
public class ItemEvents
{
    public event Action<string> OnItemAdded;
    public void AddItem(string itemName)
    {
        OnItemAdded?.Invoke(itemName);
    }

    public event Action<ItemSO, Inventory> OnItemRemove;
    public void RemoveItem(ItemSO item, Inventory inventory)
    {
        OnItemRemove?.Invoke(item, inventory);
    }

    public event Action<ItemSO> OnItemSell;
    public void SellItem(ItemSO item)
    {
        OnItemSell?.Invoke(item);
    }

    public event Action<ItemSO> OnItemUsed;
    public void UseItem(ItemSO item)
    {
        OnItemUsed?.Invoke(item);
    }

    public event Action<EquipmentSO, Slot> OnEquipItem;
    public void EquipItem(EquipmentSO equipment, Slot slot)
    {
        OnEquipItem?.Invoke(equipment, slot);
    }

    public Action<EquipmentSO> OnUnequipItem;
    public void UnequipItem(EquipmentSO equipment)
    {
        OnUnequipItem?.Invoke(equipment);
    }

    /// <summary>
    /// 아이템 획득 이후 HUD UI와 연동을 위해 호출됨
    /// </summary>
    public event Action<ItemSO> OnItemGetDisplay;
    public void GetItemDisplay(ItemSO item)
    {
        if (OnItemGetDisplay != null)
        {
            OnItemGetDisplay(item);
        }
    }

    public event Action<SlotData, SlotData> onItemSwapped;
    public void SwapItem(SlotData slotA, SlotData slotB)
    {
        onItemSwapped?.Invoke(slotA, slotB);
    }

    public event Action OnInventoryReloaded;
    public void InventoryReloaded()
    {
        OnInventoryReloaded?.Invoke();
    }

    /// <summary>
    /// 특정 아이템이 인벤토리에 몇 개 있는지 가져올 때 호출됨
    /// 여러 인벤토리에서 동시에 값을 조회하여 합산
    /// </summary>
    public event Func<ItemSO, int> OnItemNumberGetFromInventory;
    public int GetItemNumberFromInventory(ItemSO item)
    {
        var subscribers = OnItemNumberGetFromInventory;

        if (subscribers == null)
        {
            Debug.Log("구독자 없음");
            return 0;
        }

        int totalCount = 0;

        // GetInvocationList()를 통해 등록된 모든 메서드를 순회
        foreach (Func<ItemSO, int> func in subscribers.GetInvocationList())
        {
            try
            {
                totalCount += func.Invoke(item);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ItemEvents] Inventory 구독자 호출 오류: {e.Message}");
            }
        }
        return totalCount;
    }
}