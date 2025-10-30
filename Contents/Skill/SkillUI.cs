using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

/// <summary>
/// 스킬 트리 UI를 관리하는 클래스
/// 스킬 버튼 초기화, 스킬 포인트 표시, 팝업 처리, UI 갱신 등을 담당
/// </summary>
public class SkillUI : MonoBehaviour
{
    [SerializeField] private GameObject contentParent;
    [SerializeField] private Button[] skillButtons;
    [SerializeField] private UIPopup confirmPopup;
    [SerializeField] private UIPopup warningPopup;
    [SerializeField] private TextMeshProUGUI skillpointsText;

    [SerializeField] private Sprite notLearnedFrame;
    [SerializeField] private Sprite learnedFrame;

    [SerializeField] private List<SkillQuickSlot> skillQuickSlots;

    private SkillManager skillManager;

    private void Start()
    {
        skillManager = GetComponent<SkillManager>();
        InitializeSkillButtons();
        UpdateAllSkillsUI();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.InputEvents.OnSkillTreeTogglePressed += SkillTreeTogglePressed;
        GameEventsManager.Instance.UIEvents.OnSkillDataReloaded += UpdateAllSkillsUI;
        GameEventsManager.Instance.UIEvents.OnSkillDataReloaded += RedrawQuickSlots;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.InputEvents.OnSkillTreeTogglePressed -= SkillTreeTogglePressed;
        GameEventsManager.Instance.UIEvents.OnSkillDataReloaded -= UpdateAllSkillsUI;
        GameEventsManager.Instance.UIEvents.OnSkillDataReloaded -= RedrawQuickSlots;
    }

    private void RedrawQuickSlots()
    {
        foreach (var skill in skillQuickSlots)
        {
            SkillSO skillForKey = skillManager.GetSkillForKey(skill.key);

            skill.AssignSkill(skillForKey);
        }
    }

    private void InitializeSkillButtons()
    {
        foreach (var button in skillButtons)
        {
            SkillButton skillButton = button.GetComponent<SkillButton>();
            button.onClick.AddListener(() => OnSkillButtonClicked(skillButton.Skill));
        }
    }

    private void OnSkillButtonClicked(SkillSO skill)
    {
        if (!HasEnoughSkillPoints(skill))
        {
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("MyTable", "SkillPoint_Lack").Completed += handle =>
            {
                warningPopup.PopupText.text = handle.Result;
                UIManager.Instance.OpenPopupWithFade(warningPopup);
            };
            return;
        }

        ShowConfirmationPopup(skill);
    }

    private void ShowConfirmationPopup(SkillSO skill)
    {
        confirmPopup.Open();

        confirmPopup.YesButton.onClick.RemoveAllListeners();
        confirmPopup.NoButton.onClick.RemoveAllListeners();

        confirmPopup.YesButton.onClick.AddListener(() =>
        {
            OnSkillConfirmed(skill);
            confirmPopup.Close();
        });

        confirmPopup.NoButton.onClick.AddListener(() =>
        {
            confirmPopup.Close();
        });
    }

    private void OnSkillConfirmed(SkillSO skill)
    {
        skillManager.LearnSkill(skill);

        UpdateAllSkillsUI();
    }

    public void UpdateSingleSkillUI(Button button)
    {
        SkillButton skillButton = button.GetComponent<SkillButton>();

        if (skillManager.HasLearnedSkill(skillButton.Skill))
        {
            button.interactable = false;
            button.image.color = Color.white;
            skillButton.FrameImage.sprite = learnedFrame;
            skillButton.ActivateLines();
        }
        else if (skillManager.CanLearnSkill(skillButton.Skill))
        {
            button.interactable = true;
            button.image.color = Color.white;
            skillButton.FrameImage.sprite = notLearnedFrame;
        }
        else
        {
            button.interactable = false;
            button.image.color = Color.gray;
            skillButton.FrameImage.sprite = notLearnedFrame;
        }
    }

    public void UpdateAllSkillsUI()
    {
        int skillPoints = skillManager.GetSkillPoint();
        LocalizationSettings.StringDatabase.GetLocalizedStringAsync("MyTable", "SkillPoints_Text").Completed += handle =>
        {
            skillpointsText.text = string.Format(handle.Result, skillPoints);
        };

        foreach (var button in skillButtons)
        {
            UpdateSingleSkillUI(button);
        }
    }

    private void SkillTreeTogglePressed()
    {
        if (contentParent.activeInHierarchy)
        {
            HideUI();
        }
        else
        {
            ShowUI();
        }
    }

    private void ShowUI()
    {
        contentParent.SetActive(true);
    }

    private void HideUI()
    {
        contentParent.SetActive(false);
    }

    private bool HasEnoughSkillPoints(SkillSO skill)
    {
        return skillManager.GetAvailableSkillPoints();
    }
}
