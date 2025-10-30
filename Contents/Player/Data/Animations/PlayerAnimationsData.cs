using System;
using UnityEngine;

/// <summary>
/// 애니메이터 파라미터 이름을 해시값으로 변환하고 관리하는 데이터 클래스
/// 상태머신에서 애니메이션 전이 시 해시값을 빠르게 사용하기 위해 초기화 필요
/// </summary>
[Serializable]
public class PlayerAnimationsData
{
    [Header("State Group Parameter Names")]
    [SerializeField] private string groundedParameterName = "Grounded";
    [SerializeField] private string movingParameterName = "Moving";
    [SerializeField] private string stoppingParameterName = "Stopping";
    [SerializeField] private string landingParameterName = "Landing";
    [SerializeField] private string airborneParameterName = "Airborne";

    [Header("Grounded Parameter Names")]
    [SerializeField] private string idleParameterName = "isIdling";
    [SerializeField] private string dashParameterName = "isDashing";
    [SerializeField] private string walkParameterName = "isWalking";
    [SerializeField] private string runParameterName = "isRunning";
    [SerializeField] private string sprintParameterName = "isSprinting";
    [SerializeField] private string mediumStopParameterName = "isMediumStopping";
    [SerializeField] private string hardStopParameterName = "isHardStopping";
    [SerializeField] private string rollParameterName = "isRolling";
    [SerializeField] private string hardLandParameterName = "isHardLanding";
    [SerializeField] private string fallParameterName = "isFalling";

    [Header("Action Parameter Names")]
    [SerializeField] private string DefenseParameterName = "isDefending";
    [SerializeField] private string SkillParameterName = "isSkilling";
    [SerializeField] private string KillMoveParameterName = "isKillMoving";
    [SerializeField] private string HitOnSheathedParameterName = "isHitOnSheathed";
    [SerializeField] private string HitOnBattleParameterName = "isHitOnBattle";
    [SerializeField] private string DrawWeaponParameterName = "DrawWeapon";
    [SerializeField] private string UnequipWeaponParameterName = "UnequipWeapon";
    [SerializeField] private string AttackParameterName = "OnCloseAttackCombo";
    [SerializeField] private string HitOnDefenseParameterName = "HitOnDefense";
    [SerializeField] private string IsEquippedParameterName = "isEquipped";
    [SerializeField] private string PlayerDeathParameterName = "PlayerDeath";

    public int GroundedParameterHash {  get; private set; }
    public int MovingParameterHash { get; private set; }
    public int StoppingParameterHash { get; private set; }
    public int LandingParameterHash { get; private set; }
    public int AirborneParameterHash { get; private set; }

    public int IdleParameterHash { get; private set; }
    public int DashParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }
    public int SprintParameterHash { get; private set; }
    public int MediumStopParameterHash { get; private set; }
    public int HardStopParameterHash { get; private set; }
    public int RollParameterHash { get; private set; }
    public int HardLandParameterHash { get; private set; }

    public int FallParameterHash { get; private set; }

    public int DefenseParameterHash { get; private set; }
    public int SkillParameterHash { get; private set; }
    public int KillMoveParameterHash { get; private set; }
    public int HitOnSheathedParameterHash { get; private set; }
    public int HitOnBattleParameterHash { get; private set; }
    public int DrawWeaponParameterHash { get; private set; }
    public int UnequipWeaponParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int HitOnDefenseHash { get; private set; }
    public int EquipHash { get; private set; }
    public int PlayerDeath { get; private set; }

    public void Initialize()
    {
        GroundedParameterHash = Animator.StringToHash(groundedParameterName);
        MovingParameterHash = Animator.StringToHash(movingParameterName);
        StoppingParameterHash = Animator.StringToHash(stoppingParameterName);
        LandingParameterHash = Animator.StringToHash(landingParameterName);
        AirborneParameterHash = Animator.StringToHash(airborneParameterName);

        IdleParameterHash = Animator.StringToHash(idleParameterName);
        DashParameterHash = Animator.StringToHash(dashParameterName);
        WalkParameterHash = Animator.StringToHash(walkParameterName);
        RunParameterHash = Animator.StringToHash(runParameterName);
        SprintParameterHash = Animator.StringToHash(sprintParameterName);
        MediumStopParameterHash = Animator.StringToHash(mediumStopParameterName);
        HardStopParameterHash = Animator.StringToHash(hardStopParameterName);
        RollParameterHash = Animator.StringToHash(rollParameterName);
        HardLandParameterHash = Animator.StringToHash(hardLandParameterName);

        FallParameterHash = Animator.StringToHash(fallParameterName);

        DefenseParameterHash = Animator.StringToHash(DefenseParameterName);
        SkillParameterHash = Animator.StringToHash(SkillParameterName);
        KillMoveParameterHash = Animator.StringToHash(KillMoveParameterName);
        HitOnSheathedParameterHash = Animator.StringToHash(HitOnSheathedParameterName);
        HitOnBattleParameterHash = Animator.StringToHash(HitOnBattleParameterName);
        DrawWeaponParameterHash = Animator.StringToHash(DrawWeaponParameterName);
        UnequipWeaponParameterHash = Animator.StringToHash(UnequipWeaponParameterName);
        AttackParameterHash = Animator.StringToHash(AttackParameterName);
        HitOnDefenseHash = Animator.StringToHash (HitOnDefenseParameterName);
        EquipHash = Animator.StringToHash(IsEquippedParameterName);
        PlayerDeath = Animator.StringToHash(PlayerDeathParameterName);
    }
}
