using System;

/// <summary>
/// 대화 시스템에서 발생하는 이벤트들을 관리하는 클래스
/// UI와 로직을 느슨하게 연결하는 이벤트 중심 구조
/// </summary>
public class DialogueEvents
{
    /// <summary>
    /// 선택지 표시 요청 이벤트 (선택지 배열 전달)
    /// </summary>
    public event Action<DialogueData.Choice[]> OnShowChoices;
    public void ShowChoices(DialogueData.Choice[] choices)
    {
        OnShowChoices?.Invoke(choices);
    }

    public event Action<DialogueData.Choice> OnChoiceSelected;
    public void SelectChoice(DialogueData.Choice choice)
    {
        OnChoiceSelected?.Invoke(choice);
    }

    public event Action OnHideChoices;
    public void HideChoices()
    {
        OnHideChoices?.Invoke();
    }

    public event Action<string> OnTypingStart;
    public void StartTyping(string dialogue)
    {
        OnTypingStart?.Invoke(dialogue);
    }

    public event Action OnTypingComplete;
    public void CompleteTyping()
    {
        OnTypingComplete?.Invoke();
    }

    /// <summary>
    /// 대화 시작 이벤트 (캐릭터 이름, 대사 내용)
    /// </summary>
    public event Action<string, string> OnDialogueStart;
    public void StartDialogue(string characterName, string dialogue)
    {
        OnDialogueStart?.Invoke(characterName, dialogue);
    }

    public event Action OnNextDialogue;
    public void NextDialogue()
    {
        OnNextDialogue?.Invoke();
    }

    public event Action OnDialogueEnd;
    public void EndDialogue()
    {
        OnDialogueEnd?.Invoke();
    }
}
