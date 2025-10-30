using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// ��ȭ UI�� �����ϴ� Ŭ����. ��ȭ ����, ����, Ÿ���� ȿ��, ������ ���� ���� ó��
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject contentParent;

    [SerializeField] private GameObject choiceBox;
    [SerializeField] private GameObject choiceButtonPrefab;

    [SerializeField] private RectTransform nextIcon;
    private Vector3 nextIconInitialPosition;

    private void Awake()
    {
        nextIconInitialPosition = nextIcon.localPosition;
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.DialogueEvents.OnDialogueStart += ShowDialogueBox;
        GameEventsManager.Instance.DialogueEvents.OnDialogueEnd += HideDialogueBox;
        GameEventsManager.Instance.DialogueEvents.OnNextDialogue += ShowNextIcon;
        GameEventsManager.Instance.DialogueEvents.OnShowChoices += ShowChoices;
        GameEventsManager.Instance.DialogueEvents.OnTypingStart += StartTyping;
        GameEventsManager.Instance.DialogueEvents.OnHideChoices += HideChoices;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.DialogueEvents.OnDialogueStart -= ShowDialogueBox;
        GameEventsManager.Instance.DialogueEvents.OnDialogueEnd -= HideDialogueBox;
        GameEventsManager.Instance.DialogueEvents.OnNextDialogue -= ShowNextIcon;
        GameEventsManager.Instance.DialogueEvents.OnShowChoices -= ShowChoices;
        GameEventsManager.Instance.DialogueEvents.OnTypingStart -= StartTyping;
        GameEventsManager.Instance.DialogueEvents.OnHideChoices -= HideChoices;
    }

    private void ShowDialogueBox(string characterName, string dialogue)
    {
        contentParent.SetActive(true);
        characterNameText.text = characterName;
        dialogueText.text = dialogue;
        nextIcon.gameObject.SetActive(false);
    }

    private void HideDialogueBox()
    {
        contentParent.SetActive(false);
        characterNameText.text = "";
        dialogueText.text = "";
    }

    private void ShowNextIcon()
    {
        nextIcon.gameObject.SetActive(true);
        nextIcon.DOKill();
        nextIcon.localPosition = nextIconInitialPosition;
        nextIcon.DOLocalMoveY(nextIconInitialPosition.y - 10f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    private void ShowChoices(DialogueData.Choice[] choices)
    {
        choiceBox.SetActive(true);
        nextIcon.gameObject.SetActive(false);

        foreach (Transform child in choiceBox.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var choice in choices)
        {
            CreateChoiceButton(choice);
        }
    }

    private void HideChoices()
    {
        choiceBox.SetActive(false);
    }

    private void StartTyping(string dialogueLine)
    {
        StopAllCoroutines();
        StartCoroutine(Typing(dialogueLine));
    }

    /// <summary>
    /// ���� ������ ��ư ���� �� Ŭ�� �̺�Ʈ ���ε�
    /// </summary>
    private void CreateChoiceButton(DialogueData.Choice choice)
    {
        Button choiceButton = Instantiate(choiceButtonPrefab, choiceBox.transform).GetComponent<Button>();

        choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.ChoiceText;

        choiceButton.onClick.RemoveAllListeners();
        choiceButton.onClick.AddListener(() => GameEventsManager.Instance.DialogueEvents.SelectChoice(choice));
    }

    private IEnumerator Typing(string dialogueLine)
    {
        // 1. Ÿ���� �غ�
        dialogueText.text = dialogueLine;
        nextIcon.gameObject.SetActive(false);

        // 2. Ÿ���� ���� �� ���� ���
        AudioManager.Instance.PlaySFX("TypingSound");
        TMPDOText(dialogueText, 0.5f);

        // 3. Ÿ���� �Ϸ� �� �ð� ��� (DOTween�� �Ϸ�Ǵ� �ð� + ���� �ð�)
        yield return new WaitForSeconds(1f);

        // 4. ������ ó�� �� ���� �ܰ�� ��ȯ
        AudioManager.Instance.StopSFX("TypingSound");

        GameEventsManager.Instance.DialogueEvents.CompleteTyping(); // Ÿ���� �Ϸ� �˸�

        nextIcon.gameObject.SetActive(true);
        nextIcon.DOKill();
        nextIcon.localPosition = nextIconInitialPosition;
        nextIcon.DOLocalMoveY(nextIconInitialPosition.y - 10f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    /// <summary>
    /// TextMeshPro ���ڸ� DOTween�� �̿��� �� ���ھ� ���
    /// </summary>
    private void TMPDOText(TextMeshProUGUI text, float duration)
    {
        text.maxVisibleCharacters = 0;
        DOTween.To(x => text.maxVisibleCharacters = (int)x, 0f, text.text.Length, duration);
    }
}
