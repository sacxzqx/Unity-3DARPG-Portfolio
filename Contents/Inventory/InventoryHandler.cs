using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����, ������ ���/�Ǹ�/������ ó�� �� �κ��丮 ���� ���ͷ����� �����ϴ� Ŭ����
/// </summary>
public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private UIPopup TextPopup;

    private Dictionary<EquipType, Slot> equippedSlots = new Dictionary<EquipType, Slot>();

    public bool IsSellMode = false;

    [SerializeField] private GameObject UsePopup; // ������ ��� ��ư �г�
    [SerializeField] private GameObject SellPopup; // ������ �Ǹ� ��ư �г�
    [SerializeField] private UIPopup warningPopup;

    public Slot CurrentSelectedSlot;

    private void OnEnable()
    {
        GameEventsManager.Instance.ShopEvents.OnSellMode += EnableSellMode;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.ShopEvents.OnSellMode -= EnableSellMode;
    }

    public void SetCurrentSlot(Slot slot)
    {
        CurrentSelectedSlot = slot;
    }

    /// <summary>
    /// ������ ��� ��ư���� ȣ��� (�ν����Ϳ��� �����)
    /// </summary>
    public void ActivateCurrentItem()
    {
        if (CurrentSelectedSlot != null && CurrentSelectedSlot.slotData.Item != null)
        {
            if (CurrentSelectedSlot.slotData.Item is EquipmentSO)
            {
                Equip(CurrentSelectedSlot);
            }
            else
            {
                GameEventsManager.Instance.ItemEvents.UseItem(CurrentSelectedSlot.slotData.Item);
                CurrentSelectedSlot.ParentInventory.RemoveItem(CurrentSelectedSlot);
            }

            UsePopup.SetActive(false);
        }
    }

    /// <summary>
    /// InventoryUI���� �޾ƿ� �ε�ÿ� ȣ��Ǿ� ��� ���� UI�� �����ϴ� �Լ�
    /// </summary>
    public void RestoreEquippedState(Slot slot)
    {
        Debug.Log("�κ� ����");
        if (slot.slotData.Item is not EquipmentSO equipment) return;

        Equip(slot);
    }

    public void ClearEquippedSlots()
    {
        equippedSlots.Clear();
    }

    /// <summary>
    /// ������ �Ǹ� ��ư���� ȣ��� (�ν����Ϳ��� �����)
    /// </summary>
    public void SellCurrentItem()
    {
        if (CurrentSelectedSlot != null && CurrentSelectedSlot.slotData.Item != null && CurrentSelectedSlot.slotData.IsSellable && !CurrentSelectedSlot.slotData.IsEquipped)
        {
            GameEventsManager.Instance.ItemEvents.SellItem(CurrentSelectedSlot.slotData.Item);
            AudioManager.Instance.PlaySFX("Sell");
            CurrentSelectedSlot.ParentInventory.RemoveItem(CurrentSelectedSlot);
            SellPopup.SetActive(false);
        }
        else
        {
            UIManager.Instance.OpenPopupWithFade(warningPopup);
            SellPopup.SetActive(false);
        }
    }

    /// <summary>
    /// ������ ���� ��ư���� ȣ��� (�ν����Ϳ��� �����)
    /// </summary>
    public void DiscardCurrentItem()
    {
        if (CurrentSelectedSlot != null && CurrentSelectedSlot.slotData.Item != null)
        {
            CurrentSelectedSlot.ParentInventory.RemoveItem(CurrentSelectedSlot);
            UsePopup.SetActive(false);
        }
    }

    /// <summary>
    /// �ش� ������ ���� ��� �����ϰų�, ���� ��� ��ü
    /// ���� ��� �̹��� �� isUsed ���µ� �Բ� �����
    /// </summary>
    private void Equip(Slot newSlot)
    {
        if(newSlot.slotData.Item is not EquipmentSO equipmentToHandle) return;

        if (equippedSlots.TryGetValue(equipmentToHandle.EquipType, out Slot oldEquippedSlot))
        {
            // �̹� ������ �������� �����ϴ� ��� (���� ���� Ŭ��)
            if (oldEquippedSlot == newSlot)
            {
                GameEventsManager.Instance.ItemEvents.UnequipItem(equipmentToHandle);
                oldEquippedSlot.slotData.IsEquipped = false;
                oldEquippedSlot.SetBackgroundImage("Empty_Image");
                equippedSlots.Remove(equipmentToHandle.EquipType);
                return; // ���� ������ �ϰ� �Լ� ����
            }

            // �������� ��ü�ϴ� ��� (�ٸ� ���� Ŭ��)
            if (oldEquippedSlot.slotData.Item is EquipmentSO oldEquipmentToUnequip)
            {
                GameEventsManager.Instance.ItemEvents.UnequipItem(oldEquipmentToUnequip);
            }

            // ���� ������ UI�� ������Ʈ
            oldEquippedSlot.slotData.IsEquipped = false;
            oldEquippedSlot.SetBackgroundImage("Empty_Image");
        }

        GameEventsManager.Instance.ItemEvents.UseItem(equipmentToHandle);
        newSlot.slotData.IsEquipped = true;
        newSlot.SetBackgroundImage("Equipped_Image");
        equippedSlots[equipmentToHandle.EquipType] = newSlot;
    }

    /// <summary>
    /// ���� �������� ��� ���� ���¸� ���� ������
    /// ������ isUsed ������ ��츸 ��ȿ�ϰ� �۵���
    /// </summary>
    public void SwapEquippedSlot(Slot slot)
    {
        if (!slot.slotData.IsEquipped) return; // ���� ���°� �ƴϸ� ����

        if (slot.slotData.Item is not EquipmentSO equipment)
        {
            Debug.LogWarning("[SwapEquipSlot] ���Կ� ��� �������� �ƴ�");
            return;
        }

        equippedSlots[equipment.EquipType] = slot;
    }

    private void EnableSellMode()
    {
        IsSellMode = true;
    }
}