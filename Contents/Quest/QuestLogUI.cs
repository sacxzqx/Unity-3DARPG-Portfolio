using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 퀘스트 로그 화면의 최상위 UI 패널을 제어하는 클래스
/// Quest 상태 변경 이벤트를 받아 하위 스크롤 리스트에 반영하고,
/// 선택된 퀘스트의 상세 정보(보상, 요구사항)를 텍스트 필드에 바인딩
/// </summary>
public class QuestLogUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private QuestLogScrollingList scrollingList;

    [SerializeField] private TextMeshProUGUI questDisplayNameText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI goldRewardsText;
    [SerializeField] private TextMeshProUGUI experienceRewardsText;
    [SerializeField] private TextMeshProUGUI levelRequirementsText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;

    private Button firstSelectedButton;

    private void OnEnable()
    {
        GameEventsManager.Instance.InputEvents.OnQuestLogTogglePressed += QuestLogTogglePressed;
        GameEventsManager.Instance.QuestEvents.OnQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.InputEvents.OnQuestLogTogglePressed -= QuestLogTogglePressed;
        GameEventsManager.Instance.QuestEvents.OnQuestStateChange -= QuestStateChange;
    }

    private void QuestLogTogglePressed()
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

        if (firstSelectedButton != null)
        {
            firstSelectedButton.Select();
        }
    }

    private void HideUI()
    {
        contentParent.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// 퀘스트 상태 변경 시 버튼을 생성하거나 갱신하고, UI에 반영
    /// </summary>
    private void QuestStateChange(Quest quest)
    {
        if (quest.QuestState == QuestState.REQUIREMENTS_NOT_MET || quest.QuestState == QuestState.CAN_START) return;

        QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () => {
            SetQuestLogInfo(quest);
        });

        if (firstSelectedButton == null)
        {
            firstSelectedButton = questLogButton.Button;
        }

        questLogButton.SetState(quest.QuestState);
    }

    /// <summary>
    /// 선택한 퀘스트의 상세 정보를 표시
    /// </summary>
    private void SetQuestLogInfo(Quest quest)
    {
        questDisplayNameText.text = quest.QuestInfo.DisplayName;
        questDescriptionText.text = quest.QuestInfo.QuestDescription;
        questStatusText.text = quest.GetFullStatusText();
        goldRewardsText.text = quest.QuestInfo.GoldReward + " Gold";
        experienceRewardsText.text = quest.QuestInfo.ExperienceReward + " XP";
    }
}