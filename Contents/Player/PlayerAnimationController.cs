using UnityEngine;

/// <summary>
/// �÷��̾� �ִϸ��̼ǰ� ���õ� ���� ������ ó���ϴ� ������Ʈ
/// �ִϸ��̼� �̺�Ʈ�� ���� ���� ����, ȸ�� ����, �Է� ť ó�� ���� ����
/// </summary>
public class PlayerAnimationController : MonoBehaviour
{
    private PlayerContext playerContext;

    // �ִϸ��̼� ���� ȸ�� ��� ����
    public bool IsRotationEnabled = false;

    private void Start()
    {
        playerContext = GetComponent<PlayerContext>();
    }

    private void FixedUpdate()
    {
        if (IsRotationEnabled)
        {
            RotatePlayer();
        }
    }

    /// <summary>
    /// ���⸦ ���� ���·� ��ȯ (WeaponDrawnState)
    /// �ִϸ��̼� ���� �� ȣ��Ǿ� ���¸ӽŰ� �ִϸ����� ���¸� ����ȭ
    /// </summary>
    public void EnableWeaponDrawnState()
    {
        if (playerContext.ActionStateMachine.CurrentState == playerContext.ActionStateMachine.ConversationState ||
            playerContext.ActionStateMachine.CurrentState == playerContext.ActionStateMachine.SheathedState) return;

        if (playerContext.ActionStateMachine.CurrentState != playerContext.ActionStateMachine.WeaponDrawnState)
        {
            playerContext.ActionStateMachine.SetState(playerContext.ActionStateMachine.WeaponDrawnState); // �ִϸ��̼��� ������ WeaponDrawnState�� ��ȯ
            playerContext.MovementStateMachine.SetState(playerContext.MovementStateMachine.IdlingState);
            playerContext.Anim.SetBool(playerContext.AnimationsData.MovingParameterHash, false);
        }
    }

    /// <summary>
    /// ���⸦ ������� ���·� ��ȯ (SheathedState)
    /// �ִϸ��̼� ���� �� ȣ��Ǿ� ���¸ӽŰ� �ִϸ����� ���¸� ����ȭ
    /// </summary>
    public void EnableWeaponSheathedState()
    {
        if (playerContext.ActionStateMachine.CurrentState != playerContext.ActionStateMachine.SheathedState)
        {
            playerContext.ActionStateMachine.SetState(playerContext.ActionStateMachine.SheathedState);    
        }
    }

    /// <summary>
    /// �Է� ���۷κ��� ������ �Է��� ���� �ش� ���� �ִϸ��̼��� ���
    /// </summary>
    public void MoveNextAnimation()
    {
        string lastInputName = playerContext.InputBuffer.GetLastInput();

        switch(lastInputName)
        {
            case "Attack": 
                playerContext.Anim.SetTrigger("OnCloseAttackCombo");
                break;
            case "Parry":
                playerContext.Anim.ResetTrigger("OnCloseAttackCombo");
                playerContext.ActionStateMachine.SetState(playerContext.ActionStateMachine.DefenseState);
                playerContext.Anim.SetBool("IsParrying", true);
                break;
        }

        playerContext.InputBuffer.ClearLastInput();
    }

    public void EnableRotation()
    {
        IsRotationEnabled = true;
    }

    public void DisableRotation()
    {
        IsRotationEnabled = false;
    }

    /// <summary>
    /// ���� �̵� ���� �������� ĳ���͸� �ε巴�� ȸ����Ŵ
    /// �ִϸ��̼� ���� ȸ���� �ʿ��� ��� ���
    /// </summary>
    public void RotatePlayer()
    {
        if (playerContext.ActionStateMachine.ReusableData.Direction.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(playerContext.ActionStateMachine.ReusableData.Direction.x, playerContext.ActionStateMachine.ReusableData.Direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            playerContext.Rigidbody.rotation = Quaternion.Slerp(playerContext.Rigidbody.rotation, targetRotation, Time.deltaTime * 5); // 5�� ȸ�� �ӵ� �Ķ����
        }
    }
}