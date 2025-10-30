using UnityEngine;

/// <summary>
/// 모든 퀘스트 단계(QuestStep)의 기본 동작을 정의하는 추상 클래스
/// 초기화, 완료 처리, 상태 갱신 등 공통 기능을 제공하며 개별 단계는 이를 상속받아 구현됨
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
    /// 현재 퀘스트 단계를 완료 처리하고 이벤트를 통해 다음 단계로 진행
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
    /// 퀘스트 단계의 현재 상태를 변경하고 시스템에 알림
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
    /// 저장된 퀘스트 단계 상태를 로드하여 내부적으로 반영하는 메서드 (상속 클래스에서 구현)
    /// </summary>
    protected abstract void SetQuestStepState(QuestStepState state);
}
