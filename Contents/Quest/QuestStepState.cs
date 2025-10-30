[System.Serializable]
public class QuestStepState
{
    public string State;
    public string Status;

    public string ProgressData;

    public QuestStepState(string state, string status)
    {
        State = state;
        Status = status;
    }

    public QuestStepState()
    {
        State = "";
        Status = "";
    }
}
