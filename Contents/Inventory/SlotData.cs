using UnityEngine;

[SerializeField]
public class SlotData
{
    public ItemSO Item;
    public int Count;
    public bool IsEquipped;
    public bool IsSellable = true;

    public bool IsEmpty => Item == null || Count <= 0;
    public void Clear() { Item = null; Count = 0; IsEquipped = false; }
}
