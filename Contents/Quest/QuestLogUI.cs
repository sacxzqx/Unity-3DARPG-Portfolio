using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// ����Ʈ �α� ȭ���� �ֻ��� UI �г��� �����ϴ� Ŭ����
/// Quest ���� ���� �̺�Ʈ�� �޾� ���� ��ũ�� ����Ʈ�� �ݿ��ϰ�,
/// ���õ� ����Ʈ�� �� ����(����, �䱸����)�� �ؽ�Ʈ �ʵ忡 ���ε�
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
    /// ����Ʈ ���� ���� �� ��ư�� �����ϰų� �����ϰ�, UI�� �ݿ�
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
    /// ������ ����Ʈ�� �� ������ ǥ��
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