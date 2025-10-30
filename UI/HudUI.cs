using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 플레이어 HUD UI를 관리하는 클래스
/// 체력/마나 바, 스킬 슬롯, 아이템 획득 알림, 퀘스트 팝업 등을 제어
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
    /// 상호작용한 아이템 정보를 HUD에 표시하거나 숨김
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
    /// 아이템 획득 시 HUD에 획득 애니메이션 출력
    /// </summary>
    private void ShowAcquiredItem(ItemSO item)
    {
        acquiredItemDisplay.SetActive(true);
        acquiredItemName.text = item.ItemName;
        acquiredItemSprite.sprite = item.Sprite;

        currentDisplaySequence?.Kill();
        currentDisplaySequence = DOTween.Sequence();

        // 1. 초기 상태 설정 (0 스케일)
        acquiredItemBackground.gameObject.SetActive(true);
        acquiredItemBackground.localScale = new Vector3(0f, 0f, 1f);

        // 2. DOTween 시퀀스 정의: 등장 -> 대기 -> 퇴장
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
    /// 스킬 키에 해당하는 HUD 슬롯에 스킬 아이콘 및 데이터 연결
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
    /// 스킬 쿨타임 시작 시 HUD 아이콘의 fillAmount로 표시
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

        Debug.LogWarning($"해당 스킬에 매칭되는 HUD 슬롯 없음: {skill.name}");
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
    /// 스킬 사용 실패 시 메시지 팝업 표시
    /// </summary>
    private void HandleSkillFailure(SkillUseResult result)
    {
        switch (result)
        {
            case SkillUseResult.NotAssigned:
                ShowMessage("스킬이 할당되어 있지 않습니다.");
                break;

            case SkillUseResult.OnCooldown:
                ShowMessage("스킬이 아직 쿨타임 중입니다.");
                break;

            case SkillUseResult.NotEnoughMana:
                // BlinkManaBar(); // 마나바 깜빡이기 효과
                ShowMessage("마나가 부족합니다.");
                break;
        }
    }

    private void ShowMessage(string content)
    {
        messagePopupText.text = content;
        messagePopup.FadePopupInAndOut();
    }

    /// <summary>
    /// 퀘스트 시작 시 퀘스트 팝업을 HUD에 출력
    /// </summary>
    private void ShowQuestBox(Quest quest)
    {
        // 게임 로드시에 해당 함수가 트리거되는 경우에는 즉시 종료
        if (SaveManager.Instance.IsLoading) return;

        switch (quest.QuestState)
        {
            // 퀘스트가 진행 중 상태로 처음 변경되었을 때
            case QuestState.IN_PROGRESS:
                // 이 퀘스트의 시작 팝업을 아직 본 적이 없다면
                if (!shownQuestPopups.Contains(quest.QuestInfo.Id))
                {
                    questExpReward.text = "";
                    questGoldReward.text = "";
                    shownQuestPopups.Add(quest.QuestInfo.Id);
                    questName.text = "새로운 퀘스트: " + quest.QuestInfo.DisplayName;
                    questPopup.FadePopupInAndOut(1.5f, 3f);
                    AudioManager.Instance.PlaySFX("StartQuest");
                }
                break;

            // 퀘스트가 완료됨 상태로 변경되었을 때
            case QuestState.FINISHED:
                questExpReward.text = "경험치: " + quest.QuestInfo.ExperienceReward;
                questGoldReward.text = "골드: " + quest.QuestInfo.GoldReward;
                questName.text = "퀘스트 완료: " + quest.QuestInfo.DisplayName;
                questPopup.FadePopupInAndOut(1.5f, 3f);
                AudioManager.Instance.PlaySFX("CompleteQuest");
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 다른 UI 열림 여부에 따라 HUD의 표시 여부 조정
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
        // 저장된 List<string>을 HashSet으로 복원
        if (data.QuestPopupHistory != null)
        {
            // 새 HashSet을 만들어 저장된 이력으로 초기화
            shownQuestPopups = new HashSet<string>(data.QuestPopupHistory);
        }
        else
        {
            shownQuestPopups = new HashSet<string>();
        }
    }
}
