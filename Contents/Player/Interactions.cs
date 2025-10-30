using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// 트리거를 통한 외부 객체와의 상호작용 로직을 구현한 컴포넌트
/// </summary>
public class Interactions : MonoBehaviour
{
    [SerializeField] private PlayerContext playerContext;

    [SerializeField] private TextMeshProUGUI dialogueText;
    public TextMeshProUGUI DialogueText
    {
        get => dialogueText;
        set => dialogueText = value;
    }

    [SerializeField] private Camera mainCamera;
    public Camera MainCamera
    {
        get => mainCamera;
        set => mainCamera = value;
    }

    private IInteractable interactObj;
    private BoxCollider interactionBoxCollider;

    private void Start()
    {
        interactionBoxCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.DialogueEvents.OnDialogueEnd += HandleDialogueEnd;
        playerContext.Input.UIActions.Interaction.performed += Interact;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.DialogueEvents.OnDialogueEnd -= HandleDialogueEnd;
        playerContext.Input.UIActions.Interaction.performed -= Interact;
    }

    /// <summary>
    /// 상호작용 텍스트 UI가 카메라를 바라보도록 함
    /// </summary>
    private void LateUpdate()
    {
        if (dialogueText.gameObject.activeSelf)
        {
            Vector3 directionToCamera = mainCamera.transform.position - dialogueText.transform.position;
            directionToCamera.y = 0;
            Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
            rotationToCamera = Quaternion.Euler(rotationToCamera.eulerAngles.x, rotationToCamera.eulerAngles.y + 180f, rotationToCamera.eulerAngles.z);
            dialogueText.transform.rotation = rotationToCamera;
        }
    }

    /// <summary>
    /// 트리거를 통하여 객체를 분류하고 상호 작용
    /// </summary>
    /// <param name="other"> 상호작용 객체 </param>
    private void OnTriggerStay(Collider other)
    {
        if (playerContext.ActionStateMachine.CurrentState == playerContext.ActionStateMachine.ConversationState)
        {
            return;
        }

        IInteractable detectedObj = other.GetComponent<IInteractable>();
        if (detectedObj != null)
        {
            interactObj = detectedObj;

            switch (interactObj.InteractionType)
            {
                case InteractionType.NPC:
                    dialogueText.transform.position = other.transform.position + Vector3.up * 2;
                    dialogueText.text = "대화하기";
                    dialogueText.gameObject.SetActive(true);
                    break;

                case InteractionType.Item:
                    ItemSO item = other.GetComponent<ItemObject>().DropedItem;
                    if (item != null)
                    {
                        GameEventsManager.Instance.UIEvents.DiscoverItem(item, true);
                    }
                    break;
                case InteractionType.Portal:
                    dialogueText.transform.position = other.transform.position + Vector3.up * 2;
                    dialogueText.text = "이동하기"; 
                    dialogueText.gameObject.SetActive(true);
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DisableInteraction();

        if (other.CompareTag("Item"))
        {
            GameEventsManager.Instance.UIEvents.DiscoverItem(null, false);
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (interactObj != null)
        {
            IInteractable currentInteractObj = interactObj;

            if (interactObj.InteractionType == InteractionType.NPC)
            {
                playerContext.ActionStateMachine.SetState(playerContext.ActionStateMachine.ConversationState);
                DialogueText.text = "";
            }

            currentInteractObj.Interact();

            DisableInteraction();

            // 아이템을 획득한 경우, 콜라이더를 재활성화하여 다음 객체를 감지
            if (currentInteractObj.InteractionType == InteractionType.Item)
            {
                interactionBoxCollider.enabled = false;
                interactionBoxCollider.enabled = true;
            }
        }
    }

    public void DisableInteraction()
    {
        DialogueText.text = "";
        interactObj = null;
        DialogueText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 대화 종료 이벤트에 반응하여 실행되는 콜백 함수
    /// 플레이어 상태를 원래대로 되돌리고, 상호작용 관련 정보를 초기화
    /// </summary>
    public void HandleDialogueEnd()
    {
        playerContext.ActionStateMachine.SetState(playerContext.ActionStateMachine.SheathedState);
        DialogueText.gameObject.SetActive(false);
        interactObj = null;

        if (interactionBoxCollider != null)
        {
            interactionBoxCollider.enabled = false;
            interactionBoxCollider.enabled = true;
        }
    }
}
