using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// 대화 UI를 제어하는 클래스. 대화 시작, 종료, 타이핑 효과, 선택지 생성 등을 처리
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
    /// 개별 선택지 버튼 생성 및 클릭 이벤트 바인딩
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
        // 1. 타이핑 준비
        dialogueText.text = dialogueLine;
        nextIcon.gameObject.SetActive(false);

        // 2. 타이핑 시작 및 사운드 재생
        AudioManager.Instance.PlaySFX("TypingSound");
        TMPDOText(dialogueText, 0.5f);

        // 3. 타이핑 완료 및 시간 대기 (DOTween이 완료되는 시간 + 여유 시간)
        yield return new WaitForSeconds(1f);

        // 4. 마무리 처리 및 다음 단계로 전환
        AudioManager.Instance.StopSFX("TypingSound");

        GameEventsManager.Instance.DialogueEvents.CompleteTyping(); // 타이핑 완료 알림

        nextIcon.gameObject.SetActive(true);
        nextIcon.DOKill();
        nextIcon.localPosition = nextIconInitialPosition;
        nextIcon.DOLocalMoveY(nextIconInitialPosition.y - 10f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    /// <summary>
    /// TextMeshPro 글자를 DOTween을 이용해 한 글자씩 출력
    /// </summary>
    private void TMPDOText(TextMeshProUGUI text, float duration)
    {
        text.maxVisibleCharacters = 0;
        DOTween.To(x => text.maxVisibleCharacters = (int)x, 0f, text.text.Length, duration);
    }
}
