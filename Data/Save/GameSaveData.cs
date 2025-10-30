using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public List<QuestSaveData> QuestSaveDataList = new();

    public PlayerData PlayerData = new PlayerData();

    public SaveMetaData MetaData = new();

    public List<string> GameFlags = new List<string>();

    public List<ItemSaveData> InventoryData = new List<ItemSaveData>();

    public List<string> QuestPopupHistory;
}
