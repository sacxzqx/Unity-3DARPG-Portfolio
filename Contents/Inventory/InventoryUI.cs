using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 인벤토리 UI를 담당하는 클래스
/// 버튼을 통해 장비/소비창 전환, 슬롯 초기화 및 UI 업데이트 처리
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Main UI Elements")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private TextMeshProUGUI playerGold;

    [Header("Inventory Tabs")]
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Button consumableButton;

    [SerializeField] private GameObject equipmentInventoryPanel;
    [SerializeField] private GameObject consumableInventoryPanel;

    [Header("Slots")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform consumableSlotParent;

    [Header("Popups & UI Elements")]
    [SerializeField] private UIPopup selectPopup;
    [SerializeField] private UIPopup sellPopup;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;

    private Inventory equipmentInventory;
    private Inventory consumableInventory;

    private List<Slot> equipmentSlots = new List<Slot>();
    private List<Slot> consumableSlots = new List<Slot>();

    private GameObject currentActiveInventoyPanel;
    private Image currentActiveImage;

    private InventoryHandler inventoryHandler;

    private void Awake()
    {
        inventoryHandler = GetComponent<InventoryHandler>();

        if (ItemManager.Instance != null)
        {
            this.equipmentInventory = ItemManager.Instance.equipmentInventory;
            this.consumableInventory = ItemManager.Instance.consumableInventory;
        }
        else
        {
            Debug.LogError("ItemManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    private void Start()
    {
        playerGold.text = GoldManager.Instance.CurrentGold.ToString();

        equipmentButton.onClick.AddListener(() => SetActiveInventory(equipmentButton.image, equipmentInventoryPanel));
        consumableButton.onClick.AddListener(() => SetActiveInventory(consumableButton.image, consumableInventoryPanel));

        SetActiveInventory(consumableButton.image, consumableInventoryPanel);

        InitializeSlots(consumableInventory, consumableSlots, consumableSlotParent);
        InitializeSlots(equipmentInventory, equipmentSlots, equipmentSlotParent);
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.InputEvents.OnInventoryTogglePressed += InventoryTogglePressed;
        GameEventsManager.Instance.GoldEvents.OnGoldChange += PlayerGoldChange;
        GameEventsManager.Instance.ItemEvents.onItemSwapped += UpdateSwappedUI;
        GameEventsManager.Instance.ItemEvents.OnInventoryReloaded += RedrawAllSlots;
        GameEventsManager.Instance.ItemEvents.OnInventoryReloaded += OnInventoryReloaded;

        equipmentInventory.OnItemAdded += UpdateUI;
        consumableInventory.OnItemAdded += UpdateUI;
        equipmentInventory.OnItemRemoved += UpdateRemovedUI;
        consumableInventory.OnItemRemoved += UpdateRemovedUI;
        equipmentInventory.OnSlotItemRemoved += ClearSlotUI;
        consumableInventory.OnSlotItemRemoved += ClearSlotUI;
    }

    void OnDisable()
    {
        GameEventsManager.Instance.InputEvents.OnInventoryTogglePressed -= InventoryTogglePressed;
        GameEventsManager.Instance.GoldEvents.OnGoldChange -= PlayerGoldChange;
        GameEventsManager.Instance.ItemEvents.onItemSwapped -= UpdateSwappedUI;
        GameEventsManager.Instance.ItemEvents.OnInventoryReloaded -= RedrawAllSlots;
        GameEventsManager.Instance.ItemEvents.OnInventoryReloaded -= OnInventoryReloaded;

        equipmentInventory.OnItemAdded -= UpdateUI;
        consumableInventory.OnItemAdded -= UpdateUI;
        equipmentInventory.OnItemRemoved -= UpdateRemovedUI;
        consumableInventory.OnItemRemoved -= UpdateRemovedUI;
        equipmentInventory.OnSlotItemRemoved -= ClearSlotUI;
        consumableInventory.OnSlotItemRemoved -= ClearSlotUI;
    }

    /// <summary>
    /// 인벤토리 데이터를 기반으로 UI 슬롯 프리팹을 생성하고 데이터를 연결
    /// </summary>
    /// <param name="inventory">슬롯 데이터를 담고 있는 Inventory 인스턴스</param>
    /// <param name="targetList">생성된 슬롯 UI를 저장할 리스트</param>
    /// <param name="parent">슬롯이 배치될 Transform 부모</param>
    public void InitializeSlots(Inventory inventory, List<Slot> targetList, Transform parent)
    {
        int count = inventory.MaxSlotCount;

        for (int i = 0; i < count; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, parent);
            Slot slot = slotObj.GetComponent<Slot>();

            SlotData dataToLink = inventory.Slots[i];
            
            slot.Initialize(selectPopup, sellPopup, itemName, itemDescription, inventory);

            slot.LinkData(dataToLink);
            slot.ClearIcon();
            targetList.Add(slot);
        }
    }

    private void InventoryTogglePressed()
    {
        if (contentParent.activeInHierarchy)
        {
            inventoryHandler.IsSellMode = false;
            HideUI();
        }
        else
        {
            ShowUI();
        }
    }

    private void ShowUI()
    {
        contentParent.SetActive(true);
    }

    private void HideUI()
    {
        selectPopup.Close();
        sellPopup.Close();
        contentParent.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void PlayerGoldChange(int gold)
    {
        // 현재 금액 텍스트에 대하여 DOTween 애니메이션 실행
        playerGold.DOKill();

        if (!int.TryParse(playerGold.text, out int currentGold))
        {
            playerGold.text = gold.ToString();
            return;
        }

        DOTween.To(
                () => currentGold,
                x => currentGold = x,
                gold,
                0.5f
            ).OnUpdate(() => {
                playerGold.text = currentGold.ToString();
            }).SetUpdate(true);
    }

    /// <summary>
    /// 장비/소비 인벤토리 탭 전환 처리
    /// </summary>
    public void SetActiveInventory(Image newImage, GameObject newPanel)
    {
        if (currentActiveImage != null)
        {
            ResetButtonStyle(currentActiveImage);
        }

        ApplyActiveStyle(newImage);
        currentActiveImage = newImage;

        if (currentActiveInventoyPanel != null)
        {
            currentActiveInventoyPanel.SetActive(false);
        }

        selectPopup.Close();
        newPanel.SetActive(true);
        currentActiveInventoyPanel = newPanel;
    }

    private void UpdateSwappedUI(SlotData dataA, SlotData dataB)
    {
        Slot slotControlA = FindSlotControlBySlotData(dataA);
        Slot slotControlB = FindSlotControlBySlotData(dataB);

        if (slotControlA == null || slotControlB == null)
        {
            Debug.LogError("교환된 슬롯의 UI 컨트롤을 찾을 수 없어 화면 갱신을 중단합니다.");
            return;
        }

        slotControlA.UpdateIcon(dataA.Item?.Sprite);
        slotControlA.UpdateCount(dataA.Count);
        slotControlA.UpdateSlotBackground();

        slotControlB.UpdateIcon(dataB.Item?.Sprite);
        slotControlB.UpdateCount(dataB.Count);
        slotControlB.UpdateSlotBackground();

        // A 슬롯의 아이템이 장비이며 장착 중인 경우
        if (dataA.IsEquipped && dataA.Item is EquipmentSO eqA)
        {
            // InventoryHandler에게 A 슬롯의 장착 상태를 강제로 재지정 요청
            inventoryHandler.SwapEquippedSlot(slotControlA);
        }

        // B 슬롯의 아이템이 장비이며 장착 중인 경우
        if (dataB.IsEquipped && dataB.Item is EquipmentSO eqB)
        {
            // InventoryHandler에게 B 슬롯의 장착 상태를 강제로 재지정 요청
            inventoryHandler.SwapEquippedSlot(slotControlB);
        }
    }

    private void ApplyActiveStyle(Image image)
    {
        image.color = Color.white;
    }

    private void ResetButtonStyle(Image image)
    {
        image.color = Color.gray;
    }

    private void UpdateUI(SlotData updatedSlotData)
    {
        Slot slotControl = FindSlotControlBySlotData(updatedSlotData);

        if (slotControl == null)
        {
            Debug.LogError("업데이트할 슬롯 UI 컨트롤을 찾지 못함.");
            return;
        }

        if (updatedSlotData.Count > 0)
        {
            slotControl.UpdateIcon(updatedSlotData.Item.Sprite);
            slotControl.UpdateCount(updatedSlotData.Count);
        }
        else
        {
            slotControl.ClearIcon();
        }
    }

    private void UpdateRemovedUI(SlotData removedSlotData)
    {
        Slot slotControl = FindSlotControlBySlotData(removedSlotData);

        if (slotControl == null) return;

        if (removedSlotData.Count <= 0)
        {
            slotControl.ClearIcon();
        }
        else
        {
            slotControl.UpdateCount(removedSlotData.Count);
        }
    }

    private void ClearSlotUI(SlotData clearedSlotData)
    {
        Slot slotControl = FindSlotControlBySlotData(clearedSlotData);

        if (slotControl == null) return;

        slotControl.ClearIcon();
    }

    public void RedrawAllSlots()
    {
        for (int i = 0; i < consumableInventory.Slots.Count; i++)
        {
            SlotData data = consumableInventory.Slots[i];
            Slot uiSlot = consumableSlots[i];

            if (data.IsEmpty)
            {
                uiSlot.ClearIcon();
            }
            else
            {
                uiSlot.UpdateIcon(data.Item.Sprite);
                uiSlot.UpdateCount(data.Count);
            }
            uiSlot.UpdateSlotBackground();
        }

        for (int i = 0; i < equipmentInventory.Slots.Count; i++)
        {
            SlotData data = equipmentInventory.Slots[i];
            Slot uiSlot = equipmentSlots[i];

            if (data.IsEmpty)
            {
                uiSlot.ClearIcon();
            }
            else
            {
                uiSlot.UpdateIcon(data.Item.Sprite);
                uiSlot.UpdateCount(data.Count);
            }
            uiSlot.UpdateSlotBackground();
        }
    }

    private Slot FindSlotControlBySlotData(SlotData data)
    {
        int index = consumableInventory.Slots.IndexOf(data);
        if (index >= 0 && index < consumableSlots.Count)
        {
            return consumableSlots[index];
        }

        index = equipmentInventory.Slots.IndexOf(data); 
        if (index >= 0 && index < equipmentSlots.Count)
        {
            return equipmentSlots[index];
        }

        return null;
    }

    private void OnInventoryReloaded()
    {
        Inventory equipmentInventory = ItemManager.Instance.equipmentInventory;

        if (equipmentInventory == null || equipmentInventory.Slots.Count != equipmentSlots.Count) return;

        inventoryHandler.ClearEquippedSlots();

        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            SlotData data = equipmentInventory.Slots[i]; 
            Slot uiSlot = equipmentSlots[i];          

            if (data.IsEquipped)
            {
                inventoryHandler.RestoreEquippedState(uiSlot); 
            }
        }
    }
}
