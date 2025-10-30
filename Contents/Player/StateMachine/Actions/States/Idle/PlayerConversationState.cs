using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 대화 상태(ConversationState)를 관리하는 클래스
/// 다른 게임플레이 입력을 비활성화하고 'Conversation' 액션 맵으로 전환하여
/// 대화 관련 입력만 가능하도록 설정. 또한, UI 상호작용을 위해 마우스 커서를 활성화
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
