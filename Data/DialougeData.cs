using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// 하나의 캐릭터 대화 데이터를 정의하는 클래스  
/// 대사 라인, 선택지, 퀘스트 조건 등을 포함
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
        /// UI에 표시될 선택지의 텍스트
        /// </summary>
        [JsonProperty("choiceText")]
        public string ChoiceText { get; set; }

        /// <summary>
        /// 이 선택지를 골랐을 때 이어질 다음 대화의 ID
        /// </summary>
        [JsonProperty("nextDialogueId")]
        public int NextDialogueId { get; set; }

        /// <summary>
        /// 이 선택지를 골랐을 때 실행될 특수 액션의 이름 (예: "StartQuest", "OpenShop")
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// 이 선택지가 표시되기 위해 필요한 퀘스트의 상태 조건
        /// DialogueManager는 이 값을 QuestManager의 실제 퀘스트 상태와 비교
        /// </summary>
        [JsonProperty("requiredQuestState")]
        public string RequiredQuestState { get; set; }

        /// <summary>
        /// 이 선택지가 연결된 퀘스트의 고유 ID
        /// </summary>
        [JsonProperty("questId")]
        public string QuestId { get; set; }
    }
}