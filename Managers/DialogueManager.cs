using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using static DialogueData;
using System.Linq;

/// <summary>
/// JSON 기반 대화 데이터를 관리하고 UI, 퀘스트, 상호작용과 연결하는 메인 Dialogue 매니저 클래스
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private Dictionary<string, DialogueData> dialogueDictionary; // JSON에서 로드한 대화 데이터를 저장할 딕셔너리

    private int currentLineIndex = 0;
    private int currentDialogueId = 1;
    private DialogueData currentDialogue;

    private string currentQuestId = null;
    private string currentNPCName;

    private bool isChoicesActive = false;
    private bool isTyping = false;
    private bool isDialogueComplete = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadDialogue();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.DialogueEvents.OnChoiceSelected += HandleChoice;
        GameEventsManager.Instance.DialogueEvents.OnTypingComplete += OnTypingComplete;
    }

    void OnDisable()
    {
        GameEventsManager.Instance.DialogueEvents.OnChoiceSelected -= HandleChoice;
        GameEventsManager.Instance.DialogueEvents.OnTypingComplete -= OnTypingComplete;
    }

    /// <summary>
    /// StreamingAssets 경로에서 dialogue.json을 로드하고, 선택지를 퀘스트 상태와 연결
    /// </summary>
    void LoadDialogue()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "dialogue.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            dialogueDictionary = JsonConvert.DeserializeObject<Dictionary<string, DialogueData>>(json);
        }
        else
        {
            Debug.LogError("파일을 찾지 못함: " + filePath);
        }
    }

    /// <summary>
    /// 대화를 특정 캐릭터와 라인 인덱스에서 출력
    /// 데이터 검증 후, 현재 상태를 업데이트하고 타이핑을 시작
    /// </summary>
    public void DisplayDialogue(string characterName, int dialogueId = 1, int lineIndex = 0)
    {
        currentNPCName = characterName;

        if (dialogueDictionary.TryGetValue(characterName, out currentDialogue))
        {
            if (currentDialogue.Dialogues.TryGetValue(dialogueId.ToString(), out DialogueData.Dialogue dialogue))
            {
                if (lineIndex >= 0 && lineIndex < dialogue.Lines.Length)
                {
                    currentLineIndex = lineIndex;
                    currentDialogueId = dialogueId;

                    string fullLine = dialogue.Lines[currentLineIndex].LineText;
                    GameEventsManager.Instance.DialogueEvents.StartDialogue(characterName, fullLine);

                    isTyping = true;
                    isDialogueComplete = false;
                    GameEventsManager.Instance.DialogueEvents.StartTyping(fullLine);

                    // 대화의 마지막 라인인 경우에만 선택지 처리
                    if (currentLineIndex == dialogue.Lines.Length - 1)
                    {
                        List<DialogueData.Choice> validChoices = GetValidChoices(dialogue);

                        if (validChoices.Count > 0)
                        {
                            isChoicesActive = true;
                            GameEventsManager.Instance.DialogueEvents.ShowChoices(validChoices.ToArray());
                        }
                        else
                        {
                            isChoicesActive = false;
                            GameEventsManager.Instance.DialogueEvents.HideChoices();
                        }
                    }
                }
                else
                {
                    Debug.LogError("유효하지 않은 라인 인덱스: " + lineIndex);
                }
            }
            else
            {
                Debug.LogError("해당 ID에 맞는 대화를 찾을 수 없음: " + dialogueId);
            }
        }
        else
        {
            Debug.LogError("해당 캐릭터 이름에 맞는 대화를 찾을 수 없음: " + characterName);
        }
    }

    /// <summary>
    /// 게임 상황을 고려하여 가장 적절한 기본 대화를 찾아 시작
    /// </summary>
    public void StartConversationWith(string npcId)
    {
        if (dialogueDictionary.TryGetValue(npcId, out var dialogueData))
        {
            // NPC의 조건 목록을 가져와 우선순위가 높은 순으로 정렬
            var conditions = dialogueData.DefaultDialogueConditions
                .OrderByDescending(c => c.Priority)
                .ToList();

            // 가장 적절한 대화 ID를 찾음
            int startingDialogueId = FindBestStartingDialogueId(conditions);

            // 찾은 ID로 대화를 시작
            DisplayDialogue(npcId, startingDialogueId);
        }
    }

    private int FindBestStartingDialogueId(List<DefaultDialogueCondition> conditions)
    {
        foreach (var condition in conditions)
        {
            // 조건 검사 로직
            bool conditionMet = true;

            // 완료된 퀘스트 조건이 있는가?
            if (!string.IsNullOrEmpty(condition.RequiredCompletedQuest))
            {
                Quest quest = QuestManager.Instance.GetQuestById(condition.RequiredCompletedQuest);
                if (quest == null || quest.QuestState != QuestState.FINISHED)
                {
                    conditionMet = false;
                }
            }

            // 특정 퀘스트 상태 조건이 있는가?
            if (conditionMet && !string.IsNullOrEmpty(condition.QuestId))
            {
                Quest quest = QuestManager.Instance.GetQuestById(condition.QuestId);
                if (quest == null || quest.QuestState.ToString() != condition.RequiredQuestState)
                {
                    conditionMet = false;
                }
            }

            if (conditionMet && !string.IsNullOrEmpty(condition.RequiredFlag))
            {
                if (!GameFlagManager.Instance.IsFlagSet(condition.RequiredFlag))
                {
                    conditionMet = false;
                }
            }

            // '반드시 설정되지 않았어야 하는' 플래그 조건이 있는가?
            if (conditionMet && !string.IsNullOrEmpty(condition.RequiredMissingFlag))
            {
                if (GameFlagManager.Instance.IsFlagSet(condition.RequiredMissingFlag))
                {
                    conditionMet = false;
                }
            }

            if (conditionMet)
            {
                return condition.DialogueId;
            }
        }

        // 어떤 특별한 조건도 만족하지 못했다면, 가장 낮은 우선순위의 기본 대화 ID를 반환
        return 1;
    }

    /// <summary>
    /// 다음 줄 대사로 넘어감. 대화가 끝나면 종료 처리
    /// </summary>
    public void NextDialogue()
    {
        if (isTyping)
        {
            return;
        } 
        else if (isDialogueComplete)
        {
            GameEventsManager.Instance.DialogueEvents.NextDialogue();

            if (currentDialogue != null && currentLineIndex + 1 < currentDialogue.Dialogues[currentDialogueId.ToString()].Lines.Length)
            {
                currentLineIndex++;
                DisplayDialogue(currentNPCName, currentDialogueId, currentLineIndex); // 현재 대화의 인덱스와 다음 라인 인덱스 사용
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void EndDialogue()
    {
        if (isChoicesActive) return;

        var dialogueInfo = currentDialogue.Dialogues[currentDialogueId.ToString()];
        if (!string.IsNullOrEmpty(dialogueInfo.SetFlagOnCompletion))
        {
            GameFlagManager.Instance.SetFlag(dialogueInfo.SetFlagOnCompletion);
        }

        GameEventsManager.Instance.DialogueEvents.EndDialogue();

        if (currentQuestId != null)
        {
            GameEventsManager.Instance.QuestEvents.FinishQuest(currentQuestId);
            currentQuestId = null;
        }
    }

    private void HandleChoice(DialogueData.Choice choice)
    {
        if (choice.NextDialogueId != 0)
        {
            DisplayDialogue(currentNPCName, choice.NextDialogueId);
        }

        switch (choice.Action)
        {
            case "OpenShop":
                GameEventsManager.Instance.DialogueEvents.EndDialogue();
                GameEventsManager.Instance.InputEvents.ShopTogglePressed();
                break;
            case "StartQuest":
                if (!string.IsNullOrEmpty(choice.QuestId))
                {
                    GameEventsManager.Instance.QuestEvents.StartQuest(choice.QuestId);
                }
                break;
            case "SellItem":
                GameEventsManager.Instance.DialogueEvents.EndDialogue();
                GameEventsManager.Instance.ShopEvents.EnableSellMode();
                GameEventsManager.Instance.InputEvents.InventoryTogglePressed();
                break;
            case "FinishQuest":
                if (!string.IsNullOrEmpty(choice.QuestId))
                {
                    GameEventsManager.Instance.QuestEvents.FinishQuest(choice.QuestId);
                }
                break;
        }

        GameEventsManager.Instance.DialogueEvents.HideChoices();
    }

    private List<DialogueData.Choice> GetValidChoices(DialogueData.Dialogue dialogue)
    {
        if (dialogue.Choices == null || dialogue.Choices.Length == 0)
        {
            return new List<DialogueData.Choice>();
        }

        var validChoices = new List<DialogueData.Choice>();

        foreach (var choice in dialogue.Choices)
        {
            bool shouldShow = true;

            if (!string.IsNullOrEmpty(choice.QuestId) && !string.IsNullOrEmpty(choice.RequiredQuestState))
            {
               Quest quest = QuestManager.Instance.GetQuestById(choice.QuestId);

                if (quest == null || quest.QuestState.ToString() != choice.RequiredQuestState)
                {
                    shouldShow = false;
                }
            }

            if (shouldShow)
            {
                validChoices.Add(choice);
            }
        }

        return validChoices;
    }

    private void OnTypingComplete()
    {
        isTyping = false;
        isDialogueComplete = true;
    }
}