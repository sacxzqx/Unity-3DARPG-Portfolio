using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 퀘스트 로그 UI에서 스크롤 가능한 퀘스트 버튼 리스트를 관리하는 컴포넌트
/// 퀘스트 버튼 생성, 캐싱, 선택 시 스크롤 위치 자동 조정 등을 수행
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
    /// 퀘스트에 대응하는 버튼이 없으면 생성하고 반환
    /// 이미 있으면 캐시된 버튼을 반환
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
    /// 선택한 버튼이 스크롤 뷰 영역 안에 들어오도록 자동 조정
    /// </summary>
    private void UpdateScrolling(RectTransform buttonRectTransform)
    {
        float buttonYMin = Mathf.Abs(buttonRectTransform.anchoredPosition.y);
        float buttonYMax = buttonYMin + buttonRectTransform.rect.height;

        float contentYMin = contentRectTransform.anchoredPosition.y;
        float contentYMax = contentYMin + scrollRectTransform.rect.height;

        // 아래로 스크롤
        if (buttonYMax > contentYMax)
        {
            contentRectTransform.anchoredPosition = new Vector2(
                contentRectTransform.anchoredPosition.x,
                buttonYMax - scrollRectTransform.rect.height
            );
        }
        // 위로 스크롤
        else if (buttonYMin < contentYMin)
        {
            contentRectTransform.anchoredPosition = new Vector2(
                contentRectTransform.anchoredPosition.x,
                buttonYMin
            );
        }
    }
}