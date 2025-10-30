using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// ����Ʈ �α� UI���� �ϳ��� ����Ʈ �׸� ��ư�� ��Ÿ���� Ŭ����
/// ����Ʈ �̸� ����, ���� �̺�Ʈ ó��, ���¿� ���� ���� ������ ������
/// </summary>
public class QuestLogButton : MonoBehaviour, ISelectHandler
{
    public Button Button { get; private set; }
    private TextMeshProUGUI buttonText;
    private UnityAction onSelectAction;

    /// <summary>
    /// ��ư�� �ʱ�ȭ�ϰ� �ؽ�Ʈ �� ���� �� ������ ����
    /// </summary>
    /// <param name="displayName">����Ʈ �̸�</param>
    /// <param name="selectAction">���� �� ������ �ݹ�</param>
    public void Initialize(string displayName, UnityAction selectAction)
    {
        Button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        buttonText.text = displayName;
        onSelectAction = selectAction;
    }

    /// <summary>
    /// ����Ʈ ���¿� ���� ��ư �ؽ�Ʈ ���� ����
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