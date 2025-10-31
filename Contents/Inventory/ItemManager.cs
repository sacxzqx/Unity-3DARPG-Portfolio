using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 데이터와 인벤토리 연동을 관리하는 클래스
/// 아이템 추가, 제거 등 처리
/// </summary>
public class ItemManager : MonoBehaviour, ISavable
{
    public static ItemManager Instance { get; private set; }

    [SerializeField] private List<ItemSO> itemEntries = new List<ItemSO>();

    private Dictionary<string, ItemSO> itemList = new Dictionary<string, ItemSO>();

    [Header("Inventory Config")]
    [SerializeField] private int consumableSlotCount = 15;
    [SerializeField] private int equipmentSlotCount = 9;

    public Inventory consumableInventory { get; private set; }
    public Inventory equipmentInventory { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        consumableInventory = new Inventory(consumableSlotCount);
        equipmentInventory = new Inventory(equipmentSlotCount);

        foreach (ItemSO entry in itemEntries)
        {
            if (!itemList.ContainsKey(entry.ItemName))
            {
                itemList.Add(entry.ItemName, entry);
            }
        }
    }

    private void OnEnable()
    {
        SaveManager.Instance.RegisterSavable(this);

        GameEventsManager.Instance.ItemEvents.OnItemAdded += AddItem;
        GameEventsManager.Instance.ItemEvents.OnItemRemove += RemoveItem;
        GameEventsManager.Instance.ItemEvents.OnItemSell += SellItem;
        GameEventsManager.Instance.ItemEvents.OnItemNumberGetFromInventory += GetItemCountProxy;
    }

    private void OnDisable()
    {
        SaveManager.Instance.UnregisterSavable(this);

        GameEventsManager.Instance.ItemEvents.OnItemAdded -= AddItem;
        GameEventsManager.Instance.ItemEvents.OnItemRemove -= RemoveItem;
        GameEventsManager.Instance.ItemEvents.OnItemSell -= SellItem;
        GameEventsManager.Instance.ItemEvents.OnItemNumberGetFromInventory -= GetItemCountProxy;
    }

    private ItemSO GetItem(string itemName)
    {
        if (itemList.TryGetValue(itemName, out ItemSO item))
        {
            return item;
        }

        return null;
    }

    private void AddItem(string itemName)
    {
        ItemSO item = GetItem(itemName);

        if (item != null)
        {
            switch (item.ItemType)
            {
                case ItemType.Equipment:
                    equipmentInventory.AddItem(item, 1);
                    break;
                case ItemType.Consumable:
                    consumableInventory.AddItem(item, 1);
                    break;
                default:
                    Debug.LogWarning($"[ItemManager] 알 수 없는 아이템 타입: {item.ItemType}");
                    break;
            }
        }
    }


    private void RemoveItem(ItemSO item, Inventory inventory)
    {
        if (inventory != null)
        {
            inventory.RemoveItem(item);
        }
        else
        {
            Debug.LogWarning("[ItemManager] 인벤토리를 찾을 수 없음");
        }
    }

    private void SellItem(ItemSO item)
    {
        GameEventsManager.Instance.GoldEvents.GoldGained(item.SellPrice);
    }

    private int GetItemCountProxy(ItemSO item)
    {
        int totalCount = 0;

        if (equipmentInventory != null)
        {
            totalCount += equipmentInventory.GetNumberOfItem(item);
        }

        if (consumableInventory != null)
        {
            totalCount += consumableInventory.GetNumberOfItem(item);
        }

        return totalCount;
    }

    public void SaveData(GameSaveData data)
    {
        data.PlayerData.ConsumableInventoryData = InventoryToSaveData(consumableInventory);
        data.PlayerData.EquipmentInventoryData = InventoryToSaveData(equipmentInventory);
    }

    public void LoadData(GameSaveData data)
    {
        // 로드시에 현재 장비중인 장비들을 해제
        foreach (var slot in equipmentInventory.Slots)
        {
            if (slot.IsEquipped && slot.Item is EquipmentSO equipment)
            {
                GameEventsManager.Instance.ItemEvents.UnequipItem(equipment);
            }
        }

        SaveDataToInventory(consumableInventory, data.PlayerData.ConsumableInventoryData);
        SaveDataToInventory(equipmentInventory, data.PlayerData.EquipmentInventoryData);

        // 로드된 인벤토리에 따라 장비 재장착
        GameEventsManager.Instance.ItemEvents.InventoryReloaded();
    }

    private List<ItemSaveData> InventoryToSaveData(Inventory inventory)
    {
        List<ItemSaveData> saveData = new List<ItemSaveData>();
        for (int i = 0; i < inventory.Slots.Count; i++)
        {
            SlotData slot = inventory.Slots[i];
            if (slot.IsEmpty) continue;

            saveData.Add(new ItemSaveData
            {
                SlotIndex = i,
                ItemId = slot.Item.ItemName,
                Count = slot.Count,
                IsEquipped = slot.IsEquipped,
            });
        }
        return saveData;
    }

    private void SaveDataToInventory(Inventory inventory, List<ItemSaveData> saveData)
    {
        inventory.ClearAllSlots();
        foreach (var itemData in saveData)
        {
            ItemSO item = GetItem(itemData.ItemId);
            if (item != null && itemData.SlotIndex < inventory.Slots.Count)
            {
                inventory.Slots[itemData.SlotIndex].Item = item;
                inventory.Slots[itemData.SlotIndex].Count = itemData.Count;
                inventory.Slots[itemData.SlotIndex].IsEquipped = itemData.IsEquipped;
            }
        }
    }
}