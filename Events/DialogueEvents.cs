using System;

/// <summary>
/// ��ȭ �ý��ۿ��� �߻��ϴ� �̺�Ʈ���� �����ϴ� Ŭ����
/// UI�� ������ �����ϰ� �����ϴ� �̺�Ʈ �߽� ����
/// </summary>
public class DialogueEvents
{
    /// <summary>
    /// ������ ǥ�� ��û �̺�Ʈ (������ �迭 ����)
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
    /// ��ȭ ���� �̺�Ʈ (ĳ���� �̸�, ��� ����)
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
