using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 퀘스트 로그 UI에서 하나의 퀘스트 항목 버튼을 나타내는 클래스
/// 퀘스트 이름 설정, 선택 이벤트 처리, 상태에 따른 색상 변경을 지원함
/// </summary>
public class QuestLogButton : MonoBehaviour, ISelectHandler
{
    public Button Button { get; private set; }
    private TextMeshProUGUI buttonText;
    private UnityAction onSelectAction;

    /// <summary>
    /// 버튼을 초기화하고 텍스트 및 선택 시 동작을 설정
    /// </summary>
    /// <param name="displayName">퀘스트 이름</param>
    /// <param name="selectAction">선택 시 실행할 콜백</param>
    public void Initialize(string displayName, UnityAction selectAction)
    {
        Button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        buttonText.text = displayName;
        onSelectAction = selectAction;
    }

    /// <summary>
    /// 퀘스트 상태에 따라 버튼 텍스트 색상 변경
    /// </summary>
    public void SetState(QuestState state)
    {
        switch (state)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
            case QuestState.CAN_START:
                buttonText.color = Color.red;
                break;
            case QuestState.IN_PROGRESS:
            case QuestState.CAN_FINISH:
                buttonText.color = Color.yellow;
                break;
            case QuestState.FINISHED:
                buttonText.color = Color.green;
                break;
            default:
                Debug.LogWarning("Quest State not recognized by switch statement: " + state);
                break;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelectAction();
    }
}