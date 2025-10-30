using UnityEngine;

/// <summary>
/// 플레이어가 무기를 꺼내 든 전투 준비(WeaponDrawn) 상태를 관리
/// 대부분의 전투 입력 처리는 부모인 PlayerActionState의 기본 동작을 상속받아 사용
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