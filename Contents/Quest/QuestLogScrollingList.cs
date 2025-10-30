using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ����Ʈ �α� UI���� ��ũ�� ������ ����Ʈ ��ư ����Ʈ�� �����ϴ� ������Ʈ
/// ����Ʈ ��ư ����, ĳ��, ���� �� ��ũ�� ��ġ �ڵ� ���� ���� ����
/// </summary>
public class QuestLogScrollingList : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;

    [Header("Rect Transforms")]
    [SerializeField] private RectTransform scrollRectTransform;
    [SerializeField] private RectTransform contentRectTransform;

    [Header("Quest Log Button")]
    [SerializeField] private GameObject questLogButtonPrefab;

    private Dictionary<string, QuestLogButton> idToButtonMap = new Dictionary<string, QuestLogButton>();

    /// <summary>
    /// ����Ʈ�� �����ϴ� ��ư�� ������ �����ϰ� ��ȯ
    /// �̹� ������ ĳ�õ� ��ư�� ��ȯ
    /// </summary>
    public QuestLogButton CreateButtonIfNotExists(Quest quest, UnityAction selectAction)
    {
        QuestLogButton questLogButton = null;

        if (!idToButtonMap.ContainsKey(quest.QuestInfo.Id))
        {
            questLogButton = InstantiateQuestLogButton(quest, selectAction);
        }
        else
        {
            questLogButton = idToButtonMap[quest.QuestInfo.Id];
        }

        return questLogButton;
    }

    private QuestLogButton InstantiateQuestLogButton(Quest quest, UnityAction selectAction)
    {
        QuestLogButton questLogButton = Instantiate(
            questLogButtonPrefab,
            contentParent.transform).GetComponent<QuestLogButton>();

        questLogButton.gameObject.name = quest.QuestInfo.Id + "_button";

        RectTransform buttonRectTransform = questLogButton.GetComponent<RectTransform>();

        questLogButton.Initialize(quest.QuestInfo.DisplayName, () => {
            selectAction();
            UpdateScrolling(buttonRectTransform);
        });

        idToButtonMap[quest.QuestInfo.Id] = questLogButton;
        return questLogButton;
    }

    /// <summary>
    /// ������ ��ư�� ��ũ�� �� ���� �ȿ� �������� �ڵ� ����
    /// </summary>
    private void UpdateScrolling(RectTransform buttonRectTransform)
    {
        float buttonYMin = Mathf.Abs(buttonRectTransform.anchoredPosition.y);
        float buttonYMax = buttonYMin + buttonRectTransform.rect.height;

        float contentYMin = contentRectTransform.anchoredPosition.y;
        float contentYMax = contentYMin + scrollRectTransform.rect.height;

        // �Ʒ��� ��ũ��
        if (buttonYMax > contentYMax)
        {
            contentRectTransform.anchoredPosition = new Vector2(
                contentRectTransform.anchoredPosition.x,
                buttonYMax - scrollRectTransform.rect.height
            );
        }
        // ���� ��ũ��
        else if (buttonYMin < contentYMin)
        {
            contentRectTransform.anchoredPosition = new Vector2(
                contentRectTransform.anchoredPosition.x,
                buttonYMin
            );
        }
    }
}