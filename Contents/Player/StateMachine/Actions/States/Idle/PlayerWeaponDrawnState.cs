using UnityEngine;

/// <summary>
/// �÷��̾ ���⸦ ���� �� ���� �غ�(WeaponDrawn) ���¸� ����
/// ��κ��� ���� �Է� ó���� �θ��� PlayerActionState�� �⺻ ������ ��ӹ޾� ���
/// </summary>
public class PlayerWeaponDrawnState : PlayerActionState
{
    public PlayerWeaponDrawnState(PlayerActionStateMachine playerActionStateMachine) : base(playerActionStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    protected override void OnHitEnter(Collider collider)
    {
        base.OnHitEnter(collider);

        stateMachine.player.Anim.SetTrigger("PlayerHit2");
    }


    public override void OnAnimationTransitionEvent()
    {
        base.OnAnimationTransitionEvent();

        stateMachine.player.HitBoxCollider.enabled = true;
    }
}