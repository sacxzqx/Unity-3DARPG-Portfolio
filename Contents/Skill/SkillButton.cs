using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SkillSO Skill;
    public Image FrameImage;

    [SerializeField] private Image[] connectedLines;

    [SerializeField] private Transform originalParent;

    [SerializeField] private GameObject skillPopup;
    [SerializeField] private Image popupImage;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI requiredLevelText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;

    private Image skillImage;
    private SkillManager skillManager;

    private void Awake()
    {
        skillImage = GetComponent<Image>();
        FrameImage = transform.parent.GetComponent<Image>();
        skillManager = GetComponentInParent<SkillManager>();
    }

    void Start()
    {
        skillImage.sprite = Skill.Icon;
    }

    public void ActivateLines()
    {
        foreach (var line in connectedLines)
        {
            line.gameObject.SetActive(true); 
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        skillImage.raycastTarget = false; // 드래그 중 다른 UI와 충돌 방지
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillManager.HasLearnedSkill(Skill))
        {
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.position = originalParent.position;
        skillImage.raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillPopup != null)
        {
            skillPopup.SetActive(true);
            skillPopup.transform.position = eventData.position + new Vector2(350f, -150f);

            UpdatePopupInfo();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillPopup != null)
        {
            skillPopup.SetActive(false);
        }
    }

    private void UpdatePopupInfo()
    {
        if (popupImage != null)
            popupImage.sprite = Skill.Icon;

        Skill.SkillName.GetLocalizedStringAsync().Completed += handle =>
        {
            if (skillNameText != null)
                skillNameText.text = handle.Result;
        };

        Skill.Description.GetLocalizedStringAsync().Completed += handle =>
        {
            if (skillDescriptionText != null)
                skillDescriptionText.text = handle.Result;
        };

        if (requiredLevelText != null)
            requiredLevelText.text = $"필요 레벨: {Skill.RequiredLevel}";
    }
}
