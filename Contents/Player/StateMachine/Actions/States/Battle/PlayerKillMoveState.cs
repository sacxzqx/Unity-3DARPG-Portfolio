using UnityEngine;

/// <summary>
/// �÷��̾��� ó�� ����(Kill Move) ���¸� �����ϴ� Ŭ����
/// �� ���´� �ڼ��� ������ Ư�� ���� ������� �ߵ��Ǵ� ������ �ó׸�ƽ ���·�
/// ���°� ����Ǵ� ���� �÷��̾�� ���� ���°� �Ǹ�, ��� ������ ��Ȱ��ȭ��
/// ó�� �ִϸ��̼��� ��� ���� �����Ϳ� ���� �����Ǹ�, �ִϸ��̼��� ������ ���� �غ� ���·� ���ư�
/// </summary>
public class PlayerKillMoveState : PlayerActionState
{
    private Enemy targetEnemy;

    public PlayerKillMoveState(PlayerActionStateMachine playerActionStateMachine) : base(playerActionStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        GameEventsManager.Instance.PlayerEvents.StartKillMove();

        // ó�� ���� �߿��� �÷��̾�� ���� ���Ƿ� �����̰ų� �Է��� ���� �ʵ��� ��
        stateMachine.player.Rigidbody.velocity = Vector3.zero;
        stateMachine.player.Input.DisablePlayerActions();

        // ���� ���� �ٸ� ��ü�� �浹�Ͽ� �������� �ְų� ���� �ʵ��� ������ HitBox�� ��Ȱ��ȭ��
        targetEnemy.HitBoxCollider.enabled = false;
        stateMachine.player.HitBoxCollider.enabled = false;

        SetKillMove();

        StartAnimation(stateMachine.player.AnimationsData.KillMoveParameterHash);
    }

    public override void ExitState()
    {
        base.ExitState();

        stateMachine.player.Input.EnablePlayerActions();

        stateMachine.player.HitBoxCollider.enabled = true;

        StopAnimation(stateMachine.player.AnimationsData.KillMoveParameterHash);
    }

    public override void OnAnimationExitEvent()
    {
        base.OnAnimationExitEvent();
        stateMachine.SetState(stateMachine.WeaponDrawnState);
    }

    protected override void OnHitEnter(Collider collider)
    {
        // ���� ó��
    }

    public void SetTarget(Enemy enemy)
    {
        targetEnemy = enemy;
    }

    private void SetKillMove()
    {
        targetEnemy.TriggerKillmove();
        targetEnemy.MoveToKillMovePosition(stateMachine.player.transform.position, stateMachine.player.transform.rotation);
                                                                   
        stateMachine.player.Anim.SetTrigger(targetEnemy.KillMoveData.PlayerAnimationTrigger);
    }
}
