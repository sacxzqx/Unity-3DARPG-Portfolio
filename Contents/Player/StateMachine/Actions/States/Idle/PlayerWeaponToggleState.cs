using UnityEngine;

/// <summary>
/// ���⸦ �����ų� �ִ� �ִϸ��̼��� ����ϴ� ��ȯ ����
/// �ִϸ��̼� �̺�Ʈ�� ����� �޼��带 ���� ���� ���� ��ġ�� �����ϰ�,
/// �ִϸ��̼��� ������ Drawn �Ǵ� Sheathed ���·� ��ȯ
/// </summary>
public class PlayerWeaponToggleState : PlayerActionState
{
    public PlayerWeaponToggleState(PlayerActionStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if (stateMachine.FormerState == stateMachine.SheathedState)
        {
            SetWeaponEquippedStatus(true);
            StartAnimation(stateMachine.player.AnimationsData.EquipHash);
        }
        else
        {
            SetWeaponEquippedStatus(false);
            StopAnimation(stateMachine.player.AnimationsData.EquipHash);
        }

        StartAnimation(stateMachine.player.AnimationsData.DrawWeaponParameterHash);

        stateMachine.player.Input.DisablePlayerActions();
    }

    public override void ExitState()
    {
        base.ExitState();

        stateMachine.player.Input.EnablePlayerActions();
    }

    public override void OnAnimationEnterEvent()
    {
        base.OnAnimationEnterEvent();

        var currentAnimHash = stateMachine.player.Anim.GetCurrentAnimatorStateInfo(0).shortNameHash;

        if (currentAnimHash == stateMachine.player.AnimationsData.DrawWeaponParameterHash)
        {
            stateMachine.player.PlayerWeapon.transform.SetParent(stateMachine.player.ObjectOfWeaponDrawn.transform);
            stateMachine.player.PlayerWeapon.transform.localPosition = actionData.WeaponTransformData.DrawnPosition;
            stateMachine.player.PlayerWeapon.transform.localRotation = Quaternion.Euler(actionData.WeaponTransformData.DrawnRotation);
        }
    }

    public override void OnAnimationTransitionEvent()
    {
        base.OnAnimationTransitionEvent();

        var currentAnimHash = stateMachine.player.Anim.GetCurrentAnimatorStateInfo(0).shortNameHash;

        if (currentAnimHash == stateMachine.player.AnimationsData.UnequipWeaponParameterHash)
        {
            stateMachine.player.PlayerWeapon.transform.SetParent(stateMachine.player.ObjectOfWeaponSheath.transform);
            stateMachine.player.PlayerWeapon.transform.localPosition = actionData.WeaponTransformData.SheathedPosition;
            stateMachine.player.PlayerWeapon.transform.localRotation = Quaternion.Euler(actionData.WeaponTransformData.SheathedRotation);
        }
    }

    public override void OnAnimationExitEvent()
    {
        base.OnAnimationExitEvent();

        var currentAnimHash = stateMachine.player.Anim.GetCurrentAnimatorStateInfo(0).shortNameHash;

        if (currentAnimHash == stateMachine.player.AnimationsData.DrawWeaponParameterHash)
        {
            stateMachine.SetState(stateMachine.WeaponDrawnState);
        }
        else
        {
            stateMachine.SetState(stateMachine.SheathedState);
        }
    }
}
