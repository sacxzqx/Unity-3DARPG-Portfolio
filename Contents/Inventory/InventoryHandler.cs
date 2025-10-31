using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 슬롯 선택, 아이템 사용/판매/버리기 처리 등 인벤토리 내부 인터랙션을 제어하는 클래스
/// </summary>
public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private UIPopup TextPopup;

    private Dictionary<EquipType, Slot> equippedSlots = new Dictionary<EquipType, Slot>();

    public bool IsSellMode = false;

    [SerializeField] private GameObject UsePopup; // 아이템 사용 버튼 패널
    [SerializeField] private GameObject SellPopup; // 아이템 판매 버튼 패널
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
    /// 아이템 사용 버튼에서 호출됨 (인스펙터에서 연결됨)
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
    /// InventoryUI에서 받아와 로드시에 호출되어 장비 장착 UI를 갱신하는 함수
    /// </summary>
    public void RestoreEquippedState(Slot slot)
    {
        Debug.Log("인벤 복구");
        if (slot.slotData.Item is not EquipmentSO equipment) return;

        Equip(slot);
    }

    public void ClearEquippedSlots()
    {
        equippedSlots.Clear();
    }

    /// <summary>
    /// 아이템 판매 버튼에서 호출됨 (인스펙터에서 연결됨)
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
    /// 아이템 삭제 버튼에서 호출됨 (인스펙터에서 연결됨)
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
    /// 해당 슬롯을 통해 장비를 장착하거나, 기존 장비를 교체
    /// 슬롯 배경 이미지 및 isUsed 상태도 함께 제어됨
    /// </summary>
    private void Equip(Slot newSlot)
    {
        if(newSlot.slotData.Item is not EquipmentSO equipmentToHandle) return;

        if (equippedSlots.TryGetValue(equipmentToHandle.EquipType, out Slot oldEquippedSlot))
        {
            // 이미 장착된 아이템을 해제하는 경우 (같은 슬롯 클릭)
            if (oldEquippedSlot == newSlot)
            {
                GameEventsManager.Instance.ItemEvents.UnequipItem(equipmentToHandle);
                oldEquippedSlot.slotData.IsEquipped = false;
                oldEquippedSlot.SetBackgroundImage("Empty_Image");
                equippedSlots.Remove(equipmentToHandle.EquipType);
                return; // 장착 해제만 하고 함수 종료
            }

            // 아이템을 교체하는 경우 (다른 슬롯 클릭)
            if (oldEquippedSlot.slotData.Item is EquipmentSO oldEquipmentToUnequip)
            {
                GameEventsManager.Instance.ItemEvents.UnequipItem(oldEquipmentToUnequip);
            }

            // 기존 슬롯의 UI를 업데이트
            oldEquippedSlot.slotData.IsEquipped = false;
            oldEquippedSlot.SetBackgroundImage("Empty_Image");
        }

        GameEventsManager.Instance.ItemEvents.UseItem(equipmentToHandle);
        newSlot.slotData.IsEquipped = true;
        newSlot.SetBackgroundImage("Equipped_Image");
        equippedSlots[equipmentToHandle.EquipType] = newSlot;
    }

    /// <summary>
    /// 슬롯 기준으로 장비 장착 상태를 강제 재지정
    /// 슬롯이 isUsed 상태인 경우만 유효하게 작동함
    /// </summary>
    public void SwapEquippedSlot(Slot slot)
    {
        if (!slot.slotData.IsEquipped) return; // 장착 상태가 아니면 무시

        if (slot.slotData.Item is not EquipmentSO equipment)
        {
            Debug.LogWarning("[SwapEquipSlot] 슬롯에 장비 아이템이 아님");
            return;
        }

        equippedSlots[equipment.EquipType] = slot;
    }

    private void EnableSellMode()
    {
        IsSellMode = true;
    }
}