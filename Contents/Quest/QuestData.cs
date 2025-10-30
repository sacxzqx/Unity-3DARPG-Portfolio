/// <summary>
/// 특정 퀘스트의 현재 상태 정보를 저장하는 클래스
/// 퀘스트의 진행도, 단계 인덱스, 각 스텝의 상태 배열을 포함함
/// </summary>
[System.Serializable]
public class QuestData
{
    public QuestState State;
    public int QuestStepIndex;
    public QuestStepState[] QuestStepStates;

    public QuestData(QuestState state, int questStepIndex, QuestStepState[] questStepStates)
    {
        State = state;
        QuestStepIndex = questStepIndex;
        QuestStepStates = questStepStates;
    }
}