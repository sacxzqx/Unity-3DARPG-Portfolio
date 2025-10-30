using UnityEngine;

/// <summary>
/// 스킬 시전 중 상태를 관리하는 클래스
/// 부모의 기본 동작과 달리, 피격 상태(HitState)로 전환되지 않고 데미지만 입음(슈퍼 아머)
/// 이를 통해 플레이어는 적의 공격을 버티면서 스킬을 끝까지 사용할 수 있음
/// </summary>
public class PlayerSkillState : PlayerActionState
{
    public PlayerSkillState(PlayerActionStateMachine playerActionStateMachine) : base(playerActionStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if (stateMachine.player.CurrentSkill == null)
        {
            stateMachine.SetState(stateMachine.WeaponDrawnState);
            return;
        }

        StopAnimation(stateMachine.player.AnimationsData.MovingParameterHash); // 다른 이동 애니메이션이 스킬 애니메이션을 방해하지 않도록 먼저 비활성화
        StartAnimation(stateMachine.player.AnimationsData.SkillParameterHash);
        stateMachine.player.Anim.SetTrigger(stateMachine.player.CurrentSkill.AnimationTrigger);

        stateMachine.player.Input.DisablePlayerActions();

        stateMachine.player.Anim.applyRootMotion = true;
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.SkillParameterHash);

        stateMachine.player.Input.EnablePlayerActions();
        stateMachine.player.Anim.applyRootMotion = false;
    }

    protected override void OnHitEnter(Collider collider)
    {
        int damage = CalculateDamage(collider);

        ApplyDamage(damage);
    }

    public override void OnAnimationExitEvent()
    {
        base.OnAnimationExitEvent();

        stateMachine.SetState(stateMachine.WeaponDrawnState);
    }
}
