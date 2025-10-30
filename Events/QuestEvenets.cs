using System;

/// <summary>
/// 퀘스트 진행과 관련된 이벤트를 관리하는 클래스.
/// 퀘스트의 시작, 진행, 완료, 상태 변경, 단계 상태 변경 등을 처리함.
/// </summary>
public class QuestEvents 
{
    public event Action<string> OnStartQuest;
    public void StartQuest(string id)
    {
        if (OnStartQuest != null)
        {
            OnStartQuest(id);
        }
    }

    /// <summary>
    /// 퀘스트의 다음 단계로 진행될 때 호출되는 이벤트 
    /// ex) CAN_START -> IN_PROGRESS
    /// </summary>
    public event Action<string> OnAdvanceQuest;
    public void AdvanceQuest(string id)
    {
        if (OnAdvanceQuest != null)
        {
            OnAdvanceQuest(id);
        }
    }

    public event Action<string> OnFinishQuest;
    public void FinishQuest(string id)
    {
        if (OnFinishQuest != null)
        {
            OnFinishQuest(id);
        }
    }

    /// <summary>
    /// 퀘스트의 전체 상태(진행 상태 등)가 변경될 때 호출되는 이벤트
    /// </summary>
    public event Action<Quest> OnQuestStateChange;
    public void QuestStateChange(Quest quest)
    {
        if (OnQuestStateChange != null)
        {
            OnQuestStateChange(quest);
        }
    }

    /// <summary>
    /// 특정 퀘스트 단계의 상태가 변경될 때마다 호출되는 이벤트
    /// </summary>
    public event Action<string, int, QuestStepState> OnQuestStepStateChange;
    public void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        if (OnQuestStepStateChange != null)
        {
            OnQuestStepStateChange(id, stepIndex, questStepState);
        }
    }
}
