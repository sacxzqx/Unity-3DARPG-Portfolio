using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// �÷��̾� HUD UI�� �����ϴ� Ŭ����
/// ü��/���� ��, ��ų ����, ������ ȹ�� �˸�, ����Ʈ �˾� ���� ����
/// </summary>
public class HudUI : MonoBehaviour, ISavable
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private GameObject sliders;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private RectTransform healthBarRightCap;
    [SerializeField] private RectTransform manaBarRightCap;

    private const float StatToPixelScale = 5f;
    private const float BarBorderOffset = 20f;

    [SerializeField] private List<HudSkillSlot> skillSlots = new List<HudSkillSlot>();

    [SerializeField] private GameObject itemDisplay;
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemName;

    [SerializeField] private GameObject acquiredItemDisplay;
    [SerializeField] private RectTransform acquiredItemBackground;
    [SerializeField] private Image acquiredItemSprite;
    [SerializeField] private TextMeshProUGUI acquiredItemName;

    [SerializeField] private UIPopup questPopup;
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questGoldReward;
    [SerializeField] private TextMeshProUGUI questExpReward;

    [SerializeField] private UIPopup messagePopup;
    [SerializeField] private TextMeshProUGUI messagePopupText;

    private HashSet<string> shownQuestPopups = new HashSet<string>();

    private Sequence currentDisplaySequence;

    private void OnEnable()
    {
        SaveManager.Instance.RegisterSavable(this);

        GameEventsManager.Instance.PlayerEvents.OnHealthChanged += UpdateHealthSlider;
        GameEventsManager.Instance.PlayerEvents.OnManaChanged += UpdateManaSlider;
        GameEventsManager.Instance.UIEvents.OnItemDiscoverd += UpdateItemDisplay;
        GameEventsManager.Instance.ItemEvents.OnItemGetDisplay += ShowAcquiredItem;
        GameEventsManager.Instance.PlayerEvents.OnMaxHealthChanged += UpdateMaxHealthSlider;
        GameEventsManager.Instance.PlayerEvents.OnMaxManaChanged += UpdateMaxManaSlider;
        GameEventsManager.Instance.QuestEvents.OnQuestStateChange += ShowQuestBox;
        GameEventsManager.Instance.InputEvents.OnUIStateChange += HandleUIStateChange;
        GameEventsManager.Instance.UIEvents.OnSkillAssignedToKey += SetSkillToHudSlot;
        GameEventsManager.Instance.UIEvents.OnSkillCooldownStarted += HandleSkillCooldown;
        GameEventsManager.Instance.UIEvents.OnSkillFailureDisplay += HandleSkillFailure;
    }

    private void OnDisable()
    {
        SaveManager.Instance.UnregisterSavable(this);

        GameEventsManager.Instance.PlayerEvents.OnHealthChanged -= UpdateHealthSlider;
        GameEventsManager.Instance.PlayerEvents.OnManaChanged -= UpdateManaSlider;
        GameEventsManager.Instance.UIEvents.OnItemDiscoverd -= UpdateItemDisplay;
        GameEventsManager.Instance.ItemEvents.OnItemGetDisplay -= ShowAcquiredItem;
        GameEventsManager.Instance.PlayerEvents.OnMaxHealthChanged -= UpdateMaxHealthSlider;
        GameEventsManager.Instance.PlayerEvents.OnMaxManaChanged -= UpdateMaxManaSlider;
        GameEventsManager.Instance.QuestEvents.OnQuestStateChange -= ShowQuestBox;
        GameEventsManager.Instance.InputEvents.OnUIStateChange -= HandleUIStateChange;
        GameEventsManager.Instance.UIEvents.OnSkillAssignedToKey -= SetSkillToHudSlot;
        GameEventsManager.Instance.UIEvents.OnSkillCooldownStarted -= HandleSkillCooldown;
        GameEventsManager.Instance.UIEvents.OnSkillFailureDisplay -= HandleSkillFailure;
    }

    private void UpdateHealthSlider(float currentHealth)
    {
        healthSlider.DOValue(currentHealth, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    private void UpdateManaSlider(float currentMana)
    {
        manaSlider.DOValue(currentMana, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    private void UpdateMaxHealthSlider(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;

        float actualBarWidth = maxHealth * StatToPixelScale;

        float containerWidth = actualBarWidth + BarBorderOffset;

        RectTransform sliderRect = healthSlider.GetComponent<RectTransform>();
        if (sliderRect != null)
        {
            Vector2 sliderSize = sliderRect.sizeDelta;

            sliderSize.x = actualBarWidth;
            sliderRect.sizeDelta = sliderSize;
        }

        if (healthBarRightCap != null)
        {
            Vector2 capSize = healthBarRightCap.sizeDelta;

            capSize.x = containerWidth;
            healthBarRightCap.sizeDelta = capSize;
        }
    }

    private void UpdateMaxManaSlider(float maxMana)
    {
        manaSlider.maxValue = maxMana;

        float actualBarWidth = maxMana * StatToPixelScale;

        float containerWidth = actualBarWidth + BarBorderOffset;

        RectTransform sliderRect = manaSlider.GetComponent<RectTransform>();
        if (sliderRect != null)
        {
            Vector2 sliderSize = sliderRect.sizeDelta;

            sliderSize.x = actualBarWidth;
            sliderRect.sizeDelta = sliderSize;
        }

        if (manaBarRightCap != null)
        {
            Vector2 capSize = manaBarRightCap.sizeDelta;

            capSize.x = containerWidth;
            manaBarRightCap.sizeDelta = capSize;
        }
    }

    /// <summary>
    /// ��ȣ�ۿ��� ������ ������ HUD�� ǥ���ϰų� ����
    /// </summary>
    private void UpdateItemDisplay(ItemSO item, bool isActive)
    {
        itemDisplay.SetActive(isActive);

        if (isActive)
        {
            itemSprite.sprite = item.Sprite;
            itemName.text = item.ItemName;
        }
    }

    /// <summary>
    /// ������ ȹ�� �� HUD�� ȹ�� �ִϸ��̼� ���
    /// </summary>
    private void ShowAcquiredItem(ItemSO item)
    {
        acquiredItemDisplay.SetActive(true);
        acquiredItemName.text = item.ItemName;
        acquiredItemSprite.sprite = item.Sprite;

        currentDisplaySequence?.Kill();
        currentDisplaySequence = DOTween.Sequence();

        // 1. �ʱ� ���� ���� (0 ������)
        acquiredItemBackground.gameObject.SetActive(true);
        acquiredItemBackground.localScale = new Vector3(0f, 0f, 1f);

        // 2. DOTween ������ ����: ���� -> ��� -> ����
        currentDisplaySequence.Append(acquiredItemBackground.DOScaleY(1f, 0.15f).SetEase(Ease.OutQuad))
                  .Append(acquiredItemBackground.DOScaleX(1f, 0.15f).SetEase(Ease.OutQuad))
                  .AppendInterval(1.5f)
                  .Append(acquiredItemBackground.DOScaleY(0f, 0.15f).SetEase(Ease.InQuad))
                  .Append(acquiredItemBackground.DOScaleX(0f, 0.15f).SetEase(Ease.InQuad))
                  .SetUpdate(true)
                  .OnComplete(() => {
                      acquiredItemDisplay.SetActive(false);
                  });
    }

    /// <summary>
    /// ��ų Ű�� �ش��ϴ� HUD ���Կ� ��ų ������ �� ������ ����
    /// </summary>
    private void SetSkillToHudSlot(string key, SkillSO skill)
    {
        foreach (var slot in skillSlots)
        {
            if (slot.Key == key)
            {
                if (skill == null)
                {
                    slot.CurrentSkill = null;
                    slot.IconImage.sprite = null;
                    slot.SlotRoot.SetActive(false);
                    return;
                }

                slot.CurrentSkill = skill;
                slot.IconImage.sprite = skill.Icon;
                slot.SlotRoot.SetActive(true);
                return;
            }
        }
    }

    /// <summary>
    /// ��ų ��Ÿ�� ���� �� HUD �������� fillAmount�� ǥ��
    /// </summary>
    private void HandleSkillCooldown(SkillSO skill, float duration)
    {
        foreach (var slot in skillSlots)
        {
            if (slot.CurrentSkill == skill)
            {
                StartCoroutine(CooldownFillRoutine(slot.IconImage, duration));
            }
        }

        Debug.LogWarning($"�ش� ��ų�� ��Ī�Ǵ� HUD ���� ����: {skill.name}");
    }

    private IEnumerator CooldownFillRoutine(Image skillIcon, float duration)
    {
        skillIcon.fillAmount = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            skillIcon.fillAmount = 0f + (elapsed / duration);
            yield return null;
        }

        skillIcon.fillAmount = 1f;
    }

    /// <summary>
    /// ��ų ��� ���� �� �޽��� �˾� ǥ��
    /// </summary>
    private void HandleSkillFailure(SkillUseResult result)
    {
        switch (result)
        {
            case SkillUseResult.NotAssigned:
                ShowMessage("��ų�� �Ҵ�Ǿ� ���� �ʽ��ϴ�.");
                break;

            case SkillUseResult.OnCooldown:
                ShowMessage("��ų�� ���� ��Ÿ�� ���Դϴ�.");
                break;

            case SkillUseResult.NotEnoughMana:
                // BlinkManaBar(); // ������ �����̱� ȿ��
                ShowMessage("������ �����մϴ�.");
                break;
        }
    }

    private void ShowMessage(string content)
    {
        messagePopupText.text = content;
        messagePopup.FadePopupInAndOut();
    }

    /// <summary>
    /// ����Ʈ ���� �� ����Ʈ �˾��� HUD�� ���
    /// </summary>
    private void ShowQuestBox(Quest quest)
    {
        // ���� �ε�ÿ� �ش� �Լ��� Ʈ���ŵǴ� ��쿡�� ��� ����
        if (SaveManager.Instance.IsLoading) return;

        switch (quest.QuestState)
        {
            // ����Ʈ�� ���� �� ���·� ó�� ����Ǿ��� ��
            case QuestState.IN_PROGRESS:
                // �� ����Ʈ�� ���� �˾��� ���� �� ���� ���ٸ�
                if (!shownQuestPopups.Contains(quest.QuestInfo.Id))
                {
                    questExpReward.text = "";
                    questGoldReward.text = "";
                    shownQuestPopups.Add(quest.QuestInfo.Id);
                    questName.text = "���ο� ����Ʈ: " + quest.QuestInfo.DisplayName;
                    questPopup.FadePopupInAndOut(1.5f, 3f);
                    AudioManager.Instance.PlaySFX("StartQuest");
                }
                break;

            // ����Ʈ�� �Ϸ�� ���·� ����Ǿ��� ��
            case QuestState.FINISHED:
                questExpReward.text = "����ġ: " + quest.QuestInfo.ExperienceReward;
                questGoldReward.text = "���: " + quest.QuestInfo.GoldReward;
                questName.text = "����Ʈ �Ϸ�: " + quest.QuestInfo.DisplayName;
                questPopup.FadePopupInAndOut(1.5f, 3f);
                AudioManager.Instance.PlaySFX("CompleteQuest");
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// �ٸ� UI ���� ���ο� ���� HUD�� ǥ�� ���� ����
    /// </summary>
    private void HandleUIStateChange(UIType type, bool isOpen)
    {
        if (type == UIType.Inventory)
        {
            contentParent.SetActive(!isOpen);
            sliders.SetActive(true);
        }
        else
        {
            contentParent.SetActive(!isOpen);
            sliders.SetActive(!isOpen);
        }
    }

    public void SaveData(GameSaveData saveData)
    {
        saveData.QuestPopupHistory = new List<string>(shownQuestPopups);
    }

    public void LoadData(GameSaveData data)
    {
        // ����� List<string>�� HashSet���� ����
        if (data.QuestPopupHistory != null)
        {
            // �� HashSet�� ����� ����� �̷����� �ʱ�ȭ
            shownQuestPopups = new HashSet<string>(data.QuestPopupHistory);
        }
        else
        {
            shownQuestPopups = new HashSet<string>();
        }
    }
}
