using UnityEngine;

/// <summary>
/// 퀘스트 실행 중 상태와 동작을 제어하는 런타임 클래스
/// ScriptableObject 기반 데이터(QuestSO)를 참조하여 퀘스트를 진행시킴
/// </summary>
public class Quest
{
    public QuestSO QuestInfo;

    public QuestState QuestState;

    private int currentQuestStepIndex;
    private QuestStepState[] questStepStates;

    public QuestStep CurrentQuestStepObject = null;

    /// <summary>
    /// 신규 퀘스트 생성 시 사용되는 생성자
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
    /// 저장된 데이터로 퀘스트를 복원할 때 사용하는 생성자
    /// </summary>
    public Quest(QuestSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
    {
        QuestInfo = questInfo;
        QuestState = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;

        // 데이터 불일치 경고
        if (questStepStates.Length != questInfo.QuestStepPrefabs.Length)
        {
            Debug.LogWarning($"[퀘스트 로드 경고] 스텝 상태 수와 프리팹 수가 일치하지 않습니다. " +
                             $"저장된 데이터와 퀘스트 에셋이 변경되었을 수 있습니다. QuestId: {questInfo.Id}");
        }
    }

    /// <summary>
    /// 저장된 데이터로부터 퀘스트의 상태를 복원
    /// 이미 생성된 Quest 객체의 상태를 덮어쓰는 데 사용
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
    /// 현재 스텝 프리팹을 인스턴스화하고 초기화
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
            Debug.LogWarning($"현재 퀘스트 단계 인덱스가 범위를 벗어났습니다. " +
                             $"해당 퀘스트에 유효한 스텝이 존재하지 않습니다. " +
                             $"QuestId={QuestInfo.Id}, stepIndex={currentQuestStepIndex}");
        }
        return questStepPrefab;
    }

    /// <summary>
    /// 특정 스텝의 상태 정보를 저장
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
            Debug.LogWarning($"퀘스트 스텝 데이터를 접근하려 했지만, 인덱스가 범위를 벗어났습니다. " +
                             $"Quest Id = {QuestInfo.Id}, Step Index = {stepIndex}");
        }
    }

    /// <summary>
    /// 저장을 위해, 현재 퀘스트의 ID와 진행 상황 데이터를 포함하는 QuestSaveData 객체를 생성하여 반환
    /// </summary>
    public QuestSaveData GetQuestSaveData()
    {
        QuestData currentData = new QuestData(QuestState, currentQuestStepIndex, questStepStates);

        return new QuestSaveData { Id = QuestInfo.Id, Data = currentData };
    }

    /// <summary>
    /// 퀘스트의 전체 진행 상태를 요약한 텍스트 반환
    /// </summary>
    public string GetFullStatusText()
    {
        string fullStatus = "";

        if (QuestState == QuestState.REQUIREMENTS_NOT_MET)
        {
            fullStatus = "퀘스트 요구 사항을 충족하지 못함.";
        }
        else if (QuestState == QuestState.CAN_START)
        {
            fullStatus = "해당 퀘스트는 시작할 수 없음.";
        }
        else
        {
            // 완료된 스텝은 취소선 처리
            for (int i = 0; i < currentQuestStepIndex; i++)
            {
                fullStatus += "<s>" + questStepStates[i].Status + "</s>\n";
            }

            // 현재 스텝
            if (CurrentStepExists())
            {
                fullStatus += questStepStates[currentQuestStepIndex].Status;
            }

            // 완료 또는 완료 가능
            if (QuestState == QuestState.CAN_FINISH)
            {
                fullStatus += "퀘스트를 보고하러 가자.";
            }
            else if (QuestState == QuestState.FINISHED)
            {
                fullStatus += "해당 퀘스트는 완료되었습니다.";
            }
        }

        return fullStatus;
    }
}
