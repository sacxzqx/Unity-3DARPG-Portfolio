using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// �ϳ��� ĳ���� ��ȭ �����͸� �����ϴ� Ŭ����  
/// ��� ����, ������, ����Ʈ ���� ���� ����
/// </summary>
[System.Serializable]
public class DialogueData
{
    [JsonProperty("targetName")]
    public string TargetName { get; set; }

    [JsonProperty("dialogues")]
    public Dictionary<string, Dialogue> Dialogues { get; set; }

    [JsonProperty("defaultDialogueConditions")]
    public List<DefaultDialogueCondition> DefaultDialogueConditions { get; set; }

    [System.Serializable]
    public class Dialogue
    {
        [JsonProperty("lines")]
        public Line[] Lines { get; set; }

        [JsonProperty("choices")]
        public Choice[] Choices { get; set; }

        [JsonProperty("setFlagOnCompletion")]
        public string SetFlagOnCompletion { get; set; }
    }

    [System.Serializable]
    public class DefaultDialogueCondition
    {
        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("dialogueId")]
        public int DialogueId { get; set; }

        [JsonProperty("requiredCompletedQuest")]
        public string RequiredCompletedQuest { get; set; }

        [JsonProperty("requiredQuestState")]
        public string RequiredQuestState { get; set; }

        [JsonProperty("questId")]
        public string QuestId { get; set; }

        [JsonProperty("requiredFlag")]
        public string RequiredFlag { get; set; }

        [JsonProperty("requiredMissingFlag")]
        public string RequiredMissingFlag { get; set; }
    }

    [System.Serializable]
    public class Line
    {
        [JsonProperty("line")]
        public string LineText { get; set; }
    }

    [System.Serializable]
    public class Choice
    {
        /// <summary>
        /// UI�� ǥ�õ� �������� �ؽ�Ʈ
        /// </summary>
        [JsonProperty("choiceText")]
        public string ChoiceText { get; set; }

        /// <summary>
        /// �� �������� ����� �� �̾��� ���� ��ȭ�� ID
        /// </summary>
        [JsonProperty("nextDialogueId")]
        public int NextDialogueId { get; set; }

        /// <summary>
        /// �� �������� ����� �� ����� Ư�� �׼��� �̸� (��: "StartQuest", "OpenShop")
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// �� �������� ǥ�õǱ� ���� �ʿ��� ����Ʈ�� ���� ����
        /// DialogueManager�� �� ���� QuestManager�� ���� ����Ʈ ���¿� ��
        /// </summary>
        [JsonProperty("requiredQuestState")]
        public string RequiredQuestState { get; set; }

        /// <summary>
        /// �� �������� ����� ����Ʈ�� ���� ID
        /// </summary>
        [JsonProperty("questId")]
        public string QuestId { get; set; }
    }
}