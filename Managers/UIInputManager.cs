using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// UI ���� ��ǲ ó���� ����ϴ� Ŭ����
/// PlayerInput�� UI �׼Ǹ� �̺�Ʈ�� GameEventsManager�� �����ϴ� ����
/// </summary>
public class UIInputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    public void Initialize(PlayerInput input)
    {
        playerInput = input;

        if (playerInput.UIActions.Inventory == null) Debug.Log("�÷��̾� ��ǲ�� ����");

        playerInput.UIActions.QuestLog.performed += QuestLogTogglePressed;
        playerInput.UIActions.SkillTree.performed += SkillTreeTogglePressed;
        playerInput.UIActions.Inventory.performed += InventoryTogglePressed;
        playerInput.UIActions.Menu.performed += MenuTogglePressed;
        playerInput.UIActions.Status.performed += StatusTogglePressed;
        playerInput.UIActions.Map.performed += MapTogglePressed;
    }

    private void OnEnable()
    {
        if (playerInput == null)
        {
            return;
        }
        playerInput.UIActions.QuestLog.performed += QuestLogTogglePressed;
        playerInput.UIActions.SkillTree.performed += SkillTreeTogglePressed;
        playerInput.UIActions.Inventory.performed += InventoryTogglePressed;
        playerInput.UIActions.Menu.performed += MenuTogglePressed;
        playerInput.UIActions.Status.performed += StatusTogglePressed;
        playerInput.UIActions.Map.performed += MapTogglePressed;
    }

   private void OnDisable()
    {
        if (playerInput == null) return;

        playerInput.UIActions.QuestLog.performed -= QuestLogTogglePressed;
        playerInput.UIActions.SkillTree.performed -= SkillTreeTogglePressed;
        playerInput.UIActions.Inventory.performed -= InventoryTogglePressed;
        playerInput.UIActions.Menu.performed -= MenuTogglePressed;
        playerInput.UIActions.Status.performed -= StatusTogglePressed;
        playerInput.UIActions.Map.performed -= MapTogglePressed;
    }

    public void QuestLogTogglePressed(InputAction.CallbackContext context)
    {
        GameEventsManager.Instance.InputEvents.QuestLogTogglePressed();
    }

    public void MenuTogglePressed(InputAction.CallbackContext context)
    {
        GameEventsManager.Instance.InputEvents.MenuTogglePressed();
    }

    public void InventoryTogglePressed(InputAction.CallbackContext context)
    {
        GameEventsManager.Instance.InputEvents.InventoryTogglePressed();
    }

    public void SkillTreeTogglePressed(InputAction.CallbackContext context)
    {
        GameEventsManager.Instance.InputEvents.SkillTreeTogglePressed();
    }

    public void StatusTogglePressed(InputAction.CallbackContext context)
    {
        GameEventsManager.Instance.InputEvents.StatusTogglePressed();
    }

    public void MapTogglePressed(InputAction.CallbackContext context)
    {
        GameEventsManager.Instance.InputEvents.MapTogglePressed();
    }
}