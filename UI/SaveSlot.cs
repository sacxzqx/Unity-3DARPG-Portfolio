using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI slotText;
    [SerializeField] private Button slotButton;
    [SerializeField] private GameObject highlightFrame;

    public int slotIndex;

    private void OnDisable()
    {
        highlightFrame.SetActive(false);
    }

    public void Initialize(int index, SaveLoadUI saveLoadUI)
    {
        slotIndex = index;
        slotButton.onClick.AddListener(() => saveLoadUI.OnSlotClicked(this));
    }

    public void SetSlotData(string scene, string time)
    {
        if (string.IsNullOrEmpty(scene))
        {
            slotText.text = "Empty";
        }
        else
        {
            slotText.text = $"{scene}\n{time}";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightFrame.transform.SetParent(transform, false);
        highlightFrame.transform.SetAsLastSibling();
        highlightFrame.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightFrame.SetActive(false);
    }
}
