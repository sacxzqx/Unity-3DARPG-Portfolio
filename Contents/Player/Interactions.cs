using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Ʈ���Ÿ� ���� �ܺ� ��ü���� ��ȣ�ۿ� ������ ������ ������Ʈ
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
    /// ��ȣ�ۿ� �ؽ�Ʈ UI�� ī�޶� �ٶ󺸵��� ��
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
    /// Ʈ���Ÿ� ���Ͽ� ��ü�� �з��ϰ� ��ȣ �ۿ�
    /// </summary>
    /// <param name="other"> ��ȣ�ۿ� ��ü </param>
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
                    dialogueText.text = "��ȭ�ϱ�";
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
                    dialogueText.text = "�̵��ϱ�"; 
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

            // �������� ȹ���� ���, �ݶ��̴��� ��Ȱ��ȭ�Ͽ� ���� ��ü�� ����
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
    /// ��ȭ ���� �̺�Ʈ�� �����Ͽ� ����Ǵ� �ݹ� �Լ�
    /// �÷��̾� ���¸� ������� �ǵ�����, ��ȣ�ۿ� ���� ������ �ʱ�ȭ
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
