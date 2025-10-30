using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 인벤토리를 관리하는 클래스
/// 아이템의 추가, 제거, 장착 상태 등을 처리하며
/// 이벤트 시스템과 연동되어 외부 시스템과 상호작용 가능
/// </summary>
public class Inventory
{
    public int MaxSlotCount { get; private set; }
    public List<SlotData> Slots = new List<SlotData>();

    public event Action<SlotData> OnItemAdded;
    public event Action<SlotData> OnItemRemoved;
    public event Action<SlotData> OnSlotItemRemoved;

    /// <summary>
    /// 새로운 인벤토리를 생성
    /// </summary>
    /// <param name="slotCount">이 인벤토리가 가질 슬롯의 최대 개수</param>
    public Inventory(int slotCount)
    {
        MaxSlotCount = slotCount;

        for (int i = 0; i < MaxSlotCount; i++)
        {
            Slots.Add(new SlotData());
        }
    }

    /// <summary>
    /// 아이템을 인벤토리에 추가
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    /// <param name="quantity">개수</param>
    public void AddItem(ItemSO item, int quantity)
    {
        SlotData changedSlotData = null;

        // 1. 기존 슬롯에 아이템이 있는지 먼저 찾아서 개수를 늘림
        if (item.Stackable)
        {
            {
                foreach (var slot in Slots)
                {
                    if (slot.Item == item)
                    {
                        slot.Count += quantity;
                        changedSlotData = slot;
                        OnItemAdded?.Invoke(changedSlotData);
                        return;
                    }
                }
            }
            // 2. 만약 기존 슬롯에 없었다면, 빈 슬롯을 찾아 새로 추가
            foreach (var slot in Slots)
            {
                if (slot.IsEmpty)
                {
                    slot.Item = item;
                    slot.Count = quantity;
                    changedSlotData = slot;
                    OnItemAdded?.Invoke(changedSlotData);
                    return;
                }
            }
        }
        else
        {
            // 3. 스택 불가능한 경우 (개별 슬롯에 하나씩)
            for (int i = 0; i < quantity; i++)
            {
                bool added = false;
                SlotData currentSlot = null;

                foreach (var slot in Slots)
                {
                    if (slot.IsEmpty)
                    {
                        slot.Item = item;
                        slot.Count = 1;
                        currentSlot = slot;
                        added = true;
                        break;
                    }
                }
                if (added)
                {
                    OnItemAdded?.Invoke(currentSlot);
                }
                else
                {
                    Debug.LogError("빈 슬롯이 부족하여 일부 아이템을 추가하지 못했습니다.");
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 아이템을 인벤토리 슬롯에서 직접 제거(사용)
    /// </summary>
    /// <param name="item">제거(사용)할 슬롯</param>
    public void RemoveItem(Slot slot)
    {
        if (slot == null || slot.slotData.Item == null) return;

        var removedSlotData = slot.slotData;

        if (removedSlotData.Item.Stackable)
        {
            removedSlotData.Count--;

            if (removedSlotData.Count <= 0)
            {
                OnSlotItemRemoved?.Invoke(removedSlotData);
                removedSlotData.Clear();
            }

            OnItemRemoved?.Invoke(removedSlotData);
        }
        else
        {
            OnSlotItemRemoved?.Invoke(removedSlotData);
            removedSlotData.Clear();

            OnItemRemoved?.Invoke(removedSlotData);
        }
    }

    /// <summary>
    /// 특정 아이템을 인벤토리에서 찾아 제거
    /// </summary>
    /// <param name="item">제거할 아이템</param>
    public void RemoveItem(ItemSO item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return;

        for (int i = 0; i < Slots.Count; i++)
        {
            var slot = Slots[i];

            if (slot.Item == item)
            {
                if (item.Stackable)
                {
                    int removeAmount = Mathf.Min(slot.Count, quantity);
                    slot.Count -= removeAmount;
                    quantity -= removeAmount;

                    OnItemRemoved?.Invoke(slot);

                    if (slot.Count <= 0) slot.Clear();
                }
                else
                {
                    if (quantity > 0)
                    {
                        slot.Clear();
                        quantity--;

                        OnItemRemoved?.Invoke(slot);
                    }
                }

                if (quantity <= 0) break;
            }
        }

        if (quantity > 0)
        {
            Debug.LogWarning($"[Inventory] {item.ItemName}을(를) 전부 제거하지 못했습니다. 남은 개수: {quantity}");
        }
    }

    public void SwapSlotData(SlotData dataA, SlotData dataB)
    {
        ItemSO tempItem = dataA.Item;
        int tempCount = dataA.Count;
        bool tempEquipped = dataA.IsEquipped;

        dataA.Item = dataB.Item;
        dataA.Count = dataB.Count;
        dataA.IsEquipped = dataB.IsEquipped;

        dataB.Item = tempItem;
        dataB.Count = tempCount;
        dataB.IsEquipped = tempEquipped;

        GameEventsManager.Instance.ItemEvents.SwapItem(dataA, dataB);
    }

    /// <summary>
    /// 특정 아이템의 보유 개수를 반환
    /// </summary>
    /// <param name="item">조회할 아이템</param>
    /// <returns>아이템 개수</returns>
    public int GetNumberOfItem(ItemSO item)
    {
        if (item == null) return 0;

        int total = 0;
        foreach (var slot in Slots)
        {
            if (slot.Item == item)
            {
                total += slot.Count;
            }
        }

        return total;
    }

    public void SetMaxSlotCount(int value)
    {
        MaxSlotCount = value;
    }

    /// <summary>
    /// 인벤토리의 모든 슬롯을 깨끗하게 비움 (로드 시 사용)
    /// </summary>
    public void ClearAllSlots()
    {
        foreach (var slot in Slots)
        {
            slot.Clear();
        }
    }
}
