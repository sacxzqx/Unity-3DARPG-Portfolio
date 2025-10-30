using UnityEngine;

/// <summary>
/// �÷��̾��� '�и�(Parry)'�� ������ ª�� �ð� ������ ���¸� �����ϴ� Ŭ����
/// �� ���´� DefenseState���� �ִϸ��̼� �̺�Ʈ�� ���� ����
/// �� ���¿��� �ǰݴ��ϸ� �и� �������� ���ֵǸ�,
/// �ִϸ��̼��� ���� ������ �ǰݵ��� ������ �и� ���з� ���ֵǾ� ��� ���·� �ǵ��ư�
/// </summary>
public class PlayerParryState : PlayerActionState
{
    public PlayerParryState(PlayerActionStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.DefenseParameterHash);

        stateMachine.player.ParryCollider.enabled = true;
        stateMachine.ReusableData.CanMove = false;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.DefenseParameterHash);

        stateMachine.player.ParryCollider.enabled = false;
        stateMachine.ReusableData.CanMove = true;
    }

    public override void OnAnimationExitEvent()
    {
        base.OnAnimationExitEvent();

        stateMachine.SetState(stateMachine.DefenseState);
        stateMachine.player.HitBoxCollider.enabled = true;
        stateMachine.player.ParryCollider.enabled = false;
    }

    protected override void OnHitEnter(Collider collider)
    {
        if (ParticleManager.Instance != null)
        {
            stateMachine.player.ParryCollider.enabled = false;

            stateMachine.player.Anim.SetTrigger("Player_Parry");
            ParticleManager.Instance.PlayParticle("Parry", collider.transform.position);
            AudioManager.Instance.PlaySFX("ParrySound");

            Enemy enemy = collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.HandleParry();
            }
        }
    }
}
