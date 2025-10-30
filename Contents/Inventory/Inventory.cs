using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾��� �κ��丮�� �����ϴ� Ŭ����
/// �������� �߰�, ����, ���� ���� ���� ó���ϸ�
/// �̺�Ʈ �ý��۰� �����Ǿ� �ܺ� �ý��۰� ��ȣ�ۿ� ����
/// </summary>
public class Inventory
{
    public int MaxSlotCount { get; private set; }
    public List<SlotData> Slots = new List<SlotData>();

    public event Action<SlotData> OnItemAdded;
    public event Action<SlotData> OnItemRemoved;
    public event Action<SlotData> OnSlotItemRemoved;

    /// <summary>
    /// ���ο� �κ��丮�� ����
    /// </summary>
    /// <param name="slotCount">�� �κ��丮�� ���� ������ �ִ� ����</param>
    public Inventory(int slotCount)
    {
        MaxSlotCount = slotCount;

        for (int i = 0; i < MaxSlotCount; i++)
        {
            Slots.Add(new SlotData());
        }
    }

    /// <summary>
    /// �������� �κ��丮�� �߰�
    /// </summary>
    /// <param name="item">�߰��� ������</param>
    /// <param name="quantity">����</param>
    public void AddItem(ItemSO item, int quantity)
    {
        SlotData changedSlotData = null;

        // 1. ���� ���Կ� �������� �ִ��� ���� ã�Ƽ� ������ �ø�
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
            // 2. ���� ���� ���Կ� �����ٸ�, �� ������ ã�� ���� �߰�
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
            // 3. ���� �Ұ����� ��� (���� ���Կ� �ϳ���)
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
                    Debug.LogError("�� ������ �����Ͽ� �Ϻ� �������� �߰����� ���߽��ϴ�.");
                    break;
                }
            }
        }
    }

    /// <summary>
    /// �������� �κ��丮 ���Կ��� ���� ����(���)
    /// </summary>
    /// <param name="item">����(���)�� ����</param>
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
    /// Ư�� �������� �κ��丮���� ã�� ����
    /// </summary>
    /// <param name="item">������ ������</param>
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
            Debug.LogWarning($"[Inventory] {item.ItemName}��(��) ���� �������� ���߽��ϴ�. ���� ����: {quantity}");
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
    /// Ư�� �������� ���� ������ ��ȯ
    /// </summary>
    /// <param name="item">��ȸ�� ������</param>
    /// <returns>������ ����</returns>
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
    /// �κ��丮�� ��� ������ �����ϰ� ��� (�ε� �� ���)
    /// </summary>
    public void ClearAllSlots()
    {
        foreach (var slot in Slots)
        {
            slot.Clear();
        }
    }
}
