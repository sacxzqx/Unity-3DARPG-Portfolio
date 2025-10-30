using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// �κ��丮 UI���� �ϳ��� ������ ����ϴ� Ŭ����
/// ������ ǥ��, �巡�� �� ���, Ŭ�� ���ͷ��� ���� ó����
/// </summary>
public class Slot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public SlotData slotData;

    [SerializeField] private Image slotImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] public Image draggingIcon;
    [SerializeField] private TextMeshProUGUI itemCountText;

    [SerializeField] private UIPopup selectPanel;
    [SerializeField] private UIPopup sellPopup;

    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;

    public static Slot DraggedSlot;
    private bool isDragging = false;

    public Inventory ParentInventory { get; private set; }

    private InventoryHandler inventoryHandler;

    void Awake()
    {
        inventoryHandler = GetComponentInParent<InventoryHandler>();
    }

    public void Initialize(UIPopup selectPanel, UIPopup sellPopup, TextMeshProUGUI itemName, TextMeshProUGUI itemDescription, Inventory parentInventory)
    {
        if (slotData == null)
        {
            slotData = new SlotData();
        }
        this.selectPanel = selectPanel;
        this.sellPopup = sellPopup;
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.ParentInventory = parentInventory;
    }

    public void LinkData(SlotData dataToLink)
    {
        this.slotData = dataToLink;

        ClearIcon();
        UpdateCount(dataToLink.Count);
        UpdateSlotBackground();
    }

    public void UpdateIcon(Sprite icon)
    {
        if (icon != null)
        {
            slotImage.sprite = icon; 
            slotImage.color = new Color(1, 1, 1, 1); 
        }
        else
        {
            slotImage.sprite = null; 
            slotImage.color = new Color(1, 1, 1, 0); 
        }
    }

    public void ClearIcon()
    {
        slotImage.color = new Color(1, 1, 1, 0);
        slotImage.sprite = null;
    }

    public void UpdateCount(int count)
    {
        if (slotData.Item != null && !slotData.Item.Stackable)
        {
            itemCountText.text = "";
            return;
        }

        itemCountText.text = count > 1 ? count.ToString() : "";
    }

    public void UpdateSlotBackground()
    {
        if (slotData.IsEquipped)
        {
            SetBackgroundImage("Equipped_Image");
        }
        else
        {
            SetBackgroundImage("Empty_Image");
        }
    }

    public void SetBackgroundImage(string imageName)
    {
        Sprite newBackground = Resources.Load<Sprite>("Images/" + imageName);

        if (newBackground != null)
        {
            backgroundImage.sprite = newBackground;
        }
        else
        {
            Debug.LogError("�ش� �̹��� ������ ã�� �� �����ϴ�.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(slotData.Item != null && !inventoryHandler.IsSellMode)
        {
            if (selectPanel.gameObject.activeSelf)
            {
                UIManager.Instance.ClosePopup(selectPanel);
            }

            DraggedSlot = this;
            draggingIcon.sprite = slotImage.sprite;
            draggingIcon.gameObject.SetActive(true);
            slotImage.color = new Color(1, 1, 1, 0.5f);

            isDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotData.Item != null && !inventoryHandler.IsSellMode)
        {
            draggingIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(slotData.Item != null && !inventoryHandler.IsSellMode)
        {
            draggingIcon.gameObject.SetActive(false);

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = eventData.position
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            Slot dropSlot = null;

            foreach (RaycastResult result in results)
            {
                dropSlot = result.gameObject.GetComponent<Slot>();
                if (dropSlot != null) break; // ���� ����� ������ ã���� �ݺ� ����
            }

            // ��� ��ġ�� ��ȿ�� �����̰� �ٸ� �����̶�� ������ �� ��� ��ȯ
            if (dropSlot != null && dropSlot != DraggedSlot)
            {
                DraggedSlot.ParentInventory.SwapSlotData(DraggedSlot.slotData, dropSlot.slotData);
            }
            else
            {
                // ����� ��ȿ���� �ʴٸ� ���� ���·� ����
                slotImage.color = new Color(1, 1, 1, 1);
            }

            DraggedSlot = null;
            isDragging = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotData.Item != null && !isDragging)
        {
            RectTransform useButtonRectTransform = selectPanel.gameObject.GetComponent<RectTransform>();
            RectTransform sellButtonRectTransform = sellPopup.gameObject.GetComponent<RectTransform>();

            if (useButtonRectTransform != null)
            {
                // Ŭ���� ��ġ�� ȭ�� ��ǥ���� �ش� ĵ���� ��ǥ��� ��ȯ
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    useButtonRectTransform.parent as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out localPoint);

                useButtonRectTransform.localPosition = new Vector2(localPoint.x, localPoint.y - 30);
                sellButtonRectTransform.localPosition = new Vector2(localPoint.x, localPoint.y - 30);

                if (!inventoryHandler.IsSellMode)
                {
                    if (selectPanel.gameObject.activeSelf)
                    {
                        UIManager.Instance.ClosePopup(selectPanel);
                    }
                    else
                    {
                        UIManager.Instance.OpenPopup(selectPanel);
                    }
                }
                else
                {
                    if (sellPopup.gameObject.activeSelf)
                    {
                        UIManager.Instance.ClosePopup(sellPopup);
                    }
                    else
                    {
                        UIManager.Instance.OpenPopup(sellPopup);
                    }
                }

                itemName.text = slotData.Item.ItemName;
                itemDescription.text = slotData.Item.Description;

                inventoryHandler.SetCurrentSlot(this);
            }
        }
    }
}