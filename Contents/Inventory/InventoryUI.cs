using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// �κ��丮 UI�� ����ϴ� Ŭ����
/// ��ư�� ���� ���/�Һ�â ��ȯ, ���� �ʱ�ȭ �� UI ������Ʈ ó��
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
            Debug.LogError("ItemManager �ν��Ͻ��� ã�� �� �����ϴ�.");
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
    /// �κ��丮 �����͸� ������� UI ���� �������� �����ϰ� �����͸� ����
    /// </summary>
    /// <param name="inventory">���� �����͸� ��� �ִ� Inventory �ν��Ͻ�</param>
    /// <param name="targetList">������ ���� UI�� ������ ����Ʈ</param>
    /// <param name="parent">������ ��ġ�� Transform �θ�</param>
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
        // ���� �ݾ� �ؽ�Ʈ�� ���Ͽ� DOTween �ִϸ��̼� ����
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
    /// ���/�Һ� �κ��丮 �� ��ȯ ó��
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
            Debug.LogError("��ȯ�� ������ UI ��Ʈ���� ã�� �� ���� ȭ�� ������ �ߴ��մϴ�.");
            return;
        }

        slotControlA.UpdateIcon(dataA.Item?.Sprite);
        slotControlA.UpdateCount(dataA.Count);
        slotControlA.UpdateSlotBackground();

        slotControlB.UpdateIcon(dataB.Item?.Sprite);
        slotControlB.UpdateCount(dataB.Count);
        slotControlB.UpdateSlotBackground();

        // A ������ �������� ����̸� ���� ���� ���
        if (dataA.IsEquipped && dataA.Item is EquipmentSO eqA)
        {
            // InventoryHandler���� A ������ ���� ���¸� ������ ������ ��û
            inventoryHandler.SwapEquippedSlot(slotControlA);
        }

        // B ������ �������� ����̸� ���� ���� ���
        if (dataB.IsEquipped && dataB.Item is EquipmentSO eqB)
        {
            // InventoryHandler���� B ������ ���� ���¸� ������ ������ ��û
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
            Debug.LogError("������Ʈ�� ���� UI ��Ʈ���� ã�� ����.");
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
