using UnityEngine;

/// <summary>
/// 아이템 오브젝트에 부착되어 상호작용을 돕는 클래스
/// </summary>
public class ItemObject : MonoBehaviour, IInteractable
{
    public InteractionType InteractionType => InteractionType.Item;
    public ItemSO DropedItem;

    public void Interact()
    {
        string itemName = DropedItem.ItemName;

        GameEventsManager.Instance.ItemEvents.AddItem(itemName);
        GameEventsManager.Instance.UIEvents.DiscoverItem(null, false);
        GameEventsManager.Instance.ItemEvents.GetItemDisplay(DropedItem);
        AudioManager.Instance.PlaySFX("GetItem");
        Destroy(gameObject);
    }

    public void SetDroppedItem(ItemSO droppedItem)
    {
        DropedItem = droppedItem;
    }
}
