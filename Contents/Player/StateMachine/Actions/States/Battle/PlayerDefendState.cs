using UnityEngine;

/// <summary>
/// 플레이어의 방어 및 패링 시도를 시작하는 상태관리 클래스
/// 진입 시 방어 준비 애니메이션을 재생하며, 이 동안 피격되면 일반 방어(Block)가 발동
/// 애니메이션의 특정 시점이 되면, 패링(Parry) 상태로 자동 전환되었다가 복귀
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
    /// 패링상태가 되기 전, 후로 방어 준비 동작 중에 피격되었을 때 호출
    /// 일반 방어로 처리되어, 데미지를 일부 감소시키고 넉백을 받음
    /// </summary>
    protected override void OnHitEnter(Collider collider)
    {
        stateMachine.player.Anim.CrossFade(stateMachine.player.AnimationsData.HitOnDefenseHash, 0.1f);
        ApplyDamage(CalculateDamage(collider));
        ApplyKnockback(collider.transform, stateMachine.player.Data.ActionData.BlockKnockbackForce);

        AudioManager.Instance.PlaySFX("BlockSound");
    }

    /// <summary>
    /// '일반 방어' 시의 데미지를 계산. 부모의 기본 데미지 계산 결과에서 절반을 감소
    /// </summary>
    protected override int CalculateDamage(Collider collider)
    {
        int defendDamge = base.CalculateDamage(collider) / 2;
        return defendDamge;
    }

    /// <summary>
    /// 방어 애니메이션의 패링의 시작 프레임에서 애니메이션 이벤트를 통해 호출
    /// 현재 상태를 즉시 패링(Parry) 상태로 전환
    /// </summary>
    public override void OnAnimationEnterEvent()
    {
        base.OnAnimationEnterEvent();

        stateMachine.SetState(stateMachine.ParryState);
        stateMachine.player.HitBoxCollider.enabled = false;
    }

    /// <summary>
    /// 피격 시 적의 방향을 기준으로 플레이어에게 넉백을 적용
    /// </summary>
    void ApplyKnockback(Transform enemyTransform, float knockbackForce)
    {
        // 적이 바라보는 방향의 반대 방향으로 밀려남
        Vector3 knockbackDirection = -enemyTransform.forward;
        knockbackDirection.y = 0;
        knockbackDirection.Normalize();

        stateMachine.player.Rigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }
}
