using UnityEngine;

/// <summary>
/// 플레이어의 '패링(Parry)'이 가능한 짧은 시간 동안의 상태를 관리하는 클래스
/// 이 상태는 DefenseState에서 애니메이션 이벤트를 통해 진입
/// 이 상태에서 피격당하면 패링 성공으로 간주되며,
/// 애니메이션이 끝날 때까지 피격되지 않으면 패링 실패로 간주되어 방어 상태로 되돌아감
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
