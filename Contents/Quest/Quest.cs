using UnityEngine;

/// <summary>
/// ����Ʈ ���� �� ���¿� ������ �����ϴ� ��Ÿ�� Ŭ����
/// ScriptableObject ��� ������(QuestSO)�� �����Ͽ� ����Ʈ�� �����Ŵ
/// </summary>
public class Quest
{
    public QuestSO QuestInfo;

    public QuestState QuestState;

    private int currentQuestStepIndex;
    private QuestStepState[] questStepStates;

    public QuestStep CurrentQuestStepObject = null;

    /// <summary>
    /// �ű� ����Ʈ ���� �� ���Ǵ� ������
    /// </summary>
    public Quest(QuestSO questInfo)
    {
        QuestInfo = questInfo;
        QuestState = QuestState.REQUIREMENTS_NOT_MET;
        currentQuestStepIndex = 0;

        questStepStates = new QuestStepState[questInfo.QuestStepPrefabs.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    /// <summary>
    /// ����� �����ͷ� ����Ʈ�� ������ �� ����ϴ� ������
    /// </summary>
    public Quest(QuestSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
    {
        QuestInfo = questInfo;
        QuestState = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;

        // ������ ����ġ ���
        if (questStepStates.Length != questInfo.QuestStepPrefabs.Length)
        {
            Debug.LogWarning($"[����Ʈ �ε� ���] ���� ���� ���� ������ ���� ��ġ���� �ʽ��ϴ�. " +
                             $"����� �����Ϳ� ����Ʈ ������ ����Ǿ��� �� �ֽ��ϴ�. QuestId: {questInfo.Id}");
        }
    }

    /// <summary>
    /// ����� �����ͷκ��� ����Ʈ�� ���¸� ����
    /// �̹� ������ Quest ��ü�� ���¸� ����� �� ���
    /// </summary>
    public void LoadState(QuestState state, int stepIndex, QuestStepState[] stepStates)
    {
        this.QuestState = state;
        this.currentQuestStepIndex = stepIndex;
        this.questStepStates = stepStates;
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < QuestInfo.QuestStepPrefabs.Length);
    }

    /// <summary>
    /// ���� ���� �������� �ν��Ͻ�ȭ�ϰ� �ʱ�ȭ
    /// </summary>
    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();

        if (questStepPrefab != null)
        {
            QuestStep questStep = Object.Instantiate<GameObject>(questStepPrefab, parentTransform)
                .GetComponent<QuestStep>();
            questStep.InitializeQuestStep(QuestInfo.Id, currentQuestStepIndex, questStepStates[currentQuestStepIndex]);

            CurrentQuestStepObject = questStep;
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;

        if (CurrentStepExists())
        {
            questStepPrefab = QuestInfo.QuestStepPrefabs[currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning($"���� ����Ʈ �ܰ� �ε����� ������ ������ϴ�. " +
                             $"�ش� ����Ʈ�� ��ȿ�� ������ �������� �ʽ��ϴ�. " +
                             $"QuestId={QuestInfo.Id}, stepIndex={currentQuestStepIndex}");
        }
        return questStepPrefab;
    }

    /// <summary>
    /// Ư�� ������ ���� ������ ����
    /// </summary>
    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
    {
        if (stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].State = questStepState.State;
            questStepStates[stepIndex].Status = questStepState.Status;
            questStepStates[stepIndex].ProgressData = questStepState.ProgressData;
        }
        else
        {
            Debug.LogWarning($"����Ʈ ���� �����͸� �����Ϸ� ������, �ε����� ������ ������ϴ�. " +
                             $"Quest Id = {QuestInfo.Id}, Step Index = {stepIndex}");
        }
    }

    /// <summary>
    /// ������ ����, ���� ����Ʈ�� ID�� ���� ��Ȳ �����͸� �����ϴ� QuestSaveData ��ü�� �����Ͽ� ��ȯ
    /// </summary>
    public QuestSaveData GetQuestSaveData()
    {
        QuestData currentData = new QuestData(QuestState, currentQuestStepIndex, questStepStates);

        return new QuestSaveData { Id = QuestInfo.Id, Data = currentData };
    }

    /// <summary>
    /// ����Ʈ�� ��ü ���� ���¸� ����� �ؽ�Ʈ ��ȯ
    /// </summary>
    public string GetFullStatusText()
    {
        string fullStatus = "";

        if (QuestState == QuestState.REQUIREMENTS_NOT_MET)
        {
            fullStatus = "����Ʈ �䱸 ������ �������� ����.";
        }
        else if (QuestState == QuestState.CAN_START)
        {
            fullStatus = "�ش� ����Ʈ�� ������ �� ����.";
        }
        else
        {
            // �Ϸ�� ������ ��Ҽ� ó��
            for (int i = 0; i < currentQuestStepIndex; i++)
            {
                fullStatus += "<s>" + questStepStates[i].Status + "</s>\n";
            }

            // ���� ����
            if (CurrentStepExists())
            {
                fullStatus += questStepStates[currentQuestStepIndex].Status;
            }

            // �Ϸ� �Ǵ� �Ϸ� ����
            if (QuestState == QuestState.CAN_FINISH)
            {
                fullStatus += "����Ʈ�� �����Ϸ� ����.";
            }
            else if (QuestState == QuestState.FINISHED)
            {
                fullStatus += "�ش� ����Ʈ�� �Ϸ�Ǿ����ϴ�.";
            }
        }

        return fullStatus;
    }
}
