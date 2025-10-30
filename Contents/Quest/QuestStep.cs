using UnityEngine;

/// <summary>
/// ��� ����Ʈ �ܰ�(QuestStep)�� �⺻ ������ �����ϴ� �߻� Ŭ����
/// �ʱ�ȭ, �Ϸ� ó��, ���� ���� �� ���� ����� �����ϸ� ���� �ܰ�� �̸� ��ӹ޾� ������
/// </summary>
public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    private string questId;
    private int stepIndex;
    protected QuestStepState stepState;

    public void InitializeQuestStep(string questId, int stepIndex, QuestStepState stepState)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        this.stepState = stepState;

        if (stepState != null)
        {
            SetQuestStepState(stepState);
        }
    }

    /// <summary>
    /// ���� ����Ʈ �ܰ踦 �Ϸ� ó���ϰ� �̺�Ʈ�� ���� ���� �ܰ�� ����
    /// </summary>
    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            GameEventsManager.Instance.QuestEvents.AdvanceQuest(questId);
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// ����Ʈ �ܰ��� ���� ���¸� �����ϰ� �ý��ۿ� �˸�
    /// </summary>
    protected void ChangeState(string newState, string newStatus, string newProgressData)
    {
        QuestStepState updatedState = new QuestStepState(newState, newStatus);
        updatedState.ProgressData = newProgressData;

        GameEventsManager.Instance.QuestEvents.QuestStepStateChange(
            questId,
            stepIndex,
            updatedState
        );
    }

    /// <summary>
    /// ����� ����Ʈ �ܰ� ���¸� �ε��Ͽ� ���������� �ݿ��ϴ� �޼��� (��� Ŭ�������� ����)
    /// </summary>
    protected abstract void SetQuestStepState(QuestStepState state);
}
