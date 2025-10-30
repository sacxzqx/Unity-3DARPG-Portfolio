using UnityEngine;

/// <summary>
/// 플레이어의 처형 공격(Kill Move) 상태를 관리하는 클래스
/// 이 상태는 자세가 무너진 특정 적을 대상으로 발동되는 일종의 시네마틱 상태로
/// 상태가 진행되는 동안 플레이어는 무적 상태가 되며, 모든 조작이 비활성화됨
/// 처형 애니메이션은 대상 적의 데이터에 따라 결정되며, 애니메이션이 끝나면 전투 준비 상태로 돌아감
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

        // 처형 연출 중에는 플레이어와 적이 임의로 움직이거나 입력을 받지 않도록 함
        stateMachine.player.Rigidbody.velocity = Vector3.zero;
        stateMachine.player.Input.DisablePlayerActions();

        // 연출 도중 다른 객체와 충돌하여 데미지를 주거나 받지 않도록 각자의 HitBox를 비활성화함
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
        // 무적 처리
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
