using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 퀘스트 전체 상태를 관리하는 클래스
/// 퀘스트의 시작, 진행, 완료, 저장/로드를 담당
/// </summary>
public class QuestManager : MonoBehaviour, ISavable
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private bool loadQuestState = true; // 디버깅용: 시작 시 강제 로드 여부

    private Dictionary<string, Quest> questMap; // 퀘스트 ID -> 퀘스트 객체 매핑
    private int currentPlayerLevel;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        questMap = CreateQuestMap();
    }

    void OnEnable()
    {
        SaveManager.Instance.RegisterSavable(this);

        GameEventsManager.Instance.QuestEvents.OnStartQuest += StartQuest;
        GameEventsManager.Instance.QuestEvents.OnAdvanceQuest += AdvanceQuest;
        GameEventsManager.Instance.QuestEvents.OnFinishQuest += FinishQuest;

        GameEventsManager.Instance.QuestEvents.OnQuestStepStateChange += QuestStepStateChange;

        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelUp += SetPlayerLevel;
    }

    private void OnDisable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UnregisterSavable(this);
        }

        GameEventsManager.Instance.QuestEvents.OnStartQuest -= StartQuest;
        GameEventsManager.Instance.QuestEvents.OnAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance.QuestEvents.OnFinishQuest -= FinishQuest;

        GameEventsManager.Instance.QuestEvents.OnQuestStepStateChange -= QuestStepStateChange;

        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelUp -= SetPlayerLevel;
    }

    private void Start()
    {
        BroadcastAllQuestStates();
    }

    /// <summary>
    /// 조건을 만족하는 퀘스트는 CAN_START 상태로 전환
    /// </summary>
    private void Update()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.QuestState == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.QuestInfo.Id, QuestState.CAN_START);
            }
        }
    }

    /// <summary>
    /// 퀘스트 상태를 변경하고 이벤트 알림
    /// </summary>
    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.QuestState = state;
        GameEventsManager.Instance.QuestEvents.QuestStateChange(quest);
    }

    private void SetPlayerLevel(int level)
    {
        currentPlayerLevel = level;
    }

    /// <summary>
    /// 퀘스트 요구 조건(레벨, 선행 퀘스트 등)을 만족하는지 확인
    /// </summary>
    private bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;

        if (currentPlayerLevel < quest.QuestInfo.LevelRequirement)
        {
            meetsRequirements = false;
        }

        foreach (QuestSO prerequisiteQuestInfo in quest.QuestInfo.QuestPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.Id).QuestState != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.QuestInfo.Id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(transform);
        }
        else
        {
            ChangeQuestState(quest.QuestInfo.Id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.QuestInfo.Id, QuestState.FINISHED);
    }

    private void ClaimRewards(Quest quest)
    {
        GameEventsManager.Instance.GoldEvents.GoldGained(quest.QuestInfo.GoldReward);
        GameEventsManager.Instance.PlayerEvents.ExperienceGained(quest.QuestInfo.ExperienceReward);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.QuestState);
    }

    /// <summary>
    /// Resources 폴더의 모든 QuestSO를 읽어, 새 게임 상태의 런타임 Quest 객체 맵을 생성
    /// </summary>
    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestSO[] allQuests = Resources.LoadAll<QuestSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.Id))
            {
                Debug.LogWarning("중복된 퀘스트ID 발견: " + questInfo.Id);
            }
            idToQuestMap.Add(questInfo.Id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    public Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("퀘스트 맵에 해당하는 ID가 없습니다: " + id);
        }
        return quest;
    }

    /// <summary>
    /// SaveManager에 의해 호출. 모든 퀘스트의 현재 상태를 data 객체에 저장
    /// </summary>
    public void SaveData(GameSaveData saveData)
    {
        // data의 퀘스트 리스트를 먼저 비워줌.
        saveData.QuestSaveDataList.Clear();
        foreach (Quest quest in questMap.Values)
        {
            saveData.QuestSaveDataList.Add(quest.GetQuestSaveData());
        }
    }

    /// <summary>
    /// SaveManager에 의해 호출. data 객체로부터 모든 퀘스트의 상태를 복원
    /// </summary>
    public void LoadData(GameSaveData data)
    {
        CleanupActiveQuestSteps();

        Dictionary<string, QuestSaveData> questDataMap = new();
        foreach (QuestSaveData saveDataEntry in data.QuestSaveDataList)
        {
            questDataMap[saveDataEntry.Id] = saveDataEntry;
        }

        foreach (Quest quest in questMap.Values)
        {
            string questId = quest.QuestInfo.Id;
            if (questDataMap.TryGetValue(questId, out QuestSaveData loadedSaveData))
            {
                quest.LoadState(
                    loadedSaveData.Data.State,
                    loadedSaveData.Data.QuestStepIndex,
                    loadedSaveData.Data.QuestStepStates
                );
            }
        }

        InstantiateInProgressQuestSteps();
        BroadcastAllQuestStates();
    }

    private void CleanupActiveQuestSteps()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.CurrentQuestStepObject != null)
            {
                Destroy(quest.CurrentQuestStepObject.gameObject);
                quest.CurrentQuestStepObject = null;
            }
        }
    }

    private void InstantiateInProgressQuestSteps()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.QuestState == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
        }
    }

    private void BroadcastAllQuestStates()
    {
        foreach (Quest quest in questMap.Values)
        {
            GameEventsManager.Instance.QuestEvents.QuestStateChange(quest);
        }
    }

    public IEnumerable<Quest> GetAllQuests()
    {
        return questMap.Values;
    }
}
