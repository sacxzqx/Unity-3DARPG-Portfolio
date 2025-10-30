/// <summary>
/// 퀘스트의 전반적인 진행 상태를 나타내는 열거형
/// </summary>
public enum QuestState
{
    REQUIREMENTS_NOT_MET,
    CAN_START,
    IN_PROGRESS,
    CAN_FINISH,
    FINISHED
}