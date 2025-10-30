using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ����Ʈ ��ü ���¸� �����ϴ� Ŭ����
/// ����Ʈ�� ����, ����, �Ϸ�, ����/�ε带 ���
/// </summary>
public class QuestManager : MonoBehaviour, ISavable
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private bool loadQuestState = true; // ������: ���� �� ���� �ε� ����

    private Dictionary<string, Quest> questMap; // ����Ʈ ID -> ����Ʈ ��ü ����
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
    /// ������ �����ϴ� ����Ʈ�� CAN_START ���·� ��ȯ
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
    /// ����Ʈ ���¸� �����ϰ� �̺�Ʈ �˸�
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
    /// ����Ʈ �䱸 ����(����, ���� ����Ʈ ��)�� �����ϴ��� Ȯ��
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
    /// Resources ������ ��� QuestSO�� �о�, �� ���� ������ ��Ÿ�� Quest ��ü ���� ����
    /// </summary>
    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestSO[] allQuests = Resources.LoadAll<QuestSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.Id))
            {
                Debug.LogWarning("�ߺ��� ����ƮID �߰�: " + questInfo.Id);
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
            Debug.LogError("����Ʈ �ʿ� �ش��ϴ� ID�� �����ϴ�: " + id);
        }
        return quest;
    }

    /// <summary>
    /// SaveManager�� ���� ȣ��. ��� ����Ʈ�� ���� ���¸� data ��ü�� ����
    /// </summary>
    public void SaveData(GameSaveData saveData)
    {
        // data�� ����Ʈ ����Ʈ�� ���� �����.
        saveData.QuestSaveDataList.Clear();
        foreach (Quest quest in questMap.Values)
        {
            saveData.QuestSaveDataList.Add(quest.GetQuestSaveData());
        }
    }

    /// <summary>
    /// SaveManager�� ���� ȣ��. data ��ü�κ��� ��� ����Ʈ�� ���¸� ����
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
