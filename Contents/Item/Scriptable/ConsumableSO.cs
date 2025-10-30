using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Item/Consumable")]
public abstract class ConsumableSO : ItemSO
{
    public abstract void Use(PlayerStat playerStat);
}