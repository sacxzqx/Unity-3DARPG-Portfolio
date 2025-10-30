using UnityEngine;

/// <summary>
/// �÷��̾��� ��� �� �и� �õ��� �����ϴ� ���°��� Ŭ����
/// ���� �� ��� �غ� �ִϸ��̼��� ����ϸ�, �� ���� �ǰݵǸ� �Ϲ� ���(Block)�� �ߵ�
/// �ִϸ��̼��� Ư�� ������ �Ǹ�, �и�(Parry) ���·� �ڵ� ��ȯ�Ǿ��ٰ� ����
/// </summary>
public class PlayerDefenseState : PlayerActionState
{
    public PlayerDefenseState(PlayerActionStateMachine playerActionStateMachine) : base(playerActionStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.DefenseParameterHash);

        stateMachine.ReusableData.CanMove = false;

        stateMachine.player.Rigidbody.velocity = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ExitState()
    {
        base.ExitState();

        stateMachine.ReusableData.CanMove = true;

        StopAnimation(stateMachine.player.AnimationsData.DefenseParameterHash);
    }

    /// <summary>
    /// �и����°� �Ǳ� ��, �ķ� ��� �غ� ���� �߿� �ǰݵǾ��� �� ȣ��
    /// �Ϲ� ���� ó���Ǿ�, �������� �Ϻ� ���ҽ�Ű�� �˹��� ����
    /// </summary>
    protected override void OnHitEnter(Collider collider)
    {
        stateMachine.player.Anim.CrossFade(stateMachine.player.AnimationsData.HitOnDefenseHash, 0.1f);
        ApplyDamage(CalculateDamage(collider));
        ApplyKnockback(collider.transform, stateMachine.player.Data.ActionData.BlockKnockbackForce);

        AudioManager.Instance.PlaySFX("BlockSound");
    }

    /// <summary>
    /// '�Ϲ� ���' ���� �������� ���. �θ��� �⺻ ������ ��� ������� ������ ����
    /// </summary>
    protected override int CalculateDamage(Collider collider)
    {
        int defendDamge = base.CalculateDamage(collider) / 2;
        return defendDamge;
    }

    /// <summary>
    /// ��� �ִϸ��̼��� �и��� ���� �����ӿ��� �ִϸ��̼� �̺�Ʈ�� ���� ȣ��
    /// ���� ���¸� ��� �и�(Parry) ���·� ��ȯ
    /// </summary>
    public override void OnAnimationEnterEvent()
    {
        base.OnAnimationEnterEvent();

        stateMachine.SetState(stateMachine.ParryState);
        stateMachine.player.HitBoxCollider.enabled = false;
    }

    /// <summary>
    /// �ǰ� �� ���� ������ �������� �÷��̾�� �˹��� ����
    /// </summary>
    void ApplyKnockback(Transform enemyTransform, float knockbackForce)
    {
        // ���� �ٶ󺸴� ������ �ݴ� �������� �з���
        Vector3 knockbackDirection = -enemyTransform.forward;
        knockbackDirection.y = 0;
        knockbackDirection.Normalize();

        stateMachine.player.Rigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }
}
