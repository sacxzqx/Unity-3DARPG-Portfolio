using UnityEngine;

/// <summary>
/// ������ ������Ʈ�� �����Ǿ� ��ȣ�ۿ��� ���� Ŭ����
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
