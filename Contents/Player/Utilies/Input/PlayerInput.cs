using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// New Input System ����� �÷��̾� �Է� ó�� ������Ʈ
/// �׼Ǹ� ��ȯ, Ư�� �Է� �Ͻ� ��Ȱ��ȭ, �Է� �̺�Ʈ ���ε� ���� ����
/// </summary>
public class PlayerInput : MonoBehaviour
{
    public PlayerInputActions InputActions { get; private set; }

    public PlayerInputActions.PlayerActions PlayerActions { get; private set; }

    public PlayerInputActions.MovementActions MovementActions { get; private set; }

    public PlayerInputActions.ConversationActions ConversationActions { get; private set; }

    public PlayerInputActions.UIActions UIActions { get; private set; }

    /// <summary>
    /// �Է� �߻� �� �̺�Ʈ. ��: ���� �� "Attack" ���ڿ� ����.
    /// </summary>
    public event Action<string> OnInputReceived;

    private void Awake()
    {
        InputActions = new PlayerInputActions();
        
        PlayerActions = InputActions.Player;
        MovementActions = InputActions.Movement;
        ConversationActions = InputActions.Conversation;
        UIActions = InputActions.UI;

        PlayerActions.Attack.performed += context => OnInputReceived?.Invoke("Attack");
        PlayerActions.Parry.performed += context => OnInputReceived?.Invoke("Parry");
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.InputEvents.OnSetPlayerInput += ToggleGameplayInput;

        PlayerActions.Enable();
        MovementActions.Enable();
        UIActions.Enable();

        GameEventsManager.Instance.InputEvents.OnUIStateChange += DisableAllInputs;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.InputEvents.OnSetPlayerInput -= ToggleGameplayInput;

        InputActions.Disable();

        GameEventsManager.Instance.InputEvents.OnUIStateChange -= DisableAllInputs;
    }

    private void OnDestroy()
    {
        foreach (var action in InputActions)
        {
            action.performed -= context => OnInputReceived?.Invoke(action.name);
        }
    }

    public void ToggleGameplayInput(bool isEnabled)
    {
        if (isEnabled)
        {
            PlayerActions.Enable();
            MovementActions.Enable();
            UIActions.Enable();
        }
        else
        {
            PlayerActions.Disable();
            MovementActions.Disable();
            UIActions.Disable();
        }
    }

    /// <summary>
    /// Ư�� �׼��� ���� �ð� ���� ��Ȱ��ȭ�� �� �ٽ� Ȱ��ȭ
    /// </summary>
    public void DisableActionFor(InputAction action, float seconds)
    {
        StartCoroutine(DisableAction(action, seconds));
    }

    private IEnumerator DisableAction(InputAction action, float seconds)
    {
        action.Disable();

        yield return new WaitForSeconds(seconds);

        action.Enable();
    }

    /// <summary>
    /// �Է� �׼Ǹ��� ��Ȳ�� �°� ��ȯ
    /// </summary>
    public void SwitchActionMap(string state)
    {
        switch (state)
        {
            case "Player":
                DisablePlayerActions();
                PlayerActions.Enable();
                break;
            case "Movement":
                DisablePlayerActions();
                MovementActions.Enable();
                break;
            case "Conversation":
                DisablePlayerActions();
                ConversationActions.Enable();
                break;
            case "UI":
                DisablePlayerActions();
                UIActions.Enable();
                break;
        }
    }

    public void DisableExceptPlayerActions()
    {
        MovementActions.Disable();
        ConversationActions.Disable();
    }

    public void DisablePlayerActions()
    {
        PlayerActions.Disable();
        MovementActions.Disable();
    }

    public void EnablePlayerActions()
    {
        PlayerActions.Enable();
        MovementActions.Enable();
    }

    private void DisableAllInputs(UIType _, bool isOpen)
    {
        if (isOpen)
        {
            DisablePlayerActions();
        }
        else
        {
            EnablePlayerActions();
        }
    }
}
