using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// ���� UI�� �����ϴ� Ŭ����
/// ������ ��� ǥ��, �� ����, ���� Ȯ�� �˾� �� ��ȣ�ۿ� ������ ���
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("Shop UI Root")]
    [SerializeField] private GameObject shopUI;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemPrefab;

    [Header("Item Detail UI")]
    [SerializeField] private UnityEngine.UI.Image itemImageL; 
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI inInventoryText;
    [SerializeField] private TextMeshProUGUI playerGold;

    [Header("Buy Interaction")]
    [SerializeField] private UnityEngine.UI.Button buyButton;
    [SerializeField] private UIPopup confirmPopup;
    [SerializeField] private UIPopup notEnoughGoldPopup;

    private Coroutine closePopupCoroutine;

    private void OnEnable()
    {
        GameEventsManager.Instance.ShopEvents.OnShopInitialize += InitializeShop;

        GameEventsManager.Instance.InputEvents.OnShopTogglePressed += ToggleUI;
        GameEventsManager.Instance.GoldEvents.OnGoldChange += PlayerGoldChange;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.ShopEvents.OnShopInitialize -= InitializeShop;

        GameEventsManager.Instance.InputEvents.OnShopTogglePressed -= ToggleUI;
        GameEventsManager.Instance.GoldEvents.OnGoldChange -= PlayerGoldChange;
    }

    public void InitializeShop(ItemSO[] items)
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        ItemSO firstItem = null;

        foreach (ItemSO item in items)
        {
            GameObject itemGO = Instantiate(itemPrefab, itemContainer);

            UnityEngine.UI.Image itemImageS = itemGO.transform.Find("ItemImageS").GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI itemName = itemGO.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();

            itemImageS.sprite = item.Sprite;
            itemName.text = item.ItemName;

            if (firstItem == null)
            {
                firstItem = item;
            }

            UnityEngine.UI.Button itemButton = itemGO.GetComponent<UnityEngine.UI.Button>();

            itemButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySFX("Click");
                ShowItemDetails(item);
            });
        }

        // ���� ���½� ù��° �������� �ڵ����� ����
        if (firstItem != null)
        {
            ShowItemDetails(firstItem);
        }
    }

    private void ShowItemDetails(ItemSO item)
    {
        itemImageL.sprite = item.Sprite;
        itemPrice.text = item.PurchasePrice.ToString();
        itemDescription.text = item.Description;
        inInventoryText.text = GameEventsManager.Instance.ItemEvents.GetItemNumberFromInventory(item).ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.gameObject.SetActive(true);
        buyButton.onClick.AddListener(() => TryPurchaseItem(item));
    }

    private void TryPurchaseItem(ItemSO item)
    {
        if (GoldManager.Instance.CurrentGold >= item.PurchasePrice)
        {
            UIManager.Instance.OpenPopup(confirmPopup);

            confirmPopup.YesButton.onClick.RemoveAllListeners();
            confirmPopup.NoButton.onClick.RemoveAllListeners();

            confirmPopup.YesButton.onClick.AddListener(() =>
            {
                PurchaseItem(item);
                confirmPopup.Close();
            });

            confirmPopup.NoButton.onClick.AddListener(() =>
            {
                confirmPopup.Close();
            });
        }
        else
        {
            UIManager.Instance.OpenPopupWithFade(notEnoughGoldPopup);
        }
    }

    private void PurchaseItem(ItemSO item)
    {
        GameEventsManager.Instance.GoldEvents.GoldGained(-item.PurchasePrice);
        GameEventsManager.Instance.ItemEvents.AddItem(item.ItemName);
        AudioManager.Instance.PlaySFX("Purchase");

        // �κ��丮�κ��� �ش� �������� ������ ã�ƿ� UI ǥ�ð� ����
        inInventoryText.text = GameEventsManager.Instance.ItemEvents.GetItemNumberFromInventory(item).ToString();
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

    public void ToggleUI()
    {
        bool isActive = !shopUI.activeSelf;
        shopUI.SetActive(isActive);

        GameEventsManager.Instance.InputEvents.NotifyUIClosed();
    }
}
