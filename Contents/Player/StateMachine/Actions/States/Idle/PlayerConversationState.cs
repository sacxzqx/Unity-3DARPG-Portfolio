using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾��� ��ȭ ����(ConversationState)�� �����ϴ� Ŭ����
/// �ٸ� �����÷��� �Է��� ��Ȱ��ȭ�ϰ� 'Conversation' �׼� ������ ��ȯ�Ͽ�
/// ��ȭ ���� �Է¸� �����ϵ��� ����. ����, UI ��ȣ�ۿ��� ���� ���콺 Ŀ���� Ȱ��ȭ
/// </summary>
public class PlayerConversationState : PlayerActionState
{
    public PlayerConversationState(PlayerActionStateMachine playerActionStateMachine) : base(playerActionStateMachine)
    {
    }

    public override void EnterState()
    {
        stateMachine.player.Input.SwitchActionMap("Conversation");
        stateMachine.player.Input.ConversationActions.NextDialogue.performed += OnInteractionPerformed;
        stateMachine.player.Input.DisablePlayerActions();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void ExitState()
    {
        stateMachine.player.Input.ConversationActions.NextDialogue.performed -= OnInteractionPerformed;
        stateMachine.player.Input.EnablePlayerActions();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnInteractionPerformed(InputAction.CallbackContext context)
    {
        DialogueManager.Instance.NextDialogue();
    }
}
