using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using static DialogueData;
using System.Linq;

/// <summary>
/// JSON ��� ��ȭ �����͸� �����ϰ� UI, ����Ʈ, ��ȣ�ۿ�� �����ϴ� ���� Dialogue �Ŵ��� Ŭ����
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private Dictionary<string, DialogueData> dialogueDictionary; // JSON���� �ε��� ��ȭ �����͸� ������ ��ųʸ�

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
    /// StreamingAssets ��ο��� dialogue.json�� �ε��ϰ�, �������� ����Ʈ ���¿� ����
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
            Debug.LogError("������ ã�� ����: " + filePath);
        }
    }

    /// <summary>
    /// ��ȭ�� Ư�� ĳ���Ϳ� ���� �ε������� ���
    /// ������ ���� ��, ���� ���¸� ������Ʈ�ϰ� Ÿ������ ����
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

                    // ��ȭ�� ������ ������ ��쿡�� ������ ó��
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
                    Debug.LogError("��ȿ���� ���� ���� �ε���: " + lineIndex);
                }
            }
            else
            {
                Debug.LogError("�ش� ID�� �´� ��ȭ�� ã�� �� ����: " + dialogueId);
            }
        }
        else
        {
            Debug.LogError("�ش� ĳ���� �̸��� �´� ��ȭ�� ã�� �� ����: " + characterName);
        }
    }

    /// <summary>
    /// ���� ��Ȳ�� ����Ͽ� ���� ������ �⺻ ��ȭ�� ã�� ����
    /// </summary>
    public void StartConversationWith(string npcId)
    {
        if (dialogueDictionary.TryGetValue(npcId, out var dialogueData))
        {
            // NPC�� ���� ����� ������ �켱������ ���� ������ ����
            var conditions = dialogueData.DefaultDialogueConditions
                .OrderByDescending(c => c.Priority)
                .ToList();

            // ���� ������ ��ȭ ID�� ã��
            int startingDialogueId = FindBestStartingDialogueId(conditions);

            // ã�� ID�� ��ȭ�� ����
            DisplayDialogue(npcId, startingDialogueId);
        }
    }

    private int FindBestStartingDialogueId(List<DefaultDialogueCondition> conditions)
    {
        foreach (var condition in conditions)
        {
            // ���� �˻� ����
            bool conditionMet = true;

            // �Ϸ�� ����Ʈ ������ �ִ°�?
            if (!string.IsNullOrEmpty(condition.RequiredCompletedQuest))
            {
                Quest quest = QuestManager.Instance.GetQuestById(condition.RequiredCompletedQuest);
                if (quest == null || quest.QuestState != QuestState.FINISHED)
                {
                    conditionMet = false;
                }
            }

            // Ư�� ����Ʈ ���� ������ �ִ°�?
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

            // '�ݵ�� �������� �ʾҾ�� �ϴ�' �÷��� ������ �ִ°�?
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

        // � Ư���� ���ǵ� �������� ���ߴٸ�, ���� ���� �켱������ �⺻ ��ȭ ID�� ��ȯ
        return 1;
    }

    /// <summary>
    /// ���� �� ���� �Ѿ. ��ȭ�� ������ ���� ó��
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
                DisplayDialogue(currentNPCName, currentDialogueId, currentLineIndex); // ���� ��ȭ�� �ε����� ���� ���� �ε��� ���
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