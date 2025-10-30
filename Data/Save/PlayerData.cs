using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int Level;
    public int Experience;
    public int StatPoint;
    public int Gold;
    public List<StatSaveData> Stats = new();

    public Vector3 Position;
    public string SceneName;

    public List<ItemSaveData> ConsumableInventoryData = new List<ItemSaveData>();
    public List<ItemSaveData> EquipmentInventoryData = new List<ItemSaveData>();

    public SkillSaveData SkillData;
}